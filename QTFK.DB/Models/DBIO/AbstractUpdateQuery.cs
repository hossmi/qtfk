using QTFK.Extensions.Collections.Strings;
using QTFK.Services;
using System.Collections.Generic;
using System.Linq;

namespace QTFK.Models.DBIO
{
    public abstract class AbstractUpdateQuery : IDBQueryUpdate
    {
        protected readonly IParameterBuilder parameterBuilder;
        protected string prefix;
        protected string table;
        protected IQueryFilter filter;
        protected readonly IDictionary<string, object> fields;

        public AbstractUpdateQuery(IParameterBuilder parameterBuilder)
        {
            Asserts.isSomething(parameterBuilder, $"Constructor parameter '{nameof(parameterBuilder)}' cannot be null.");

            this.parameterBuilder = parameterBuilder;
            this.prefix = "";
            this.table = "";
            this.filter = NullQueryFilter.Instance;
            this.fields = new Dictionary<string, object>();
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

        public virtual void setColumn(string fieldName, object value)
        {
            this.fields[fieldName] = value;
        }

        public IEnumerable<string> getFields()
        {
            return this.fields.Keys;
        }

        public virtual QueryCompilation Compile()
        {
            QueryCompilation result;
            string whereSegment, columnSegment, query;
            IDictionary<string, object> parameters;
            FilterCompilation filterCompilation;

            Asserts.isFilled(this.table, $"Property '{nameof(this.Table)}' cannot be empty.");

            filterCompilation = this.filter.Compile(this.parameterBuilder);
            whereSegment = filterCompilation.FilterSegment;
            whereSegment = string.IsNullOrWhiteSpace(whereSegment) ? "" : $"WHERE ({whereSegment})";

            parameters = new Dictionary<string, object>();

            columnSegment = this.fields
                .Stringify(field =>
                {
                    string fieldResult, parameter;

                    parameter = this.parameterBuilder.buildParameter("update_" + field.Key);
                    parameters.Add(parameter, field.Value);

                    fieldResult = $" [{field.Key}] = {parameter} ";

                    return fieldResult;
                });

            query = $@"
                UPDATE {this.prefix}[{this.table}]
                SET {columnSegment}
                {whereSegment}
                ;";

            foreach (var filterParameter in filterCompilation.Parameters)
                parameters.Add(filterParameter.Parameter, filterParameter.Value);

            result = new QueryCompilation(query, parameters);

            return result;
        }

    }
}