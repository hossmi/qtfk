using System;
using System.Collections.Generic;
using QTFK.Extensions.Collections.Dictionaries;
using QTFK.Extensions.Collections.Filters;
using QTFK.Models;

namespace QTFK.Services
{
    public partial class ConsoleArgsService : IConsoleArgsService
    {
        public bool CaseSensitive { get; set; }
        public string Prefix { get; set; }
        public string Description { get; set; }
        public string HelpOptionName { get; set; }
        public string HelpDescription { get; set; }

        public event Action<ArgumentException> Error;
        public event Action<string> Usage;
        public event Action<ArgumentInfo> UsageOption;

        public T Parse<T>(IEnumerable<string> args, Func<IConsoleArgsBuilder, T> builder) where T: class
        {
            if (string.IsNullOrWhiteSpace(Prefix))
                throw new ArgumentNullException(nameof(Prefix), "It is mandatory the use of a prefix for options.");

            Description = Description ?? string.Empty;
            HelpOptionName = HelpOptionName ?? "help";
            HelpDescription = HelpDescription ?? string.Empty;
            bool thereAreErrors = false;

            IConsoleArgsBuilder argsBuilder = new ConsoleArgsBuilder(
                this
                , e => {
                    thereAreErrors = true;
                    Error?.Invoke(e);
                }
                , Usage
                , UsageOption
                , args
                );

            var result = new Result<T>(() => builder(argsBuilder));

            if (result.Ok)
                return thereAreErrors 
                    ? null 
                    : result.Value
                    ;

            Error(new ArgumentException($"Un expected error '{result.Exception.Message}'", result.Exception));
            return null;
        }
    }
}