using System.Collections.Generic;

namespace QTFK.Models
{
    public interface IDBQuery
    {
        string Compile();
    }

    public interface IDBQueryWithTableName : IDBQuery
    {
        string Table { get; set; }
    }

    public interface IDBQueryWithValuedFields : IDBQuery
    {
        IDictionary<string, object> ValuedFields { get; set; }
    }

    public interface IDBQueryWithFields : IDBQuery
    {
        ICollection<string> Fields { get; set; }
    }

    public interface IDBQueryWithWhere : IDBQuery
    {
        string Where { get; set; }
    }

}