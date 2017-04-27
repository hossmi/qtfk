using QTFK.Models;
using QTFK.Services;
using System.Linq;

namespace QTFK.Extensions.DBIO.DBQueries
{
    public static class DBQueryExtension
    {
        public static T SetTable<T>(this T query, string tableName) where T : IDBQueryWithTableProperty
        {
            query.Table = tableName;
            return query;
        }

        public static T SetColumn<T>(this T query, string column, object value) where T : IDBQueryWithColumnsProperty
        {
            query.Columns[column] = value;
            return query;
        }

        public static T AddColumn<T>(this T query, string column) where T : IDBQueryWithFieldsProperty
        {
            query.Columns.Add(column);
            return query;
        }

        public static T AddColumns<T>(this T query, params string[] columns) where T : IDBQueryWithFieldsProperty
        {
            foreach (var column in columns)
                query.Columns.Add(column);
            return query;
        }
    }
}