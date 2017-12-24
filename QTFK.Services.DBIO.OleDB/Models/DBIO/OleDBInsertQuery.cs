using QTFK.Attributes;
using QTFK.Extensions.Collections.Dictionaries;
using QTFK.Extensions.Collections.Strings;
using System.Collections.Generic;
using System.Linq;

namespace QTFK.Models.DBIO
{
    [OleDB]
    public class OleDBInsertQuery : IDBQueryInsert
    {
        public string Prefix { get; set; } = "";
        public string Table { get; set; } = "";
        public IDictionary<string, object> Fields { get; } = DictionaryExtension.New();
        public IDictionary<string, object> Parameters { get; } = DictionaryExtension.New();

        public string Compile()
        {
            Asserts.isFilled(this.Table, $"Property '{nameof(this.Table)}' cannot be empty.");

            string prefix = string.IsNullOrWhiteSpace(Prefix) ? "" : Prefix.Trim();
            var columnList = Fields
                .Select(c => new { K = c.Key, V = c.Value.ToString() })
                .ToList()
                ;

            return $@"
                INSERT INTO {prefix}[{Table}] ({columnList.Stringify(c => $"[{c.K}]")})
                VALUES ({columnList.Stringify(c => c.V.StartsWith("@") ? c.V : $"'{c.V}'")})
                ;";
        }
    }
}