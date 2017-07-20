using QTFK.Models;
using System;
using System.Collections.Generic;

namespace QTFK.Services
{
    public delegate void ArgsErrorDelegate(ArgumentException error);
    public delegate void ArgsUsageDelegate(string description, IEnumerable<ArgumentInfo> options);

    public interface IConsoleArgsService
    {
        bool CaseSensitive { get; set; }
        string Prefix { get; set; }
        string Description { get; set; }
        ArgumentInfo HelpArgument { get; set; }
        bool ShowHelpOnError { get; set; }
        ArgsErrorDelegate ErrorMessage { get; set; }
        ArgsUsageDelegate UsageMessage { get; set; }

        T Parse<T>(IEnumerable<string> args, Func<IConsoleArgsBuilder, T> builder) where T: class;
    }
}