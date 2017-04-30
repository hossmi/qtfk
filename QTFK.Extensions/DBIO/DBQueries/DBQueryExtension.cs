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

        public static T SetColumns<T>(this T query,  params string[] columns) where T : IDBQueryWithValuedFields
        {
            foreach (var column in columns)
                query.ValuedFields[column] = $"@{column}";
            return query;
        }

        /// <summary>
        /// Adds or sets the column [column] with "'@' + [column value]" as value
        /// </summary>
        /// <typeparam name="T">IDBQuery implementation</typeparam>
        /// <param name="query">IDBQuery object</param>
        /// <param name="column">name of the column</param>
        /// <returns>returns the IDBQuery</returns>
        public static T SetColumn<T>(this T query, string column) where T : IDBQueryWithValuedFields
        {
            query.ValuedFields[column] = $"@{column}";
            return query;
        }

        /// <summary>
        /// Adds or sets the column [column] with parameter [parameter] as value
        /// </summary>
        /// <typeparam name="T">IDBQuery implementation</typeparam>
        /// <param name="query">IDBQuery object</param>
        /// <param name="column">name of the column</param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static T SetColumn<T>(this T query, string column, string parameter) where T : IDBQueryWithValuedFields
        {
            query.ValuedFields[column] = parameter;
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

        public static T SetTablePrefix<T>(this T query, string prefix) where T : IDBQueryWithTablePrefix
        {
            query.Prefix = prefix;
            return query;
        }

    }
}