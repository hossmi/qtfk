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
        public static IConsoleArgsService setPrefix(this IConsoleArgsService service, string prefix)
        {
            service.Prefix = prefix;
            return service;
        }

        public static IConsoleArgsService setCaseSensitive(this IConsoleArgsService service, bool value)
        {
            service.CaseSensitive = value;
            return service;
        }

        public static IConsoleArgsService setErrorHandler(this IConsoleArgsService service, ArgsErrorDelegate onError)
        {
            service.OnError = onError;
            return service;
        }

        public static IConsoleArgsService setUsageHandler(this IConsoleArgsService service, ArgsUsageDelegate onUsage)
        {
            service.OnUsage = onUsage;
            return service;
        }

        public static IConsoleArgsService setDescription(this IConsoleArgsService service, string description)
        {
            service.Description = description;
            return service;
        }

        public static IConsoleArgsService setShowHelpOnError(this IConsoleArgsService service, bool value)
        {
            service.ShowHelpOnError = value;
            return service;
        }

        public static IConsoleArgsService setHelp(this IConsoleArgsService service, string name, string description)
        {
            service.HelpArgument = ArgumentInfo.createHelp(name, description);
            return service;
        }

        public static IConsoleArgsService setTerminationHandler(this IConsoleArgsService service, Action termination)
        {
            service.OnFatal = termination;
            return service;
        }





        public static T setRequired<T>(this IConsoleArgsBuilder builder, string name, string description) where T : struct
        {
            string result = builder.getRequired(name, description);
            if (string.IsNullOrEmpty(result))
                return default(T);

            var converter = TypeDescriptor.GetConverter(typeof(T));
            return (T)converter.ConvertFromString(result);
        }

        public static T setOptional<T>(this IConsoleArgsBuilder builder, string name, string description, T defaultValue)
        {
            string result = builder.getOptional(name, description, defaultValue.ToString());
            var converter = TypeDescriptor.GetConverter(defaultValue);
            return (T)converter.ConvertFromString(result);
        }

        public static T setOptional<T>(this IConsoleArgsBuilder builder, string name, string description, T defaultValue, Func<string, T> customConverter)
        {
            string result = builder.getOptional(name, description, defaultValue.ToString());
            return customConverter(result);
        }

        public static T setOptional<T>(this IConsoleArgsBuilder builder, int index, string name, string description, T defaultValue)
        {
            string result = builder.getOptional(index, name, description, defaultValue.ToString());
            var converter = TypeDescriptor.GetConverter(defaultValue);
            return (T)converter.ConvertFromString(result);
        }
        public static T setOptional<T>(this IConsoleArgsBuilder builder, int index, string name, string description, T defaultValue, Func<string, T> customConverter)
        {
            string result = builder.getOptional(index, name, description, defaultValue.ToString());
            return customConverter(result);
        }
    }
}
