using QTFK.Extensions.Collections.Dictionaries;
using QTFK.Extensions.Collections.Strings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QTFK.Models.DBIO
{
    public class OleDBUpdateQuery : IDBQueryUpdate
    {
        public string Prefix { get; set; } = "";
        public string Table { get; set; } = "";
        public IDictionary<string, object> Fields { get; set; } = DictionaryExtension.New();
        public string Where { get; set; } = "";
        public IDictionary<string, object> Parameters { get; set; } = DictionaryExtension.New();

        public string Compile()
        {
            string prefix = string.IsNullOrWhiteSpace(Prefix) ? "" : Prefix.Trim();
            var columnList = Fields
                .Select(c => new { K = c.Key, V = c.Value.ToString() })
                .ToList()
                ;

            string whereSegment = string.IsNullOrWhiteSpace(Where) ? "" : $"WHERE ({Where})";

            return $@"
                UPDATE {prefix}[{Table}]
                SET {columnList.Stringify(c => $"[{c.K}] = {(c.V.StartsWith("@") ? c.V : $"'{c.V}'")}")}
                {whereSegment}
                ;";
        }
    }
}