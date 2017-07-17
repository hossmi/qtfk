using System;
using System.Collections.Generic;

namespace QTFK.Services
{
    public interface IConsoleArgsService
    {
        bool CaseSensitive { get; set; }
        string Prefix { get; set; }
        Action<ArgumentException> OnError { get; set; }

        T Parse<T>(IEnumerable<string> args, Func<IConsoleArgsBuilder, T> builder) where T: class;
    }

    public interface IConsoleArgsBuilder
    {
        string Get(string name);
        string Get<T>(string name, T defaultValue);
        string Get(int index);
        string Get<T>(int index, T defaultValue);
        bool HasFlag(string name);
    }
}