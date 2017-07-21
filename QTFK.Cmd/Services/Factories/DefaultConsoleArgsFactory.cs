using QTFK.Extensions;
using QTFK.Extensions.Collections.Casting;
using QTFK.Extensions.Collections.Strings;
using QTFK.Extensions.Collections.SwitchCase;
using System;
using System.Linq;

namespace QTFK.Services.Factories
{
    public class DefaultConsoleArgsFactory : IFactory<IConsoleArgsService>
    {
        public IConsoleArgsService Build()
        {
            var service = new ConsoleArgsService()
                .As<IConsoleArgsService>()
                .SetCaseSensitive(false)
                .SetHelp("help", "Shows this help page.")
                .SetPrefix("--")
                .AddErrorHandler(e => Console.Error.WriteLine($"{e.Message}"))
                ;

            service.Usage += (descrip, options) =>
            {
                options = options
                    .OrderByDescending(o => o.IsIndexed)
                    .ThenBy(o => o.IsOptional)
                    .ThenBy(o => o.IsFlag)
                    .ThenBy(o => o.Name)
                    ;

                var optionsCommand = options
                    .Case(o => $"<{o.Name}>", o => o.IsIndexed)
                    .Case(o => $"{service.Prefix + o.Name + $" <{o.Name}>"}", o => !o.IsFlag)
                    .CaseElse(o => $"{service.Prefix + o.Name}")
                    .Stringify(" ")
                    ;
                var optionsList = options
                    .Case(o => $"{"",5}{o.Name,-21}{o.Description}", o => o.IsIndexed && !o.IsOptional)
                    .Case(o => $"{"",5}{o.Name,-21}{o.Description}{Environment.NewLine,-28}(default: {o.Default})", o => o.IsIndexed)
                    .Case(o => $"{"",5}{service.Prefix + o.Name + $" <{o.Name}>",-21}{o.Description}", o => !o.IsOptional)
                    .Case(o => $"{"",5}{service.Prefix + o.Name + $" <{o.Name}>",-21}{o.Description}{Environment.NewLine,-28}(default: {o.Default})", o => !o.IsFlag)
                    .CaseElse(o => $"{"",5}{service.Prefix + o.Name,-21}{o.Description}")
                    .Stringify(Environment.NewLine)
                    ;
                Console.Error.WriteLine($@"

    {descrip}

    Usage:
        {optionsCommand}

    Options:

{optionsList}
");
            };

            return service;
        }
    }
}