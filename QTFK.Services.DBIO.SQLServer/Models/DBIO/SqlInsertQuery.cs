using QTFK.Extensions.Collections.Strings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QTFK.Models.DBIO
{
    public class SqlInsertQuery : IDBQuery, IDBQueryWithTableName, IDBQueryWithValuedFields, IDBQueryWithTablePrefix
    {
        public string Prefix { get; set; } = "";
        public string Table { get; set; } = "";
        public IDictionary<string, object> ValuedFields { get; set; } = new Dictionary<string, object>();

        public string Compile()
        {
            string prefix = string.IsNullOrWhiteSpace(Prefix) ? "" : Prefix.Trim();

            return $@"
                INSERT INTO {prefix}[{Table}] ({ValuedFields.Stringify(c => $"[{c.Key}]")})
                VALUES ({ValuedFields.Stringify(c => $"({c.Value})")})
                ;";
        }
    }
}