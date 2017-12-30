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
        protected readonly IDictionary<string, SetColumn> fields;

        public AbstractUpdateQuery(IParameterBuilder parameterBuilder)
        {
            Asserts.isSomething(parameterBuilder, $"Constructor parameter '{nameof(parameterBuilder)}' cannot be null.");

            this.parameterBuilder = parameterBuilder;
            this.prefix = "";
            this.table = "";
            this.filter = NullQueryFilter.Instance;
            this.fields = new Dictionary<string, SetColumn>();
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

        public virtual IEnumerable<string> getFields()
        {
            return this.fields.Keys;
        }

        public virtual void setColumn(string fieldName, object value)
        {
            if (this.fields.ContainsKey(fieldName))
            {
                this.fields[fieldName].Value = value;
            }
            else
            {
                SetColumn column;

                column = new SetColumn
                {
                    Name = fieldName,
                    Value = value,
                    Parameter = this.parameterBuilder.buildParameter("update_" + fieldName),
                };

                this.fields.Add(fieldName, column);
            }
        }

        public virtual IDictionary<string, object> getParameters()
        {
            IDictionary<string, object> resultParameters;
            IDictionary<string, object> filterParameters;

            resultParameters = this.fields.Values
                .ToDictionary(p => p.Parameter, p => p.Value);

            filterParameters = this.filter.getParameters();

            foreach (var pair in filterParameters)
            {
                Asserts.check(resultParameters.ContainsKey(pair.Key) == false, $"Filter parameter '{pair.Key}' already exists in query parameters.");
                resultParameters.Add(pair);
            }

            return resultParameters;
        }

        public virtual string Compile()
        {
            string result;
            string whereSegment, columnSegment;

            Asserts.isFilled(this.table, $"Property '{nameof(this.Table)}' cannot be empty.");

            whereSegment = this.filter.Compile();
            whereSegment = string.IsNullOrWhiteSpace(whereSegment) ? "" : $"WHERE ({whereSegment})";

            columnSegment = this.fields
                .Stringify(field => $" [{field.Value.Name}] = {field.Value.Parameter} ");

            result = $@"
                UPDATE {this.prefix}[{this.table}]
                SET {columnSegment}
                {whereSegment}
                ;";

            return result;
        }
    }
}