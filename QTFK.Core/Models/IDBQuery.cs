using System.Collections.Generic;

namespace QTFK.Models
{
    public interface IDBQuery
    {
        string Compile();
        IDictionary<string, object> Parameters { get; set; }
        string Prefix { get; set; }
    }

    public interface IDBQueryWithTableName : IDBQuery
    {
        string Table { get; set; }
    }

    public interface IDBQueryWriteColumns : IDBQueryWithTableName
    {
        IDictionary<string, object> Fields { get; set; }
    }

    public interface IDBQuerySelectColumns : IDBQueryWithTableName
    {
        ICollection<SelectColumn> Columns { get; set; }
    }

    public interface IDBQueryWhereClause : IDBQuery
    {
        string Where { get; set; }
    }

    public interface IDBQueryJoin : IDBQuery
    {
        ICollection<JoinTable> Joins { get; set; }
    }

    public interface IDBCrudQuery : IDBQueryWithTableName
    {
    }

    public interface IDBQuerySelect : IDBCrudQuery, IDBQuerySelectColumns, IDBQueryWhereClause, IDBQueryJoin
    {
    }

    public interface IDBQueryDelete : IDBCrudQuery, IDBQueryWhereClause
    {
    }

    public interface IDBQueryInsert : IDBCrudQuery, IDBQueryWriteColumns
    {
    }

    public interface IDBQueryUpdate : IDBCrudQuery, IDBQueryWriteColumns, IDBQueryWhereClause
    {
    }

}