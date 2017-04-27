using System.Collections.Generic;

namespace QTFK.Models
{
    public interface IDBQuery
    {
        string Compile();
    }

    public interface IDBQueryWithTableProperty : IDBQuery
    {
        string Table { get; set; }
    }

    public interface IDBQueryWithColumnsProperty : IDBQuery
    {
        IDictionary<string, object> Columns { get; set; }
    }

    public interface IDBQueryWithFieldsProperty : IDBQuery
    {
        ICollection<string> Columns { get; set; }
    }

}