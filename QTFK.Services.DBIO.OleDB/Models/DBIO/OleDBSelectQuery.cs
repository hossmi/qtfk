using QTFK.Extensions.Collections.Strings;
using System;
using System.Collections.Generic;

namespace QTFK.Models.DBIO
{
    public class OleDBSelectQuery : IDBQuery
    {
        public IEnumerable<string> Columns { get; set; }
        public string Table { get; set; }
        public string Where { get; set; }

        public string Compile()
        {
            string whereSegment = string.IsNullOrWhiteSpace(Where) ? "" : $"WHERE {Where}";

            return $@"
                SELECT {Columns.Stringify()}
                FROM {Table}
                {whereSegment}
                ;";
        }
    }
}