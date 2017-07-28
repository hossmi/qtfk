using QTFK.Services;
using System;

namespace QTFK.Models.DBIO.Filters
{
    public class OleDBByParamEqualsFilter : IByParamEqualsFilter
    {
        public string Field { get; set; }
        public string Parameter { get; set; }

        public string Compile()
        {
            return $"{Field} = {Parameter}";
        }
    }
}