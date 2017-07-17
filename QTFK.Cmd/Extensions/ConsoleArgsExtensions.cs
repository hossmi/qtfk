using QTFK.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTFK.Extensions
{
    public static class ConsoleArgsExtensions
    {
        public static IConsoleArgsService SetPrefix(this IConsoleArgsService service, string prefix)
        {
            service.Prefix = prefix;
            return service;
        }

        public static IConsoleArgsService SetCaseSensitive(this IConsoleArgsService service, bool value)
        {
            service.CaseSensitive = value;
            return service;
        }

        public static IConsoleArgsService SetErrorHandler(this IConsoleArgsService service, Action<ArgumentException> onError)
        {
            service.OnError = onError;
            return service;
        }
    }
}
