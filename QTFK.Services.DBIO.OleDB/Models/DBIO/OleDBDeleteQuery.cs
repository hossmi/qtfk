using QTFK.Extensions.Collections.Dictionaries;
using QTFK.Extensions.Collections.Strings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QTFK.Models.DBIO
{
    public class OleDBDeleteQuery : IDBQuery, IDBQueryWithTableName, IDBQueryWhereClause
    {
        public string Table { get; set; } = "";
        public string Where { get; set; } = "";
        public IDictionary<string, object> Parameters { get; set; } = DictionaryExtension.New();

        public string Compile()
        {
            string whereSegment = string.IsNullOrWhiteSpace(Where) ? "" : $"WHERE ({Where})";

            return $@"
                DELETE FROM [{Table}]
                {whereSegment}
                ;";
        }
    }
}