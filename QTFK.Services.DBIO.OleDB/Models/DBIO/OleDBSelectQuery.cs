using QTFK.Extensions.Collections.Dictionaries;
using QTFK.Extensions.Collections.Strings;
using QTFK.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QTFK.Models.DBIO
{
    public class OleDBSelectQuery : IDBQuerySelect, IOleDB
    {
        public string Prefix { get; set; } = "";
        public ICollection<SelectColumn> Columns { get; set; } = new List<SelectColumn>();
        public string Table { get; set; } = "";
        public IDictionary<string, object> Parameters { get; set; } = DictionaryExtension.New();
        public ICollection<JoinTable> Joins { get; set; } = new List<JoinTable>();
        public IQueryFilter Filter { get ; set ; }

        IEnumerable<string> PrepareColumns(string table, IEnumerable<SelectColumn> columns)
        {
            return columns
                .Select(c =>
                {
                    if (c.Name.Trim() == "*")
                        return $"{table}.*";

                    return $"{table}.[{c.Name}] {(string.IsNullOrWhiteSpace(c.Alias) ? "" : $" AS [{c.Alias}]")}";
                });
        }

        public string Compile()
        {
            string prefix = string.IsNullOrWhiteSpace(Prefix) ? "" : Prefix.Trim();
            string whereSegment = (Filter ?? EmptyQueryFilter.Instance).Compile();
            whereSegment = string.IsNullOrWhiteSpace(whereSegment) ? "" : $"WHERE ({whereSegment})";

            int n = 1;
            string t0 = "t0";
            IList<IEnumerable<string>> allColumns = new List<IEnumerable<string>>();

            allColumns.Add(PrepareColumns(t0, Columns));

            string joins = Joins
                .Stringify(join =>
                {
                    string alias = $"t{n++}";
                    allColumns.Add(PrepareColumns(alias, join.Columns));

                    string matches = join.Matches
                        .Stringify(match => $"{t0}.[{match.LeftField}] = {alias}.[{match.RightField}]", " AND ");

                    return $"{join.Kind} JOIN {prefix}[{join.Table}] AS {alias} ON {matches} ";
                }, Environment.NewLine);


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
    }
}