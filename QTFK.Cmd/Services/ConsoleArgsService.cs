using System;
using System.Collections.Generic;
using QTFK.Extensions.Collections.Dictionaries;
using System.Linq;
using QTFK.Extensions.Collections.Filters;
using QTFK.Models;

namespace QTFK.Services
{
    public class ConsoleArgsService : IConsoleArgsService
    {
        public bool CaseSensitive { get; set; }
        public string Prefix { get; set; }
        public Action<ArgumentException> OnError { get; set; }

        public T Parse<T>(IEnumerable<string> args, Func<IConsoleArgsBuilder, T> builder) where T: class
        {
            if (string.IsNullOrWhiteSpace(Prefix))
                throw new ArgumentNullException(nameof(Prefix), "It is mandatory the use of a prefix for options.");

            IConsoleArgsBuilder argsBuilder = new ConsoleArgsBuilder(
                prefix: Prefix, 
                caseSensitive: CaseSensitive, 
                onError: OnError,
                args: args
                );
            var result = new Result<T>(() => builder(argsBuilder));

            if (result.Ok)
                return result.Value;

            OnError(new ArgumentException($"Un expected error '{result.Exception.Message}'", result.Exception));
            return null;
        }

        private class ConsoleArgsBuilder : IConsoleArgsBuilder
        {
            private string _prefix;
            private bool _caseSensitive;
            private Action<ArgumentException> _onError;
            private string[] _args;

            public ConsoleArgsBuilder(string prefix, bool caseSensitive, Action<ArgumentException> onError, IEnumerable<string> args)
            {
                _prefix = prefix;
                _caseSensitive = caseSensitive;
                _onError = onError;
                _args = args.ToArray();
            }

            public string Get(string name)
            {
                string result = FindNamed(name, 0);

                if (result == null)
                    _onError(new ArgumentException($"Missing value for option '{name}'"));

                return result;
            }

            public string Get<T>(string name, T defaultValue)
            {
                return FindNamed(name, 0) ?? defaultValue.ToString();
            }

            public string Get(int index)
            {
                string result = FindUnnamed(index, 0, 1);

                if (result == null)
                    _onError(new ArgumentException($"Missing {index}{(index == 1 ? "st" : "")}{(index == 2 ? "nd" : "")}{(index > 2 ? "th" : "")} argument."));

                return result;
            }

            public string Get<T>(int index, T defaultValue)
            {
                return FindUnnamed(index, 0, 1) ?? defaultValue.ToString();
            }

            public bool HasFlag(string name)
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
                return arg.StartsWith(_prefix);
            }

            private bool IsOption(string arg, string name)
            {
                return $"{_prefix}{name}" == arg;
            }
        }
    }
}