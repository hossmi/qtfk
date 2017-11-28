namespace QTFK.Models
{
    public class ArgumentInfo
    {
        public string Name { get; internal set; }
        public string Description { get; internal set; }
        public string Default { get; internal set; }
        public bool IsOptional { get; internal set; }
        public bool IsIndexed { get; internal set; }
        public bool IsFlag { get; internal set; }
    }
}