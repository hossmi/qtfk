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
        protected readonly IParameterBuilderFactory parameterBuilderFactory;

        public AbstractInsertQuery(IParameterBuilderFactory parameterBuilderFactory)
        {
            this.prefix = "";
            this.table = "";
            this.fields = new Dictionary<string, object>();
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
            IParameterBuilder parameterBuilder;

            Asserts.stringIsNotEmpty(this.table);

            parameterBuilder = this.parameterBuilderFactory.buildInstance();
            parameters = this.fields.ToDictionary(f => parameterBuilder.buildParameter("insert_" + f.Key), f => f.Value);
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