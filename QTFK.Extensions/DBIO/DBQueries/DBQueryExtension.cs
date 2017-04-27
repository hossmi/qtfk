using QTFK.Models;
using QTFK.Services;
using System.Linq;

namespace QTFK.Extensions.DBIO.DBQueries
{
    public static class DBQueryExtension
    {
        public static T SetTable<T>(this T query, string tableName) where T : IDBQueryWithTableName
        {
            query.Table = tableName;
            return query;
        }

        public static T SetColumn<T>(this T query, string column, object value) where T : IDBQueryWithValuedFields
        {
            query.ValuedFields[column] = value;
            return query;
        }

        public static T AddColumn<T>(this T query, string column) where T : IDBQueryWithFields
        {
            query.Fields.Add(column);
            return query;
        }

        public static T AddColumns<T>(this T query, params string[] columns) where T : IDBQueryWithFields
        {
            foreach (var column in columns)
                query.Fields.Add(column);
            return query;
        }

        public static T SetWhere<T>(this T query, string where) where T : IDBQueryWithWhere
        {
            query.Where = where;
            return query;
        }

    }
}