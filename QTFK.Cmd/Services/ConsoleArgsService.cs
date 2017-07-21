using System;
using System.Collections.Generic;
using QTFK.Models;
using System.Linq;
using QTFK.Extensions.Collections.Strings;
using QTFK.Extensions.Collections.SwitchCase;

namespace QTFK.Services
{
    public partial class ConsoleArgsService : IConsoleArgsService
    {
        public bool CaseSensitive { get; set; }
        public string Prefix { get; set; }
        public string Description { get; set; }
        public ArgumentInfo HelpArgument { get; set; }
        public bool ShowHelpOnError { get; set; }
        public ArgsErrorDelegate ErrorMessage { get; set; }
        public ArgsUsageDelegate UsageMessage { get; set; }

        public T Parse<T>(IEnumerable<string> args, Func<IConsoleArgsBuilder, T> builder) where T : class
        {
            Init();

            var argsInfo = new Dictionary<string, ArgumentInfo>();
            var result = new Result<T>(() => builder(new ExplorerConsoleArgsBuilder(argsInfo)));
            if (!result.Ok)
                throw new ArgumentException($"Can not obtain arguments data. Unexpected error building output type: '{result.Exception.Message}'", result.Exception);

            Action showHelp = () =>
            {
                UsageMessage(Description, argsInfo
                    .Values
                    .Concat(new ArgumentInfo[] { HelpArgument })
                    );
            };

            if (args.Contains($"{Prefix}{HelpArgument.Name}"))
            {
                showHelp();
                return null;
            }


            IConsoleArgsBuilder argsBuilder = new ConsoleArgsBuilder(this, args, argsInfo);
            var reportedErrors = new List<Exception>();
            argsBuilder.Error += e =>
            {
                reportedErrors.Add(e);
                ErrorMessage(e);
            };
            
            result = new Result<T>(() => builder(argsBuilder));

            if (result.Ok)
            {
                if (reportedErrors.Any())
                {
                    if (ShowHelpOnError)
                        showHelp();
                    return null;
                }
                else
                {
                    return result.Value;
                }
            }

            ErrorMessage(new Exception($"Un expected error '{result.Exception.Message}'", result.Exception));
            return null;
        }

        private void Init()
        {
            if (string.IsNullOrWhiteSpace(Prefix))
                throw new ArgumentNullException(nameof(Prefix), "It is mandatory the use of a prefix for options.");

            Description = Description ?? string.Empty;

            HelpArgument = HelpArgument ?? new ArgumentInfo();
            HelpArgument.Name = HelpArgument.Name ?? "help";
            HelpArgument.Description = HelpArgument.Description ?? string.Empty;

            ErrorMessage = ErrorMessage ?? (e => Console.Error.WriteLine($"{e.Message}"));

            if(UsageMessage == null)
                UsageMessage = (descrip, options) =>
                {
                    options = options
                        .OrderByDescending(o => o.IsIndexed)
                        .ThenBy(o => o.IsOptional)
                        .ThenBy(o => o.IsFlag)
                        .ThenBy(o => o.Name)
                        ;

                    var optionsCommand = options
                        .Case(o => $"<{o.Name}>", o => o.IsIndexed)
                        .Case(o => $"{Prefix + o.Name + $" <{o.Name}>"}", o => !o.IsFlag)
                        .CaseElse(o => $"{Prefix + o.Name}")
                        .Stringify(" ")
                        ;
                    var optionsList = options
                        .Case(o => $"{"",5}{o.Name,-21}{o.Description}", o => o.IsIndexed && !o.IsOptional)
                        .Case(o => $"{"",5}{o.Name,-21}{o.Description}{Environment.NewLine,-28}(default: {o.Default})", o => o.IsIndexed)
                        .Case(o => $"{"",5}{Prefix + o.Name + $" <{o.Name}>",-21}{o.Description}", o => !o.IsOptional)
                        .Case(o => $"{"",5}{Prefix + o.Name + $" <{o.Name}>",-21}{o.Description}{Environment.NewLine,-28}(default: {o.Default})", o => !o.IsFlag)
                        .CaseElse(o => $"{"",5}{Prefix + o.Name,-21}{o.Description}")
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
        }
    }
}