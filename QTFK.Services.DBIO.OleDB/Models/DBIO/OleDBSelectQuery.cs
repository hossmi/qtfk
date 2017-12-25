using QTFK.Attributes;
using QTFK.Extensions.Collections.Strings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QTFK.Models.DBIO
{
    [OleDB]
    public class OleDBSelectQuery : IDBQuerySelect
    {
        private string prefix;
        private string table;
        private IQueryFilter filter;
        private IList<SelectColumn> columns;


        public OleDBSelectQuery()
        {
            this.prefix = "";
            this.table = "";
            this.columns = new List<SelectColumn>();
            this.Joins = new List<JoinTable>();
            this.filter = NullQueryFilter.Instance;
        }

        public string Prefix
        {
            get
            {
                return this.prefix;
            }
            set
            {
                Asserts.isSomething(value, $"Value for property {nameof(Prefix)} cannot be null.");
                this.prefix = value;
            }
        }

        public string Table
        {
            get
            {
                return this.table;
            }
            set
            {
                Asserts.isSomething(value, $"Value for property {nameof(Table)} cannot be null.");
                this.table = value.Trim();
            }
        }

        public ICollection<JoinTable> Joins { get; }

        public IQueryFilter Filter
        {
            get
            {
                return this.filter;
            }
            set
            {
                this.filter = value ?? NullQueryFilter.Instance;
            }
        }

        public void addColumn(SelectColumn column)
        {
            this.columns.Add(column);
        }

        public IEnumerable<SelectColumn> getColumns()
        {
            return this.columns;
        }

        public IDictionary<string, object> getParameters()
        {
            return this.filter.Parameters;
        }

        public string Compile()
        {
            string compiledResult;
            string whereSegment, columnsSegment, joinsSegment, mainTable;
            int tableIndex;
            IList<IEnumerable<string>> allColumns;
            IEnumerable<string> tableColumns;

            Asserts.isFilled(this.table, $"Property '{nameof(this.Table)}' cannot be empty.");

            whereSegment = this.filter.Compile();
            whereSegment = string.IsNullOrWhiteSpace(whereSegment) ? "" : $"WHERE ({whereSegment})";

            tableIndex = 1;
            mainTable = "t0";
            allColumns = new List<IEnumerable<string>>();
            tableColumns = prv_prepareColumns(mainTable, this.columns);
            allColumns.Add(tableColumns);

            joinsSegment = Joins
                .Stringify(join =>
                {
                    string joinResult;
                    string alias, matches;

                    alias = $"t{tableIndex++}";
                    tableColumns = prv_prepareColumns(alias, join.Columns);
                    allColumns.Add(tableColumns);

                    matches = join.Matches
                        .Stringify(match => $"{mainTable}.[{match.LeftField}] = {alias}.[{match.RightField}]", " AND ");

                    joinResult = $"{join.Kind} JOIN {this.prefix}[{join.Table}] AS {alias} ON {matches} ";

                    return joinResult;
                }, Environment.NewLine);

            Asserts.check(allColumns.Any(), $"Query has no columns.");

            columnsSegment = allColumns
                .SelectMany(c => c)
                .Stringify()
                ;

            compiledResult = $@"
                SELECT {columnsSegment}
                FROM {this.prefix}[{Table}] as {mainTable}
                {joinsSegment}
                {whereSegment}
                ;";

            return compiledResult;
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