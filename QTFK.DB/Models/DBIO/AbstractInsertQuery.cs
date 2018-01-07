using QTFK.Extensions.Collections.Strings;
using QTFK.Services;
using System.Collections.Generic;
using System.Linq;

namespace QTFK.Models.DBIO
{
    public abstract class AbstractInsertQuery : IDBQueryInsert
    {
        protected string prefix;
        protected string table;
        protected readonly IDictionary<string, object> fields;
        protected readonly IParameterBuilder parameterBuilder;

        public AbstractInsertQuery(IParameterBuilder parameterBuilder)
        {
            this.prefix = "";
            this.table = "";
            this.fields = new Dictionary<string, object>();
            this.parameterBuilder = parameterBuilder;
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

        public virtual IEnumerable<string> getFields()
        {
            return this.fields.Keys;
        }

        public virtual void setColumn(string fieldName, object value)
        {
            this.fields[fieldName] = value;
        }

        public virtual QueryCompilation Compile()
        {
            QueryCompilation result;
            string columnsSegment, valuesSegment, query;
            IDictionary<string, object> parameters;

            Asserts.isFilled(this.table, $"Property '{nameof(this.Table)}' cannot be empty.");

            parameters = this.fields.ToDictionary(f => this.parameterBuilder.buildParameter("insert_" + f.Key), f => f.Value);
            columnsSegment = this.fields.Keys.Stringify(f => $" [{f}] ");
            valuesSegment = parameters.Keys.Stringify();

            query = $@"
                INSERT INTO {this.prefix}[{this.table}] ({columnsSegment})
                VALUES ({valuesSegment})
                ;";

            result = new QueryCompilation(query, parameters);

            return result;
        }
    }
}