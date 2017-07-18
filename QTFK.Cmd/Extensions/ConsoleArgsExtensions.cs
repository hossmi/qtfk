using QTFK.Models;
using QTFK.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        public static IConsoleArgsService AddErrorHandler(this IConsoleArgsService service, Action<ArgumentException> onError)
        {
            service.Error += onError;
            return service;
        }

        public static IConsoleArgsService AddUsageHandler(this IConsoleArgsService service, Action<string> onUsage)
        {
            service.Usage += onUsage;
            return service;
        }

        public static IConsoleArgsService AddUsageOptionHandler(this IConsoleArgsService service, Action<ArgumentInfo> onUsageOption)
        {
            service.UsageOption += onUsageOption;
            return service;
        }

        public static IConsoleArgsService SetDescription(this IConsoleArgsService service, string description)
        {
            service.Description = description;
            return service;
        }

        public static IConsoleArgsService SetShowHelpOnError(this IConsoleArgsService service, bool value)
        {
            service.ShowHelpOnError = value;
            return service;
        }

        public static IConsoleArgsService SetHelp(this IConsoleArgsService service, string name, string description)
        {
            return SetHelp(service, new ArgumentInfo
            {
                Name = name,
                Description = description
            });
        }

        public static IConsoleArgsService SetHelp(this IConsoleArgsService service, ArgumentInfo info)
        {
            service.HelpArgument = info;
            return service;
        }

        public static T Optional<T>(this IConsoleArgsBuilder builder, string name, string description, T defaultValue)
        {
            string result = builder.Optional(name, description, defaultValue.ToString());
            var converter = TypeDescriptor.GetConverter(defaultValue);
            return (T)converter.ConvertFromString(result);
        }

        public static T Optional<T>(this IConsoleArgsBuilder builder, string name, string description, T defaultValue, Func<string,T> customConverter)
        {
            string result = builder.Optional(name, description, defaultValue.ToString());
            return customConverter(result);
        }

        public static T Optional<T>(this IConsoleArgsBuilder builder, int index, string name, string description, T defaultValue)
        {
            string result = builder.Optional(index, name, description, defaultValue.ToString());
            var converter = TypeDescriptor.GetConverter(defaultValue);
            return (T)converter.ConvertFromString(result);
        }
        public static T Optional<T>(this IConsoleArgsBuilder builder, int index, string name, string description, T defaultValue, Func<string, T> customConverter)
        {
            string result = builder.Optional(index, name, description, defaultValue.ToString());
            return customConverter(result);
        }
    }
}
