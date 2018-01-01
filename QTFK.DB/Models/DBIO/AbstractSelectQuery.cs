﻿using QTFK.Extensions.Collections.Strings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QTFK.Models.DBIO
{
    public abstract class AbstractSelectQuery : IDBQuerySelect
    {
        private string prefix;
        private string table;
        private IQueryFilter filter;
        private IList<SelectColumn> columns;

        public AbstractSelectQuery()
        {
            this.prefix = "";
            this.table = "";
            this.filter = NullQueryFilter.Instance;
            this.columns = new List<SelectColumn>();
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

        public IEnumerable<QueryParameter> getParameters()
        {
            return this.filter.getParameters();
        }

        public void addColumn(SelectColumn column)
        {
            this.columns.Add(column);
        }

        public IEnumerable<SelectColumn> getColumns()
        {
            return this.columns;
        }

        public virtual string Compile()
        {
            string compiledResult;
            string whereSegment, columnsSegment, mainTable;
            IList<IEnumerable<string>> allColumns;
            IEnumerable<string> tableColumns;

            Asserts.isFilled(this.table, $"Property '{nameof(this.Table)}' cannot be empty.");

            whereSegment = this.filter.Compile();
            whereSegment = string.IsNullOrWhiteSpace(whereSegment) ? "" : $"WHERE ({whereSegment})";

            mainTable = "t0";
            allColumns = new List<IEnumerable<string>>();
            tableColumns = prv_prepareColumns(mainTable, this.columns);
            allColumns.Add(tableColumns);

            Asserts.check(allColumns.Any(), $"Query has no columns.");

            columnsSegment = allColumns
                .SelectMany(c => c)
                .Stringify()
                ;

            compiledResult = $@"
                SELECT {columnsSegment}
                FROM {this.prefix}[{Table}] as {mainTable}
                {whereSegment}
                ;";

            return compiledResult;
        }

        protected static IEnumerable<string> prv_prepareColumns(string table, IEnumerable<SelectColumn> columns)
        {
            return columns.Select(c =>
            {
                if (c.Name.Trim() == "*")
                    return $"{table}.*";

                return $"{table}.[{c.Name}] {(string.IsNullOrWhiteSpace(c.Alias) ? "" : $" AS [{c.Alias}]")}";
            });
        }
    }
}