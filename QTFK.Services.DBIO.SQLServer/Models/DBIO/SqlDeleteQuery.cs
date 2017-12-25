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
    public class SqlDeleteQuery : IDBQueryDelete
    {
        private string prefix;
        private string table;
        private IQueryFilter filter;

        public SqlDeleteQuery()
        {
            this.prefix = "";
            this.table = "";
            this.filter = NullQueryFilter.Instance;
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

        public string Compile()
        {
            string whereSegment;

            Asserts.isFilled(this.Table, $"Property '{nameof(this.Table)}' cannot be empty.");

            whereSegment = this.filter.Compile();
            whereSegment = string.IsNullOrWhiteSpace(whereSegment) ? "" : $"WHERE ({whereSegment})";

            return $@"
                DELETE FROM {this.prefix}[{this.table}]
                {whereSegment}
                ;";
        }

        public IDictionary<string, object> getParameters()
        {
            return this.filter.getParameters();
        }
    }
}