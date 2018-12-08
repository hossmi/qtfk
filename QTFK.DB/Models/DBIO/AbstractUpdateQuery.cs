using QTFK.Extensions.Collections.Strings;
using QTFK.Extensions.DBIO;
using QTFK.Services;
using System.Collections.Generic;
using System.Linq;

namespace QTFK.Models.DBIO
{
    public abstract class AbstractUpdateQuery : IDBQueryUpdate
    {
        protected readonly IParameterBuilderFactory parameterBuilderFactory;
        protected string prefix;
        protected string table;
        protected IQueryFilter filter;
        protected readonly IDictionary<string, object> fields;

        public AbstractUpdateQuery(IParameterBuilderFactory parameterBuilderFactory)
        {
            Asserts.isNotNull(parameterBuilderFactory);

            this.parameterBuilderFactory = parameterBuilderFactory;
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
            ICollection<KeyValuePair<string,object>> fieldParameters;
            IEnumerable<KeyValuePair<string,object>> allParameters;
            FilterCompilation filterCompilation;
            IParameterBuilder parameterBuilder;

            Asserts.stringIsNotEmpty(this.table);

            parameterBuilder = this.parameterBuilderFactory.buildInstance();
            filterCompilation = this.filter.Compile(parameterBuilder);
            whereSegment = filterCompilation.FilterSegment;
            whereSegment = string.IsNullOrWhiteSpace(whereSegment) ? "" : $"WHERE ({whereSegment})";

            fieldParameters = Parameters.start();

            columnSegment = this.fields
                .Stringify(field =>
                {
                    string fieldResult, parameter;

                    parameter = parameterBuilder.buildParameter("update_" + field.Key);
                    fieldParameters.push(parameter, field.Value);

                    fieldResult = $" [{field.Key}] = {parameter} ";

                    return fieldResult;
                });

            query = $@"
                UPDATE {this.prefix}[{this.table}]
                SET {columnSegment}
                {whereSegment}
                ;";

            allParameters = fieldParameters.Concat(filterCompilation.Parameters);

            result = new QueryCompilation(query, allParameters);

            return result;
        }

    }
}