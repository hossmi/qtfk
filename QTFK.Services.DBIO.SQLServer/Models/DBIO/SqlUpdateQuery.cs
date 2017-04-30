using QTFK.Extensions.Collections.Strings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QTFK.Models.DBIO
{
    public class SqlUpdateQuery : IDBQuery, IDBQueryWithTableName, IDBQueryWithValuedFields, IDBQueryWithWhere, IDBQueryWithTablePrefix
    {
        public string Prefix { get; set; } = "";
        public string Table { get; set; } = "";
        public IDictionary<string, object> ValuedFields { get; set; } = new Dictionary<string, object>();
        public string Where { get; set; } = "";

        public string Compile()
        {
            string whereSegment = string.IsNullOrWhiteSpace(Where) ? "" : $"WHERE ({Where})";
            string prefix = string.IsNullOrWhiteSpace(Prefix) ? "" : Prefix.Trim();
            string fieldValueList = ValuedFields.Stringify(c => $"[{c.Key}] = ({c.Value})");

            return $@"
                UPDATE {prefix}[{Table}]
                SET {fieldValueList}
                {whereSegment}
                ;";
        }
    }
}