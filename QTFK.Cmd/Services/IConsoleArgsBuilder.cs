namespace QTFK.Services
{
    public interface IConsoleArgsBuilder
    {
        string ByName(string name, string description);
        string ByName(string name, string description, string defaultValue);

        string ByIndex(int index, string name, string description);
        string ByIndex(int index, string name, string description, string defaultValue);

        bool Flag(string name, string description);
    }
}