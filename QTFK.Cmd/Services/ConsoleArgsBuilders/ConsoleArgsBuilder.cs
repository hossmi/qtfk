using QTFK.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QTFK.Services.ConsoleArgsBuilders
{
    internal class ConsoleArgsBuilder : IConsoleArgsBuilder
    {
        private string[] args;
        private readonly IConsoleArgsService service;
        private readonly IDictionary<string, ArgumentInfo> argsInfo;
        private readonly IEqualityComparer<string> stringComparer;

        public event ArgsErrorDelegate Error;

        public ConsoleArgsBuilder(
            IConsoleArgsService service
            , IEnumerable<string> args
            , IDictionary<string, ArgumentInfo> argsInfo
            , IEqualityComparer<string> stringcomparer
            )
        {
            this.args = args.ToArray();
            this.service = service;
            this.argsInfo = argsInfo;
            this.stringComparer = stringcomparer;
        }

        public string Required(string name, string description)
        {
            string result = prv_findNamed(name, 0);

            if (result == null)
                Error?.Invoke(new Exception($"Missing value for '{name}'"));

            return result;
        }

        public string Optional(string name, string description, string defaultValue)
        {
            return prv_findNamed(name, 0) ?? defaultValue;
        }

        public string Required(int index, string name, string description)
        {
            string result = prv_findUnnamed(index, 0, 1);

            if (result == null)
                Error?.Invoke(new Exception($"Missing value for {index}{(index==1 ? "st" : index == 2 ? "nd" : "th")} argument '{name}'."));

            return result;
        }

        public string Optional(int index, string name, string description, string defaultValue)
        {
            return prv_findUnnamed(index, 0, 1) ?? defaultValue;
        }

        public bool Flag(string name, string description)
        {
            return prv_findFlag(name, 0);
        }

        private bool prv_findFlag(string name, int i)
        {
            if (i >= this.args.Length)
                return false;

            if (prv_isOption(this.args[i], name))
                return true;

            return prv_findFlag(name, i + 1);
        }

        private string prv_findNamed(string name, int i)
        {
            if (i >= this.args.Length)
                return null;

            if(prv_isOption(this.args[i], name))
            {
                if (i+1 >= this.args.Length)
                    return null;

                if(prv_isOption(this.args[i+1]))
                    return null;

                return this.args[i + 1];
            }
            else
            {
                return prv_findNamed(name, i + 1);
            }
        }

        private string prv_findUnnamed(int index, int i, int currentIndex)
        {
            if(i >= this.args.Length)
                return null;

            if (prv_isOption(this.args[i]))
            {
                if (i + 1 >= this.args.Length)
                    return null;

                string optionName = this.args[i].Remove(0, this.service.Prefix.Length);
                if (this.argsInfo[optionName].IsFlag)
                    return prv_findUnnamed(index, i + 1, currentIndex);

                if (prv_isOption(this.args[i + 1]))
                    return prv_findUnnamed(index, i + 1, currentIndex);
                else
                    return prv_findUnnamed(index, i + 2, currentIndex);
            }
            else
            {
                if (index == currentIndex)
                    return this.args[i];
                else
                {
                    return prv_findUnnamed(index, i + 1, currentIndex + 1);
                }
            }
        }

        private bool prv_isOption(string arg)
        {
            return arg.StartsWith(this.service.Prefix);
        }

        private bool prv_isOption(string arg, string name)
        {
            return this.stringComparer.Equals($"{this.service.Prefix}{name}", arg);
        }
    }
}