using System;
using System.Collections.Generic;
using QTFK.Models;
using System.Linq;
using QTFK.Services.ConsoleArgsBuilders;
using QTFK.Extensions.Collections.Casting;
using QTFK.Extensions;
using QTFK.Extensions.Collections.SwitchCase;
using QTFK.Extensions.Collections.Strings;

namespace QTFK.Services.ConsoleArgsServices
{
    public partial class ConsoleArgsService : IConsoleArgsService
    {
        public bool CaseSensitive { get; set; }
        public string Prefix { get; set; }
        public string Description { get; set; }
        public ArgumentInfo HelpArgument { get; set; }
        public bool ShowHelpOnError { get; set; }
        public ArgsErrorDelegate OnError { get; set; }
        public ArgsUsageDelegate OnUsage { get; set; }
        public Action OnFatal { get; set; }

        private void prv_init()
        {
            Asserts.isFilled(this.Prefix, "It is mandatory the use of a prefix for options.");

            this.Description = this.Description ?? string.Empty;
            this.HelpArgument = this.HelpArgument ?? ArgumentInfo.createHelp("help", string.Empty);
        }

        public static IConsoleArgsService createDefault()
        {
            IConsoleArgsService service;

            service = new ConsoleArgsService()
                .As<IConsoleArgsService>()
                .setCaseSensitive(false)
                .setHelp("help", "Shows this help page.")
                .setPrefix("--")
                .setErrorHandler(e => Console.Error.WriteLine($"{e.Message}"))
                .setTerminationHandler(() => Environment.Exit(1))
                ;

            service.OnUsage = (descrip, options) =>
            {
                options = options
                    .OrderByDescending(o => o.IsIndexed)
                    .ThenBy(o => o.IsOptional)
                    .ThenBy(o => o.IsFlag)
                    .ThenBy(o => o.Name)
                    ;

                var optionsCommand = options
                    .Case(o => $"<{o.Name}>", o => o.IsIndexed)
                    .Case(o => $"{service.Prefix + o.Name + $" <{o.Name}>"}", o => !o.IsFlag)
                    .CaseElse(o => $"{service.Prefix + o.Name}")
                    .Stringify(" ")
                    ;

                var optionsList = options
                    .Case(o => $"{"",5}{o.Name,-21}{o.Description}", o => o.IsIndexed && !o.IsOptional)
                    .Case(o => $"{"",5}{o.Name,-21}{o.Description}{Environment.NewLine,-28}(default: {o.Default})", o => o.IsIndexed)
                    .Case(o => $"{"",5}{service.Prefix + o.Name + $" <{o.Name}>",-21}{o.Description}", o => !o.IsOptional)
                    .Case(o => $"{"",5}{service.Prefix + o.Name + $" <{o.Name}>",-21}{o.Description}{Environment.NewLine,-28}(default: {o.Default})", o => !o.IsFlag)
                    .CaseElse(o => $"{"",5}{service.Prefix + o.Name,-21}{o.Description}")
                    .Stringify(Environment.NewLine)
                    ;
                Console.Error.WriteLine($@"

    {descrip}

    Usage:
        {optionsCommand}

    Options:

{optionsList}
");
            };

            return service;
        }

        public T Parse<T>(IEnumerable<string> args, Func<IConsoleArgsBuilder, T> builder) where T : class
        {
            prv_init();

            var stringComparer = CaseSensitive
                ? StringComparer.InvariantCulture
                : StringComparer.InvariantCultureIgnoreCase
                ;

            var argsInfo = new Dictionary<string, ArgumentInfo>(stringComparer);
            var result = new Result<T>(() => builder(new ExplorerConsoleArgsBuilder(argsInfo)));
            if (!result.Ok)
                throw new ArgumentException($"Can not obtain arguments data. Unexpected error building output type: '{result.Exception.Message}'", result.Exception);

            Action showHelp = () =>
            {
                this.OnUsage?.Invoke(Description, argsInfo
                    .Values
                    .Concat(new ArgumentInfo[] { HelpArgument })
                    );
            };

            if (args.Contains($"{Prefix}{HelpArgument.Name}", stringComparer))
            {
                showHelp();
                OnFatal?.Invoke();
                return null;
            }

            IConsoleArgsBuilder argsBuilder = new ConsoleArgsBuilder(this, args, argsInfo, stringComparer);
            var reportedErrors = new List<Exception>();
            argsBuilder.ErrorFound = e =>
            {
                reportedErrors.Add(e);
                this.OnError?.Invoke(e);
            };

            result = new Result<T>(() => builder(argsBuilder));

            if (result.Ok)
            {
                if (reportedErrors.Any())
                {
                    if (ShowHelpOnError)
                        showHelp();
                    OnFatal?.Invoke();
                    return null;
                }
                else
                {
                    return result.Value;
                }
            }

            this.OnError?.Invoke(new Exception($"Un expected error '{result.Exception.Message}'", result.Exception));
            this.OnFatal?.Invoke();
            return null;
        }

    }
}