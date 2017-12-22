using QTFK.Extensions.Collections.Dictionaries;
using QTFK.Extensions.Collections.Filters;
using QTFK.Extensions.Collections.Strings;
using System;
using System.Linq;
using System.Collections.Generic;
using QTFK.Services;
using QTFK.Attributes;

namespace QTFK.Models.DBIO
{
    [SqlServer]
    public class SqlSelectQuery : IDBQuerySelect
    {
        public string Prefix { get; set; } = "";
        public string Table { get; set; } = "";
        public IDictionary<string, object> Parameters { get; set; } = DictionaryExtension.New();
        public ICollection<SelectColumn> Columns { get; set; } = new List<SelectColumn>();
        public ICollection<JoinTable> Joins { get; set; } = new List<JoinTable>();
        public IQueryFilter Filter { get; set; }

        public string Compile()
        {
            Asserts.isFilled(this.Table, $"Property '{nameof(this.Table)}' cannot be empty.");

            string prefix = string.IsNullOrWhiteSpace(Prefix) ? "" : Prefix.Trim();
            string whereSegment = (Filter ?? NullQueryFilter.Instance).Compile();
            whereSegment = string.IsNullOrWhiteSpace(whereSegment) ? "" : $"WHERE ({whereSegment})";
            int n = 1;
            string t0 = "t0";
            IList<IEnumerable<string>> allColumns = new List<IEnumerable<string>>();

            allColumns.Add(prv_prepareColumns(t0, Columns));

            string joins = Joins
                .Stringify(join =>
                {
                    string alias = $"t{n++}";
                    allColumns.Add(prv_prepareColumns(alias, join.Columns));

                    string matches = join.Matches
                        .Stringify(match => $"{t0}.[{match.LeftField}] = {alias}.[{match.RightField}]", " AND ");

                    return $"{join.Kind} JOIN {prefix}[{join.Table}] AS {alias} ON {matches} ";
                }, Environment.NewLine);

            Asserts.check(allColumns.Any(), $"Query has no columns.");

            string columns = allColumns
                .SelectMany(c => c)
                .Stringify()
                ;

            return $@"
                SELECT {columns}
                FROM {prefix}[{Table}] as {t0}
                {joins}
                {whereSegment}
                ;";
        }

        private static IEnumerable<string> prv_prepareColumns(string table, IEnumerable<SelectColumn> columns)
        {
            return columns
                .Select(c =>
                {
                    if (c.Name.Trim() == "*")
                        return $"{table}.*";

                    return $"{table}.[{c.Name}] {(string.IsNullOrWhiteSpace(c.Alias) ? "" : $" AS [{c.Alias}]")}";
                });
        }
    }
}