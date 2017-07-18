namespace QTFK.Services
{
    public interface IConsoleArgsBuilder
    {
        string Required(string name, string description);
        string Optional(string name, string description, string defaultValue);

        string Required(int index, string name, string description);
        string Optional(int index, string name, string description, string defaultValue);

        bool Flag(string name, string description);
    }
}