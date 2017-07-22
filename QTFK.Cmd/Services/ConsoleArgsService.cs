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
        public event ArgsErrorDelegate Error;
        public event ArgsUsageDelegate Usage;
        public Action OnNullResult { get; set; }

        public T Parse<T>(IEnumerable<string> args, Func<IConsoleArgsBuilder, T> builder) where T : class
        {
            Init();

            var argsInfo = new Dictionary<string, ArgumentInfo>();
            var result = new Result<T>(() => builder(new ExplorerConsoleArgsBuilder(argsInfo)));
            if (!result.Ok)
                throw new ArgumentException($"Can not obtain arguments data. Unexpected error building output type: '{result.Exception.Message}'", result.Exception);

            Action showHelp = () =>
            {
                Usage?.Invoke(Description, argsInfo
                    .Values
                    .Concat(new ArgumentInfo[] { HelpArgument })
                    );
            };

            if (args.Contains($"{Prefix}{HelpArgument.Name}"))
            {
                showHelp();
                OnNullResult?.Invoke();
                return null;
            }


            IConsoleArgsBuilder argsBuilder = new ConsoleArgsBuilder(this, args, argsInfo);
            var reportedErrors = new List<Exception>();
            argsBuilder.Error += e =>
            {
                reportedErrors.Add(e);
                Error?.Invoke(e);
            };
            
            result = new Result<T>(() => builder(argsBuilder));

            if (result.Ok)
            {
                if (reportedErrors.Any())
                {
                    if (ShowHelpOnError)
                        showHelp();
                    OnNullResult?.Invoke();
                    return null;
                }
                else
                {
                    return result.Value;
                }
            }

            Error?.Invoke(new Exception($"Un expected error '{result.Exception.Message}'", result.Exception));
            OnNullResult?.Invoke();
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
        }
    }
}