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
        protected readonly IDictionary<string, SetColumn> fields;
        protected readonly IParameterBuilder parameterBuilder;

        public AbstractInsertQuery(IParameterBuilder parameterBuilder)
        {
            this.prefix = "";
            this.table = "";
            this.fields = new Dictionary<string, SetColumn>();
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

        public virtual IEnumerable<QueryParameter> getParameters()
        {
            return this.fields.Values
                .Select(p => new QueryParameter { Parameter = p.Parameter, Value = p.Value });
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
                    Parameter = this.parameterBuilder.buildParameter("insert_" + fieldName),
                };

                this.fields.Add(fieldName, column);
            }
        }

        public virtual string Compile()
        {
            string columnsSegment, valuesSegment;

            Asserts.isFilled(this.table, $"Property '{nameof(this.Table)}' cannot be empty.");

            columnsSegment = this.fields.Values.Stringify(f => $" [{f.Name}] ");
            valuesSegment = this.fields.Values.Stringify(f => $" {f.Parameter} ");

            return $@"
                INSERT INTO {this.prefix}[{this.table}] ({columnsSegment})
                VALUES ({valuesSegment})
                ;";
        }
    }
}