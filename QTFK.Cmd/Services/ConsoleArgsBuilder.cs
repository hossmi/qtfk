using QTFK.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QTFK.Services
{
    internal class ConsoleArgsBuilder : IConsoleArgsBuilder
    {
        private string[] _args;
        private readonly IConsoleArgsService _service;
        private readonly Action<ArgumentException> _onError;
        private readonly IDictionary<string, ArgumentInfo> _argsInfo;

        public ConsoleArgsBuilder(
            IConsoleArgsService service
            , Action<ArgumentException> onError
            , IEnumerable<string> args
            , IDictionary<string, ArgumentInfo> argsInfo
            )
        {
            _args = args.ToArray();
            _service = service;
            _onError = onError;
            _argsInfo = argsInfo;
        }

        public string Required(string name, string description)
        {
            string result = FindNamed(name, 0);

            if (result == null)
                _onError(new ArgumentException($"Missing value for option '{name}'"));

            return result;
        }

        public string Optional(string name, string description, string defaultValue)
        {
            return FindNamed(name, 0) ?? defaultValue;
        }

        public string Required(int index, string name, string description)
        {
            string result = FindUnnamed(index, 0, 1);

            if (result == null)
                _onError(new ArgumentException($"Missing {index}{(index == 1 ? "st" : "")}{(index == 2 ? "nd" : "")}{(index > 2 ? "th" : "")} argument."));

            return result;
        }

        public string Optional(int index, string name, string description, string defaultValue)
        {
            return FindUnnamed(index, 0, 1) ?? defaultValue;
        }

        public bool Flag(string name, string description)
        {
            return FindFlag(name, 0);
        }

        private bool FindFlag(string name, int i)
        {
            if (i >= _args.Length)
                return false;

            if (IsOption(_args[i], name))
                return true;

            return FindFlag(name, i + 1);
        }

        private string FindNamed(string name, int i)
        {
            if (i >= _args.Length)
                return null;

            if(IsOption(_args[i], name))
            {
                if (i+1 >= _args.Length)
                    return null;

                if(IsOption(_args[i+1]))
                    return null;

                return _args[i + 1];
            }
            else
            {
                return FindNamed(name, i + 1);
            }
        }

        private string FindUnnamed(int index, int i, int currentIndex)
        {
            if(i >= _args.Length)
                return null;

            if (IsOption(_args[i]))
            {
                if (i + 1 >= _args.Length)
                    return null;

                string optionName = _args[i].Remove(0, _service.Prefix.Length);
                if (_argsInfo[optionName].IsFlag)
                    return FindUnnamed(index, i + 1, currentIndex);

                if (IsOption(_args[i + 1]))
                    return FindUnnamed(index, i + 1, currentIndex);
                else
                    return FindUnnamed(index, i + 2, currentIndex);
            }
            else
            {
                if (index == currentIndex)
                    return _args[i];
                else
                {
                    return FindUnnamed(index, i + 1, currentIndex + 1);
                }
            }
        }

        private bool IsOption(string arg)
        {
            return arg.StartsWith(_service.Prefix);
        }

        private bool IsOption(string arg, string name)
        {
            return $"{_service.Prefix}{name}" == arg;
        }
    }
}