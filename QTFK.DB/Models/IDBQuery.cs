using System;
using System.Collections.Generic;

namespace QTFK.Models
{
    public interface IDBQuery
    {
        string Compile();
        IDictionary<string, object> Parameters { get; }
        string Prefix { get; set; }
    }

    public interface IDBQueryWithTableName : IDBQuery
    {
        string Table { get; set; }
    }

    public interface IDBQueryWriteColumns : IDBQueryWithTableName
    {
        IDictionary<string, object> Fields { get; }
    }

    public interface IDBQuerySelectColumns : IDBQueryWithTableName
    {
        ICollection<SelectColumn> Columns { get; }
    }

    public interface IDBQueryJoin : IDBQuery
    {
        ICollection<JoinTable> Joins { get; }
    }


    public interface IDBQuerySelect : IDBQueryWithTableName, IDBQuerySelectColumns, IDBQueryJoin, IDBQueryFilterable
    {
    }

    public interface IDBQueryDelete : IDBQueryWithTableName, IDBQueryFilterable
    {
    }

    public interface IDBQueryInsert : IDBQueryWithTableName, IDBQueryWriteColumns
    {
    }

    public interface IDBQueryUpdate : IDBQueryWithTableName, IDBQueryWriteColumns, IDBQueryFilterable
    {
    }

}