using QTFK.Extensions.Collections.Dictionaries;
using QTFK.Models;
using System.Collections.Generic;

namespace QTFK.Services
{
    internal class ExplorerConsoleArgsBuilder : IConsoleArgsBuilder
    {
        private readonly IDictionary<string, ArgumentInfo> _data;

        public ExplorerConsoleArgsBuilder(IDictionary<string, ArgumentInfo> data)
        {
            _data = data;
        }

        public bool Flag(string name, string description)
        {
            _data.Set(name, new ArgumentInfo
            {
                Name = name,
                Description = description,
                Default = false.ToString(),
                IsOptional = true,
                IsIndexed = false,
                IsFlag = true,
            });
            return false;
        }

        public string Optional(string name, string description, string defaultValue)
        {
            return SetOptionData(name, description, true, false, defaultValue);
        }

        public string Optional(int index, string name, string description, string defaultValue)
        {
            return SetOptionData(name, description, true, true, defaultValue);
        }

        public string Required(string name, string description)
        {
            return SetOptionData(name, description, false, false);
        }

        public string Required(int index, string name, string description)
        {
            return SetOptionData(name, description, false, true);
        }

        private string SetOptionData(string name, string description, bool isOptional, bool isIndexed, string defaultValue = null)
        {
            defaultValue = defaultValue ?? string.Empty;
            _data.Set(name, new ArgumentInfo
            {
                Name = name,
                Description = description,
                Default = defaultValue,
                IsOptional = isOptional,
                IsIndexed = isIndexed,
                IsFlag = false,
            });
            return defaultValue;
        }
    }
}
