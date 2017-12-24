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
    public class SqlUpdateQuery : IDBQueryUpdate
    {
        public string Prefix { get; set; } = "";
        public string Table { get; set; } = "";
        public IDictionary<string, object> Fields { get; } = DictionaryExtension.New();
        public IDictionary<string, object> Parameters { get; } = DictionaryExtension.New();
        public IQueryFilter Filter { get; set; }

        public string Compile()
        {
            Asserts.isFilled(this.Table, $"Property '{nameof(this.Table)}' cannot be empty.");

            string whereSegment = (Filter ?? NullQueryFilter.Instance).Compile();
            whereSegment = string.IsNullOrWhiteSpace(whereSegment) ? "" : $"WHERE ({whereSegment})";
            string prefix = string.IsNullOrWhiteSpace(Prefix) ? "" : Prefix.Trim();
            string fieldValueList = Fields.Stringify(c => $"[{c.Key}] = {c.Value}");

            return $@"
                UPDATE {prefix}[{Table}] 
                SET {fieldValueList}
                {whereSegment}
                ;";
        }
    }
}