using QTFK.Attributes;
using QTFK.Extensions.Collections.Dictionaries;
using QTFK.Extensions.Collections.Strings;
using QTFK.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QTFK.Models.DBIO
{
    [SqlServer]
    public class SqlInsertQuery : IDBQueryInsert
    {
        private string prefix;
        private string table;
        private readonly IDictionary<string, SetColumn> fields;
        private readonly BuildParameterDelegate buildParameterDelegate;

        public SqlInsertQuery(BuildParameterDelegate buildParameterDelegate)
        {
            Asserts.isSomething(buildParameterDelegate, $"Constructor parameter '{nameof(buildParameterDelegate)}' cannot be null.");

            this.buildParameterDelegate = buildParameterDelegate;
            this.prefix = "";
            this.table = "";
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

        public IEnumerable<string> getFields()
        {
            return this.fields.Keys;
        }

        public IDictionary<string, object> getParameters()
        {
            IDictionary<string, object> resultParameters;

            resultParameters = this.fields.Values
                .ToDictionary(p => p.Parameter, p => p.Value);

            return resultParameters;
        }

        public void setColumn(string fieldName, object value)
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
                    Parameter = this.buildParameterDelegate("insert_" + fieldName),
                };

                this.fields.Add(fieldName, column);
            }
        }

        public string Compile()
        {
            string columnsSegment, valuesSegment;

            Asserts.isFilled(this.table, $"Property '{nameof(this.Table)}' cannot be empty.");

            columnsSegment = this.fields.Values.Stringify(f => $" [{f.Name}] ");
            valuesSegment = this.fields.Values.Stringify(f => $" [{f.Parameter}] ");

            return $@"
                INSERT INTO {this.prefix}[{this.table}] ({columnsSegment})
                VALUES ({valuesSegment})
                ;";
        }
    }
}