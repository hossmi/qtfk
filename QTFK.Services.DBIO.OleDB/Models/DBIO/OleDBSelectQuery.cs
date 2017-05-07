using QTFK.Extensions.Collections.Dictionaries;
using QTFK.Extensions.Collections.Strings;
using System;
using System.Collections.Generic;

namespace QTFK.Models.DBIO
{
    public class OleDBSelectQuery : IDBQuery, IDBQueryWithTableName, IDBQuerySelectColumns, IDBQueryWhereClause
    {
        public ICollection<SelectColumn> Columns { get; set; } = new List<SelectColumn>();
        public string Table { get; set; } = "";
        public string Where { get; set; } = "";
        public IDictionary<string, object> Parameters { get; set; } = DictionaryExtension.New();

        public string Compile()
        {
            string whereSegment = string.IsNullOrWhiteSpace(Where) ? "" : $"WHERE ({Where})";
            string columns = Columns
                .Stringify(f =>
                {
                    if (f.Name.Trim() == "*")
                        return f.Name;

                    return $"[{f.Name}] {(string.IsNullOrWhiteSpace(f.Alias) ? "" : $" AS [{f.Alias}]")}";
                });

            return $@"
                SELECT {columns}
                FROM [{Table}]
                {whereSegment}
                ;";
        }
    }
}