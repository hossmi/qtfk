using QTFK.Extensions.Collections.Strings;
using System;
using System.Collections.Generic;

namespace QTFK.Models.DBIO
{
    public class SqlSelectQuery : IDBQuery, IDBQueryWithTableName, IDBQueryWithFields, IDBQueryWithWhere, IDBQueryWithTablePrefix
    {
        public string Prefix { get; set; } = "";
        public ICollection<string> Fields { get; set; } = new List<string>();
        public string Table { get; set; } = "";
        public string Where { get; set; } = "";

        public string Compile()
        {
            string prefix = string.IsNullOrWhiteSpace(Prefix) ? "" : Prefix.Trim();
            string whereSegment = string.IsNullOrWhiteSpace(Where) ? "" : $"WHERE ({Where})";
            string columns = Fields.Stringify(f => f.Trim() == "*" ? f : $"[{f}]");

            return $@"
                SELECT {columns}
                FROM {prefix}[{Table}]
                {whereSegment}
                ;";
        }
    }
}