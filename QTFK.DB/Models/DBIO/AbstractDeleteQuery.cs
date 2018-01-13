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

        public virtual QueryCompilation Compile()
        {
            QueryCompilation result;
            string whereSegment, query;
            FilterCompilation filterCompilation;
            IDictionary<string, object> parameters;
            IParameterBuilder parameterBuilder;

            Asserts.isFilled(this.Table, $"Property '{nameof(this.Table)}' cannot be empty.");

            parameterBuilder = this.parameterBuilderFactory.buildInstance();
            filterCompilation = this.filter.Compile(parameterBuilder);

            whereSegment = filterCompilation.FilterSegment;
            whereSegment = string.IsNullOrWhiteSpace(whereSegment) ? "" : $"WHERE ({whereSegment})";

            query = $@"
                DELETE FROM {this.prefix}[{this.table}]
                {whereSegment}
                ;";

            parameters = filterCompilation.Parameters.ToDictionary(p => p.Parameter, p => p.Value);
            result = new QueryCompilation(query, parameters);

            return result;
        }
    }
}