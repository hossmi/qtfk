using System;
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

    public interface IDBQueryJoin : IDBQuery
    {
        ICollection<JoinTable> Joins { get; set; }
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