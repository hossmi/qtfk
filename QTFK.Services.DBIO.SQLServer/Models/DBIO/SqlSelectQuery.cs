﻿using QTFK.Extensions.Collections.Dictionaries;
using QTFK.Extensions.Collections.Filters;
using QTFK.Extensions.Collections.Strings;
using System;
using System.Linq;
using System.Collections.Generic;

namespace QTFK.Models.DBIO
{
    public class SqlSelectQuery : IDBQuery, IDBQueryWithTableName, IDBQuerySelectColumns, IDBQueryWhereClause, IDBQueryTablePrefix, IDBQueryJoin
    {
        public string Prefix { get; set; } = "";
        public string Table { get; set; } = "";
        public string Where { get; set; } = "";
        public IDictionary<string, object> Parameters { get; set; } = DictionaryExtension.New();
        public ICollection<SelectColumn> Columns { get; set; } = new List<SelectColumn>();
        public ICollection<JoinTable> Joins { get; set; } = new List<JoinTable>();

        IEnumerable<string> PrepareColumns(string table, IEnumerable<SelectColumn> columns)
        {
            return columns
                .Select(c =>
                {
                    if (c.Name.Trim() == "*")
                        return $"[{table}].*";

                    return $"[{table}].[{c.Name}] {(string.IsNullOrWhiteSpace(c.Alias) ? "" : $" AS [{c.Alias}]")}";
                });
        }

        public string Compile()
        {
            string prefix = string.IsNullOrWhiteSpace(Prefix) ? "" : Prefix.Trim();
            string whereSegment = string.IsNullOrWhiteSpace(Where) ? "" : $"WHERE ({Where})";
            int n = 1;
            string t0 = "t0";
            IList<IEnumerable<string>> allColumns = new List<IEnumerable<string>>();

            string joins = Joins
                .Stringify(join =>
                {
                    string alias = $"t{n++}";
                    allColumns.Add(PrepareColumns(alias, join.Columns));

                    string matches = join.Matches
                        .Stringify(match => $"{t0}.{match.LeftField} = {alias}.{match.RightField}", " AND ");

                    return $"{join.Kind} JOIN {prefix}{join.Table} AS {alias} ON {matches} ";
                }, Environment.NewLine);

            allColumns.Add(PrepareColumns(t0, Columns));

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