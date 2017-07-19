using System;
using System.Collections.Generic;
using QTFK.Extensions.Collections.Dictionaries;
using QTFK.Extensions.Collections.Filters;
using QTFK.Models;
using System.Linq;

namespace QTFK.Services
{
    public partial class ConsoleArgsService : IConsoleArgsService
    {
        public bool CaseSensitive { get; set; }
        public string Prefix { get; set; }
        public string Description { get; set; }
        public ArgumentInfo HelpArgument { get; set; }
        public bool ShowHelpOnError { get; set; }

        public event Action<ArgumentException> Error;
        public event Action<string> Usage;
        public event Action<ArgumentInfo> UsageOption;

        public T Parse<T>(IEnumerable<string> args, Func<IConsoleArgsBuilder, T> builder) where T : class
        {
            if (string.IsNullOrWhiteSpace(Prefix))
                throw new ArgumentNullException(nameof(Prefix), "It is mandatory the use of a prefix for options.");

            Description = Description ?? string.Empty;

            HelpArgument = HelpArgument ?? new ArgumentInfo();
            HelpArgument.Name = HelpArgument.Name ?? "help";
            HelpArgument.Description = HelpArgument.Description ?? string.Empty;

            var argsInfo = new Dictionary<string, ArgumentInfo>();
            var result = new Result<T>(() => builder(new ExplorerConsoleArgsBuilder(argsInfo)));
            if (!result.Ok)
                throw new ArgumentException($"Can not obtain arguments data. Unexpected error building output type: '{result.Exception.Message}'", result.Exception);

            int anyErrors = 0;

            Action showHelp = () =>
            {
                Usage(Description);
                argsInfo
                    .Values
                    .Concat(new ArgumentInfo[] { HelpArgument })
                    .OrderBy(k => k.Name)
                    .ToList()
                    .ForEach(UsageOption)
                    ;
            };

            if (args.Contains($"{Prefix}{HelpArgument.Name}"))
            {
                showHelp();
                return null;
            }

            IConsoleArgsBuilder argsBuilder = new ConsoleArgsBuilder(this, e => { ++anyErrors; Error(e); }, args, argsInfo);
            result = new Result<T>(() => builder(argsBuilder));

            if (result.Ok)
            {
                if (anyErrors > 0)
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

            Error(new ArgumentException($"Un expected error '{result.Exception.Message}'", result.Exception));
            return null;
        }
    }
}