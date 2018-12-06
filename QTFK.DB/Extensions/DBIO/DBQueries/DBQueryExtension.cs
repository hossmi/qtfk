using QTFK.Extensions.Collections.Dictionaries;
using QTFK.Models;
using QTFK.Services;
using System.Linq;
using System;
using System.Collections.Generic;

namespace QTFK.Extensions.DBIO.DBQueries
{
    public static class DBQueryExtension
    {
        public static T setTable<T>(this T query, string tableName) where T : IDBQuery
        {
            query.Table = tableName;
            return query;
        }

        public static T column<T>(this T query, string columnName, object columnValue) where T : IDBQueryWriteColumns
        {
            query.setColumn(columnName, columnValue);
            return query;
        }

        public static T column<T>(this T query, string columnName) where T : IDBQuerySelectColumns
        {
            prv_addColumn<T>(query, columnName, null);
            return query;
        }

        public static T addColumn<T>(this T query, string column, string alias) where T : IDBQuerySelectColumns
        {
            prv_addColumn<T>(query, column, alias);
            return query;
        }

        public static T setFilter<T>(this T query, IQueryFilter filter) where T : IDBQueryFilterable
        {
            query.Filter = filter;
            return query;
        }

        public static T setPrefix<T>(this T query, string prefix) where T : IDBQuery
        {
            query.Prefix = prefix;
            return query;
        }

        public static int execute(this IDBQuery query, IDBIO db)
        {
            return db.Set(query);
        }

        private static void prv_addColumn<T>(T query, string column, string alias) where T : IDBQuerySelectColumns
        {
            query.addColumn(new SelectColumn
            {
                Name = column,
                Alias = alias,
            });
        }

    }
}