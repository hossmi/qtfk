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

            int anyErrors = 0;

            IConsoleArgsBuilder argsBuilder = null;
            Action showHelp = () =>
            {
                Usage(Description);
                UsageOption(HelpArgument);
                argsBuilder = new ConsoleArgsUsageBuilder(this, UsageOption);
                builder(argsBuilder);
            };
            if (ExistsHelpFlag(args.ToArray(), 0))
            {
                showHelp();
                return null;
            }

            argsBuilder = new ConsoleArgsBuilder(this, e => { ++anyErrors; Error(e); }, args);

            var result = new Result<T>(() => builder(argsBuilder));

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

        private bool ExistsHelpFlag(string[] args, int i)
        {
            if (i >= args.Length)
                return false;

            if ($"{Prefix}{HelpArgument.Name}" == args[i])
                return true;

            return ExistsHelpFlag(args, i + 1);
        }

    }
}