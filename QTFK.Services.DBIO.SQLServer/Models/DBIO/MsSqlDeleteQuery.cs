using QTFK.Extensions.Collections.Strings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QTFK.Models.DBIO
{
    public class MsSqlDeleteQuery : IDBQuery, IDBQueryWithTableName, IDBQueryWithWhere
    {
        public MsSqlDeleteQuery()
        {
            Table = "";
            Where = "";
        }

        public string Table { get; set; }
        public string Where { get; set; }

        public string Compile()
        {
            string whereSegment = string.IsNullOrWhiteSpace(Where) ? "" : $"WHERE {Where}";

            return $@"
                DELETE FROM [{Table}]
                {whereSegment}
                ;";
        }
    }
}