using System.Collections.Generic;
using System.Linq;
using QTFK.Services;

namespace QTFK.Models.DBIO
{
    public abstract class AbstractDeleteQuery : IDBQueryDelete
    {
        private readonly IParameterBuilderFactory parameterBuilderFactory;
        protected string prefix;
        protected string table;
        protected IQueryFilter filter;

        public AbstractDeleteQuery(IParameterBuilderFactory parameterBuilderFactory)
        {
            this.prefix = "";
            this.table = "";
            this.filter = NullQueryFilter.Instance;
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

        public virtual QueryCompilation Compile()
        {
            QueryCompilation result;
            string whereSegment, query;
            FilterCompilation filterCompilation;
            IParameterBuilder parameterBuilder;

            Asserts.stringIsNotEmpty(this.Table);

            parameterBuilder = this.parameterBuilderFactory.buildInstance();
            filterCompilation = this.filter.Compile(parameterBuilder);

            whereSegment = filterCompilation.FilterSegment;
            whereSegment = string.IsNullOrWhiteSpace(whereSegment) ? "" : $"WHERE ({whereSegment})";

            query = $@"
                DELETE FROM {this.prefix}[{this.table}]
                {whereSegment}
                ;";

            result = new QueryCompilation(query, filterCompilation.Parameters);

            return result;
        }
    }
}