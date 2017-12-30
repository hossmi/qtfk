using System;
using System.Collections.Generic;

namespace QTFK.Models
{
    public interface IDBQuery
    {
        string Compile();
        IDictionary<string, object> getParameters();
        string Table { get; set; }
        string Prefix { get; set; }
    }

    public interface IDBQueryWriteColumns : IDBQuery
    {
        void setColumn(string fieldName, object value);
        IEnumerable<string> getFields();
    }

    public interface IDBQuerySelectColumns : IDBQuery
    {
        void addColumn(SelectColumn column);
        IEnumerable<SelectColumn> getColumns();
    }

    public interface IDBQuerySelect : IDBQuery, IDBQuerySelectColumns, IDBQueryFilterable
    {
    }

    public interface IDBQueryDelete : IDBQuery, IDBQueryFilterable
    {
    }

    public interface IDBQueryInsert : IDBQuery, IDBQueryWriteColumns
    {
    }

    public interface IDBQueryUpdate : IDBQuery, IDBQueryWriteColumns, IDBQueryFilterable
    {
    }

    public interface IDBQueryFilterable : IDBQuery
    {
        IQueryFilter Filter { get; set; }
    }

}