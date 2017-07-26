using System;

namespace QTFK.Models.DBIO
{
    public class OleDBEqualsToParamFilter : IEqualsToParamQueryFilter
    {
        private string _field;
        private string _paramName;

        public OleDBEqualsToParamFilter(string field, string paramName)
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