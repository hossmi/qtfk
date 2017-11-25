using QTFK.Extensions.Collections.Dictionaries;
using QTFK.Models;
using System.Collections.Generic;

namespace QTFK.Services
{
    internal class ExplorerConsoleArgsBuilder : IConsoleArgsBuilder
    {
        private readonly IDictionary<string, ArgumentInfo> data;

        public ExplorerConsoleArgsBuilder(IDictionary<string, ArgumentInfo> data)
        {
            Asserts.isSomething(data, $"'{nameof(data)}' cannot be null.");

            this.data = data;
        }

        public event ArgsErrorDelegate Error;

        public bool Flag(string name, string description)
        {
            prv_setOptionData(this.data, name, description, true, false, false.ToString(), true);
            return false;
        }

        public string Optional(string name, string description, string defaultValue)
        {
            return prv_setOptionData(this.data, name, description, true, false, defaultValue, false);
        }

        public string Optional(int index, string name, string description, string defaultValue)
        {
            return prv_setOptionData(this.data, name, description, true, true, defaultValue, false);
        }

        public string Required(string name, string description)
        {
            return prv_setOptionData(this.data, name, description, false, false, string.Empty, false);
        }

        public string Required(int index, string name, string description)
        {
            return prv_setOptionData(this.data, name, description, false, true, string.Empty, false);
        }

        private static string prv_setOptionData(IDictionary<string, ArgumentInfo> data
            , string name, string description
            , bool isOptional, bool isIndexed
            , string defaultValue
            , bool isFlag
            )
        {
            ArgumentInfo argumentInfo;

            argumentInfo = new ArgumentInfo
            {
                Name = name,
                Description = description,
                Default = defaultValue,
                IsOptional = isOptional,
                IsIndexed = isIndexed,
                IsFlag = isFlag,
            };
            data.Set(name, argumentInfo);

            return defaultValue;
        }
    }
}
