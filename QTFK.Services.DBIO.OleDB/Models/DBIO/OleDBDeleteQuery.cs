using QTFK.Attributes;
using QTFK.Extensions.Collections.Dictionaries;
using QTFK.Extensions.Collections.Strings;
using QTFK.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QTFK.Models.DBIO
{
    [OleDB]
    public class OleDBDeleteQuery : IDBQueryDelete
    {
        public string Prefix { get; set; } = "";
        public string Table { get; set; } = "";
        public IDictionary<string, object> Parameters { get; } = DictionaryExtension.New();
        public IQueryFilter Filter { get; set; }

        public string Compile()
        {
            Asserts.isFilled(this.Table, $"Property '{nameof(this.Table)}' cannot be empty.");

            string prefix = string.IsNullOrWhiteSpace(Prefix) ? "" : Prefix.Trim();
            string whereSegment = (Filter ?? NullQueryFilter.Instance).Compile();
            whereSegment = string.IsNullOrWhiteSpace(whereSegment) ? "" : $"WHERE ({whereSegment})";

            return $@"
                DELETE FROM {prefix}[{Table}]
                {whereSegment}
                ;";
        }
    }
}