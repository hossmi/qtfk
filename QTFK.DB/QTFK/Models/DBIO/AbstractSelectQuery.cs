using QTFK.Extensions.Collections.Strings;
using QTFK.Services;
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
        private readonly IParameterBuilderFactory parameterBuilderFactory;

        public AbstractSelectQuery(IParameterBuilderFactory parameterBuilderFactory)
        {
            this.prefix = "";
            this.table = "";
            this.filter = NullQueryFilter.Instance;
            this.columns = new List<SelectColumn>();
            this.parameterBuilderFactory = parameterBuilderFactory;
        }

        public string Prefix
        {
            get
            {
                return this.prefix;
            }
            set
            {
                Asserts.isNotNull(value);
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
                Asserts.isNotNull(value);
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

        public void addColumn(SelectColumn column)
        {
            this.columns.Add(column);
        }

        public IEnumerable<SelectColumn> getColumns()
        {
            return this.columns;
        }

        public virtual QueryCompilation Compile()
        {
            QueryCompilation result;
            string query;
            string whereSegment, columnsSegment, mainTable;
            IList<IEnumerable<string>> allColumns;
            IEnumerable<string> tableColumns;
            FilterCompilation filterCompilation;
            IParameterBuilder parameterBuilder;

            Asserts.stringIsNotEmpty(this.table);

            parameterBuilder = this.parameterBuilderFactory.buildInstance();
            filterCompilation = this.filter.Compile(parameterBuilder);
            whereSegment = filterCompilation.FilterSegment;
            whereSegment = string.IsNullOrWhiteSpace(whereSegment) ? "" : $"WHERE ({whereSegment})";

            mainTable = "t0";
            allColumns = new List<IEnumerable<string>>();
            tableColumns = prv_prepareColumns(mainTable, this.columns);
            allColumns.Add(tableColumns);

            Asserts.isTrue(allColumns.Any());

            columnsSegment = allColumns
                .SelectMany(c => c)
                .Stringify()
                ;

            query = $@"
                SELECT {columnsSegment}
                FROM {this.prefix}[{Table}] as {mainTable}
                {whereSegment}
                ;";

            result = new QueryCompilation(query, filterCompilation.Parameters);

            return result;
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