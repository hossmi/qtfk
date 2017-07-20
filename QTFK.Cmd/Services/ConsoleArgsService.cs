using System;
using System.Collections.Generic;
using QTFK.Models;
using System.Linq;
using QTFK.Extensions.Collections.Strings;

namespace QTFK.Services
{
    public partial class ConsoleArgsService : IConsoleArgsService
    {
        private IList<ArgumentException> _reportedErrors;

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
                    .OrderBy(k => k.Name)
                    );
            };

            if (args.Contains($"{Prefix}{HelpArgument.Name}"))
            {
                showHelp();
                return null;
            }


            IConsoleArgsBuilder argsBuilder = new ConsoleArgsBuilder(this, args, argsInfo);
            result = new Result<T>(() => builder(argsBuilder));

            if (result.Ok)
            {
                if (_reportedErrors.Any())
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

            ErrorMessage(new ArgumentException($"Un expected error '{result.Exception.Message}'", result.Exception));
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

            _reportedErrors = new List<ArgumentException>();
            var subErrorMessage = ErrorMessage ?? (e => Console.Error.WriteLine($"{e.Message} (Argument: {e.ParamName})"));
            ErrorMessage = (e =>
            {
                subErrorMessage(e);
                _reportedErrors.Add(e);
            });
            UsageMessage = UsageMessage ?? ((descrip, options) => Console.Error.WriteLine($@"

    {descrip}

    Usage:
        {options
            .Select(o => $"{(o.IsIndexed ? "" : Prefix)}{o.Name}{(!o.IsFlag && !o.IsIndexed ? $"<{o.Name}>" : "")}")
            .Stringify(" ")}

    Options:

{options
    .Select(o => $"{(o.IsIndexed ? "" : Prefix)}{o.Name}{(!o.IsFlag && !o.IsIndexed ? $"<{o.Name}>" : "\t")}\t{o.Description} Default: '{o.Default}'")
    .Stringify(Environment.NewLine)}
"));
        }
    }
}