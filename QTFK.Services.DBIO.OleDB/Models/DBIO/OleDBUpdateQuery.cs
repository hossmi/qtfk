using QTFK.Attributes;
using QTFK.Extensions.Collections.Dictionaries;
using QTFK.Extensions.Collections.Strings;
using QTFK.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QTFK.Models.DBIO
{
    [OleDB]
    public class OleDBUpdateQuery : IDBQueryUpdate
    {
        private string prefix;
        private string table;
        private IQueryFilter filter;
        private readonly IDictionary<string, SetColumn> fields;
        private readonly BuildParameterDelegate buildParameterDelegate;

        public OleDBUpdateQuery(BuildParameterDelegate buildParameterDelegate)
        {
            Asserts.isSomething(buildParameterDelegate, $"Constructor parameter '{nameof(buildParameterDelegate)}' cannot be null.");

            this.buildParameterDelegate = buildParameterDelegate;
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

        public IEnumerable<string> getFields()
        {
            return this.fields.Keys;
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
                    Parameter = this.buildParameterDelegate("update_" + fieldName),
                };

                this.fields.Add(fieldName, column);
            }
        }

        public IDictionary<string, object> getParameters()
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

        public string Compile()
        {
            string result;
            string whereSegment, columnSegment;

            Asserts.isFilled(this.table, $"Property '{nameof(this.Table)}' cannot be empty.");

            whereSegment = this.filter.Compile();
            whereSegment = string.IsNullOrWhiteSpace(whereSegment) ? "" : $"WHERE ({whereSegment})";

            columnSegment = this.fields
                .Stringify(field => prv_buildUpdateColumn(field.Value));

            result = $@"
                UPDATE {this.prefix}[{this.table}]
                SET {columnSegment}
                {whereSegment}
                ;";

            return result;
        }

        private static string prv_buildUpdateColumn(SetColumn column)
        {
            return $" [{column.Name}] = {column.Parameter} ";
        }
    }
}