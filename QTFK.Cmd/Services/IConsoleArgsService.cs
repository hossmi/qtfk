using QTFK.Models;
using System;
using System.Collections.Generic;

namespace QTFK.Services
{
    public interface IConsoleArgsService
    {
        bool CaseSensitive { get; set; }
        string Prefix { get; set; }
        event Action<ArgumentException> Error;
        event Action<string> Usage;
        event Action<ArgumentInfo> UsageOption;
        string Description { get; set; }
        ArgumentInfo HelpArgument { get; set; }
        bool ShowHelpOnError { get; set; }

        T Parse<T>(IEnumerable<string> args, Func<IConsoleArgsBuilder, T> builder) where T: class;
    }
}