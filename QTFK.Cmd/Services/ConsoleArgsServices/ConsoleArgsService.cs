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
    public class ConsoleArgsService : IConsoleArgsService
    {
        private class PrvExplorerConsoleArgsBuilder : IConsoleArgsBuilder
        {
            private static string prv_setOptionData(IDictionary<string, ArgumentInfo> data
                , string name, string description
                , bool isOptional, bool isIndexed
                , string defaultValue
                , bool isFlag
                )
            {
                ArgumentInfo argumentInfo;

                argumentInfo = ArgumentInfo.createDefault(name, description, defaultValue, isOptional, isIndexed, isFlag);
                data[name] = argumentInfo;

                return defaultValue;
            }

            private readonly IDictionary<string, ArgumentInfo> data;

            public PrvExplorerConsoleArgsBuilder(IDictionary<string, ArgumentInfo> data)
            {
                Asserts.isSomething(data, $"'{nameof(data)}' cannot be null.");

                this.data = data;
            }

            public ArgsErrorDelegate ErrorFound { get; set; }

            public bool getFlag(string name, string description)
            {
                prv_setOptionData(this.data, name, description, true, false, false.ToString(), true);
                return false;
            }

            public string getOptional(string name, string description, string defaultValue)
            {
                return prv_setOptionData(this.data, name, description, true, false, defaultValue, false);
            }

            public string getOptional(int index, string name, string description, string defaultValue)
            {
                return prv_setOptionData(this.data, name, description, true, true, defaultValue, false);
            }

            public string getRequired(string name, string description)
            {
                return prv_setOptionData(this.data, name, description, false, false, string.Empty, false);
            }

            public string getRequired(int index, string name, string description)
            {
                return prv_setOptionData(this.data, name, description, false, true, string.Empty, false);
            }

        }


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
            StringComparer stringComparer;
            IDictionary<string, ArgumentInfo> argsInfo;
            Result<T> result;
            Action showHelp;
            IConsoleArgsBuilder argsBuilder;
            IList<Exception> reportedErrors;

            prv_init();

            stringComparer = CaseSensitive 
                ? StringComparer.InvariantCulture 
                : StringComparer.InvariantCultureIgnoreCase;

            argsInfo = new Dictionary<string, ArgumentInfo>(stringComparer);
            argsBuilder = new PrvExplorerConsoleArgsBuilder(argsInfo);
            result = new Result<T>(() => builder(argsBuilder));

            if (!result.Ok)
                throw new ArgumentException($"Can not obtain arguments data. Unexpected error building output type: '{result.Exception.Message}'", result.Exception);

            showHelp = () =>
            {
                IEnumerable<ArgumentInfo> argsInfoWithHelp;

                argsInfoWithHelp = argsInfo
                    .Values
                    .Concat(new ArgumentInfo[] { HelpArgument });

                this.OnUsage?.Invoke(Description, argsInfoWithHelp);
            };

            if (args.Contains($"{Prefix}{HelpArgument.Name}", stringComparer))
            {
                showHelp();
                OnFatal?.Invoke();
                return null;
            }

            argsBuilder = new ConsoleArgsBuilder(this, args, argsInfo, stringComparer);
            reportedErrors = new List<Exception>();
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