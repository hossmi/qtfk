using QTFK.Extensions.Collections.Strings;
using System;
using System.Collections.Generic;

namespace QTFK.Models.DBIO
{
    public class MsSqlSelectQuery : IDBQuery, IDBQueryWithTableName, IDBQueryWithFields, IDBQueryWithWhere
    {
        public MsSqlSelectQuery()
        {
            Table = "";
            Where = "";
            Fields = new List<string>();
        }

        public ICollection<string> Fields { get; set; }
        public string Table { get; set; }
        public string Where { get; set; }

        public string Compile()
        {
            string whereSegment = string.IsNullOrWhiteSpace(Where) ? "" : $"WHERE {Where}";

            return $@"
                SELECT {Fields.Stringify(f => $"[{f}]")}
                FROM [{Table}]
                {whereSegment}
                ;";
        }
    }
}