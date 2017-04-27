using QTFK.Extensions.Collections.Strings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QTFK.Models.DBIO
{
    public class OleDBUpdateQuery : IDBQuery, IDBQueryWithTableName, IDBQueryWithValuedFields, IDBQueryWithWhere
    {
        public OleDBUpdateQuery()
        {
            Table = "";
            ValuedFields = new Dictionary<string, object>();
        }

        public string Table { get; set; }
        public IDictionary<string, object> ValuedFields { get; set; }
        public string Where { get; set; }

        public string Compile()
        {
            var columnList = ValuedFields
                .Select(c => new { K = c.Key, V = c.Value.ToString() })
                .ToList()
                ;

            string whereSegment = string.IsNullOrWhiteSpace(Where) ? "" : $"WHERE {Where}";

            return $@"
                UPDATE {Table}
                SET {columnList.Stringify(c => $"{c.K} = {(c.V.StartsWith("@") ? c.V : $"'{c.V}'")}")}
                {whereSegment}
                ;";
        }
    }
}