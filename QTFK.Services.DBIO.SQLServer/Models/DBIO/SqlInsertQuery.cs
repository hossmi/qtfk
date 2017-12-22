using QTFK.Attributes;
using QTFK.Extensions.Collections.Dictionaries;
using QTFK.Extensions.Collections.Strings;
using QTFK.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QTFK.Models.DBIO
{
    [SqlServer]
    public class SqlInsertQuery : IDBQueryInsert
    {
        public string Prefix { get; set; } = "";
        public string Table { get; set; } = "";
        public IDictionary<string, object> Fields { get; set; } = DictionaryExtension.New();
        public IDictionary<string, object> Parameters { get; set; } = DictionaryExtension.New();

        public string Compile()
        {
            Asserts.isFilled(this.Table, $"Property '{nameof(this.Table)}' cannot be empty.");

            string prefix = string.IsNullOrWhiteSpace(Prefix) ? "" : Prefix.Trim();

            return $@"
                INSERT INTO {prefix}[{Table}] ({Fields.Stringify(c => $"[{c.Key}]")})
                VALUES ({Fields.Stringify(c => $"{c.Value}")})
                ;";
        }
    }
}