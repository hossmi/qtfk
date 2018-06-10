using System;

namespace QTFK.Services
{
    public interface IConsoleArgsBuilder
    {
        string getRequired(string name, string description);
        string getOptional(string name, string description, string defaultValue);

        string getRequired(int index, string name, string description);
        string getOptional(int index, string name, string description, string defaultValue);

        bool getFlag(string name, string description);

        ArgsErrorDelegate ErrorFound { get; set; }
    }
}