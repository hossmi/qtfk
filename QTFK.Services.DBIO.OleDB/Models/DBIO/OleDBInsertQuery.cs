using QTFK.Extensions.Collections.Strings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QTFK.Models.DBIO
{
    public class OleDBInsertQuery : IDBQuery, IDBQueryWithTableName, IDBQueryWithValuedFields
    {
        public OleDBInsertQuery()
        {
            Table = "";
            ValuedFields = new Dictionary<string, object>();
        }

        public string Table { get; set; }
        public IDictionary<string, object> ValuedFields { get; set; }

        public string Compile()
        {
            var columnList = ValuedFields
                .Select(c => new { K = c.Key, V = c.Value.ToString() })
                .ToList()
                ;

            return $@"
                INSERT INTO [{Table}] ({columnList.Stringify(c => $"[{c.K}]")})
                VALUES ({columnList.Stringify(c => c.V.StartsWith("@") ? c.V : $"'{c.V}'")})
                ;";
        }
    }
}