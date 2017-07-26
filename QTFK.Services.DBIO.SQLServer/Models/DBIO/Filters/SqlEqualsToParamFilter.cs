using System;

namespace QTFK.Models.DBIO.Filters
{
    public class SqlEqualsToParamFilter : IEqualsToParamQueryFilter
    {
        private readonly string _field;
        private readonly string _paramName;

        public SqlEqualsToParamFilter(string field, string paramName)
        {
            _field = field;
            _paramName = paramName;
        }

        public string Compile()
        {
            return $"{_field} = {_paramName}";
        }
    }
}