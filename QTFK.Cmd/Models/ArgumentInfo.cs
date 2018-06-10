namespace QTFK.Models
{
    public class ArgumentInfo
    {
        private ArgumentInfo(string name, string description, string defaultValue, bool isOptional, bool isIndexed, bool isFlag)
        {
            this.Name = name;
            this.Description = description;
            this.Default = defaultValue;
            this.IsOptional = isOptional;
            this.IsIndexed = isIndexed;
            this.IsFlag = isFlag;
        }

        public static ArgumentInfo createHelp(string name, string description)
        {
            return new ArgumentInfo(name, description, "", true, false, true);
        }

        public static ArgumentInfo createDefault(string name, string description, string defaultValue, bool isOptional, bool isIndexed, bool isFlag)
        {
            return new ArgumentInfo(name, description, defaultValue, isOptional, isIndexed, isFlag);
        }

        public string Name { get; }
        public string Description { get; }
        public string Default { get; }
        public bool IsOptional { get; }
        public bool IsIndexed { get; }
        public bool IsFlag { get; }
    }
}