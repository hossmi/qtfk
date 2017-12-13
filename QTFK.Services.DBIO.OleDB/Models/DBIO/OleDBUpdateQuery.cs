using QTFK.Attributes;
using QTFK.Extensions.Collections.Dictionaries;
using QTFK.Extensions.Collections.Strings;
using QTFK.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QTFK.Models.DBIO
{
    [OleDB]
    public class OleDBUpdateQuery : IDBQueryUpdate
    {
        public string Prefix { get; set; } = "";
        public string Table { get; set; } = "";
        public IDictionary<string, object> Fields { get; set; } = DictionaryExtension.New();
        public IDictionary<string, object> Parameters { get; set; } = DictionaryExtension.New();
        public IQueryFilter Filter { get ; set ; }

        public string Compile()
        {
            string prefix = string.IsNullOrWhiteSpace(Prefix) ? "" : Prefix.Trim();
            var columnList = Fields
                .Select(c => new { K = c.Key, V = c.Value.ToString() })
                .ToList()
                ;

            string whereSegment = (Filter ?? NullQueryFilter.Instance).Compile();
            whereSegment = string.IsNullOrWhiteSpace(whereSegment) ? "" : $"WHERE ({whereSegment})";

            return $@"
                UPDATE {prefix}[{Table}]
                SET {columnList.Stringify(c => $"[{c.K}] = {(c.V.StartsWith("@") ? c.V : $"'{c.V}'")}")}
                {whereSegment}
                ;";
        }
    }
}