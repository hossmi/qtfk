using System;
using System.Collections.Generic;

namespace QTFK.Models
{
    public interface IDBQuery
    {
        string Compile();
    }

    public interface ICrudDBQuery : IDBQuery
    {
        string Table { get; set; }
        string Prefix { get; set; }
    }

    public interface IDBQueryWriteColumns : ICrudDBQuery
    {
        ICollection<SetColumn> Fields { get; }
    }

    public interface IDBQuerySelectColumns : ICrudDBQuery
    {
        ICollection<SelectColumn> Columns { get; }
    }

    public interface IDBQueryJoin : IDBQuery
    {
        ICollection<JoinTable> Joins { get; }
    }

    public interface IDBQuerySelect : ICrudDBQuery, IDBQuerySelectColumns, IDBQueryJoin, IDBQueryFilterable
    {
    }

    public interface IDBQueryDelete : ICrudDBQuery, IDBQueryFilterable
    {
    }

    public interface IDBQueryInsert : ICrudDBQuery, IDBQueryWriteColumns
    {
    }

    public interface IDBQueryUpdate : ICrudDBQuery, IDBQueryWriteColumns, IDBQueryFilterable
    {
    }

}