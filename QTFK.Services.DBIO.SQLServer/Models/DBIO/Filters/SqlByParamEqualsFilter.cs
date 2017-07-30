using System;

namespace QTFK.Models.DBIO.Filters
{
    public class SqlByParamEqualsFilter : IByParamEqualsFilter
    {
        public string Field { get ; set ; }
        public string Parameter { get ; set ; }

        public string Compile()
        {
            return $"{Field} = {Parameter}";
        }

        public void SetValues(params object[] args)
        {
            if (args.Length < 1)
                throw new ArgumentException("Missing argument 1", nameof(Parameter));

            Parameter = args[0].ToString();
        }
    }
}