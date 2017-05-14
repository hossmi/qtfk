using QTFK.Extensions.Collections.Dictionaries;
using QTFK.Extensions.Collections.Strings;
using QTFK.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QTFK.Models.DBIO
{
    public class SqlDeleteQuery : IDBQueryDelete, ISQLServer
    {
        public IDictionary<string, object> Parameters { get; set; } = DictionaryExtension.New();
        public string Prefix { get; set; } = "";
        public string Table { get; set; } = "";
        public string Where { get; set; } = "";

        public string Compile()
        {
            string prefix = string.IsNullOrWhiteSpace(Prefix) ? "" : Prefix.Trim();
            string whereSegment = string.IsNullOrWhiteSpace(Where) ? "" : $"WHERE ({Where})";

            return $@"
                DELETE FROM {prefix}[{Table}]
                {whereSegment}
                ;";
        }
    }
}