using QTFK.Attributes;
using QTFK.Extensions.Collections.Dictionaries;
using QTFK.Extensions.Collections.Strings;
using System.Collections.Generic;
using System.Linq;

namespace QTFK.Models.DBIO
{
    [OleDB]
    public class OleDBInsertQuery : IDBQueryInsert
    {
        private string prefix;
        private string table;
        private IList<SetColumn> fields;

        public OleDBInsertQuery()
        {
            this.fields = new List<SetColumn>();
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
                this.prefix = value.Trim();
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

        public ICollection<SetColumn> Fields
        {
            get
            {
                return this.fields;
            }
        }

        public string Compile()
        {
            string compilationResult;
            string insertColumnsSegment, valuesColumnsSegment;

            Asserts.isFilled(this.Table, $"Property '{nameof(this.Table)}' cannot be empty.");

            insertColumnsSegment = this.fields.Stringify(c => $"[{c.Name}]");
            valuesColumnsSegment = this.fields.Stringify(c => c.Parameter); //columnList.Stringify(c => c.V.StartsWith("@") ? c.V : $"'{c.V}'")

            compilationResult = $@"
                INSERT INTO {this.prefix}[{this.table}] ({insertColumnsSegment})
                VALUES ({valuesColumnsSegment})
                ;";

            return compilationResult;
        }

        public IDictionary<string, object> getParameters()
        {
            return this.fields.ToDictionary(k => k.Parameter, v => v.Value);
        }
    }
}