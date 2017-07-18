using QTFK.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QTFK.Services
{
    public partial class ConsoleArgsService : IConsoleArgsService
    {
        private class ConsoleArgsBuilder : IConsoleArgsBuilder
        {
            private string[] _args;
            private readonly IConsoleArgsService _service;
            private readonly Action<ArgumentException> _onError;
            private readonly Action<string> _onUsage;
            private readonly Action<ArgumentInfo> _onUsageOption;
            private readonly bool _helpMode;

            public ConsoleArgsBuilder(
                IConsoleArgsService service
                , Action<ArgumentException> onError
                , Action<string> onUsage
                , Action<ArgumentInfo> onUsageOption
                , IEnumerable<string> args
                )
            {
                _args = args.ToArray();
                _service = service;
                _onError = onError;
                _onUsage = onUsage;
                _onUsageOption = onUsageOption;

                _helpMode = FindFlag(_service.HelpOptionName, 0);

                if (_helpMode)
                {
                    _onUsage?.Invoke(_service.HelpDescription);
                    _onUsageOption?.Invoke(new ArgumentInfo { Name = _service.HelpOptionName, Description = _service.HelpDescription });
                }
            }

            public string ByName(string name, string description)
            {
                if (_helpMode)
                {
                    _onUsageOption?.Invoke(new ArgumentInfo
                    {
                        Name = name,
                        Description = description
                    });
                    return null;
                }

                string result = FindNamed(name, 0);

                if (result == null)
                    _onError(new ArgumentException($"Missing value for option '{name}'"));

                return result;
            }

            public string ByName(string name, string description, string defaultValue)
            {
                return FindNamed(name, 0) ?? defaultValue;
            }

            public string ByIndex(int index, string name, string description)
            {
                if (_helpMode)
                {
                    _onUsageOption?.Invoke(new ArgumentInfo
                    {
                        Name = name,
                        Description = description
                    });
                    return null;
                }

                string result = FindUnnamed(index, 0, 1);

                if (result == null)
                    _onError(new ArgumentException($"Missing {index}{(index == 1 ? "st" : "")}{(index == 2 ? "nd" : "")}{(index > 2 ? "th" : "")} argument."));

                return result;
            }

            public string ByIndex(int index, string name, string description, string defaultValue)
            {
                return FindUnnamed(index, 0, 1) ?? defaultValue;
            }

            public bool Flag(string name, string description)
            {
                if (_helpMode)
                {
                    _onUsageOption?.Invoke(new ArgumentInfo
                    {
                        Name = name,
                        Description = description
                    });
                    return false;
                }

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
}