using QTFK.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QTFK.Services
{
    internal class ConsoleArgsUsageBuilder : IConsoleArgsBuilder
    {
        private readonly IConsoleArgsService _service;
        private readonly Action<ArgumentInfo> _onUsageOption;

        public ConsoleArgsUsageBuilder(
            IConsoleArgsService service
            , Action<ArgumentInfo> onUsageOption
            )
        {
            _service = service;
            _onUsageOption = onUsageOption;
        }

        public string Required(string name, string description)
        {
            return Usage(name, description);
        }

        public string Optional(string name, string description, string defaultValue)
        {
            return Usage(name, description, defaultValue);
        }

        public string Required(int index, string name, string description)
        {
            return Usage(name, description);
        }

        public string Optional(int index, string name, string description, string defaultValue)
        {
            return Usage(name, description, defaultValue);
        }

        public bool Flag(string name, string description)
        {
            Usage(name, description);
            return false;
        }

        private string Usage(string name, string description, string defaultValue = null)
        {
            defaultValue = defaultValue ?? string.Empty;

            _onUsageOption?.Invoke(new ArgumentInfo
            {
                Name = name,
                Description = description,
                Default = defaultValue,
            });
            return defaultValue;
        }
    }
}