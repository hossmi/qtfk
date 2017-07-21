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

        public static IConsoleArgsService SetErrorHandler(this IConsoleArgsService service, ArgsErrorDelegate onError)
        {
            service.ErrorMessage = onError;
            return service;
        }

        public static IConsoleArgsService SetUsageHandler(this IConsoleArgsService service, ArgsUsageDelegate onUsage)
        {
            service.UsageMessage = onUsage;
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
            service.HelpArgument = new ArgumentInfo
            {
                Name = name,
                Description = description,
                IsFlag = true,
                IsIndexed = false,
                IsOptional = true,
            };
            return service;
        }





        public static T Required<T>(this IConsoleArgsBuilder builder, string name, string description) where T : struct
        {
            string result = builder.Required(name, description);
            if (string.IsNullOrEmpty(result))
                return default(T);

            var converter = TypeDescriptor.GetConverter(typeof(T));
            return (T)converter.ConvertFromString(result);
        }

        public static T Optional<T>(this IConsoleArgsBuilder builder, string name, string description, T defaultValue)
        {
            string result = builder.Optional(name, description, defaultValue.ToString());
            var converter = TypeDescriptor.GetConverter(defaultValue);
            return (T)converter.ConvertFromString(result);
        }

        public static T Optional<T>(this IConsoleArgsBuilder builder, string name, string description, T defaultValue, Func<string, T> customConverter)
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
