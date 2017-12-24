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
        public static T SetTable<T>(this T query, string tableName) where T : ICrudDBQuery
        {
            query.Table = tableName;
            return query;
        }



        public static T Set<T>(this T query, string table, Action<ICollection<SetColumn>> columns) where T : IDBQueryWriteColumns 
        {
            var columnsCollection = new List<SetColumn>();
            columns(columnsCollection);
            columnsCollection.ForEach(c => SetColumn<T>(query, c.Name, c.Value, c.Parameter));
            query.Table = table;
            return query;
        }

        public static T SetColumn<T>(this T query, string column, object value, string parameter = null) where T : IDBQueryWriteColumns
        {
            parameter = string.IsNullOrWhiteSpace(parameter) ? $"@{column}" : parameter;
            query.Fields.Set(column, parameter);
            query.Parameters.Set(parameter, value);
            return query;
        }

        public static ICollection<SetColumn> Column(this ICollection<SetColumn> columns, string name, object value = null, string parameter = null)
        {
            columns.Add(new SetColumn
            {
                Name = name,
                Parameter = parameter,
                Value = value,
            });
            return columns;
        }



        public static T Select<T>(this T query, string table, Action<ICollection<SelectColumn>> columns) where T : IDBQuerySelectColumns
        {
            var columnsCollection = new List<SelectColumn>();
            columns(columnsCollection);
            columnsCollection.ForEach(c => query.Columns.Add(c));
            query.Table = table;
            return query;
        }

        public static T AddColumn<T>(this T query, string column, string alias = null) where T : IDBQuerySelectColumns
        {
            query.Columns.Add(new SelectColumn { Name = column, Alias = alias });
            return query;
        }

        public static ICollection<SelectColumn> Column(this ICollection<SelectColumn> columns, string name, string alias = null)
        {
            columns.Add(new SelectColumn
            {
                Name = name,
                Alias = alias,
            });
            return columns;
        }



        public static T SetFilter<T>(this T query, IQueryFilter filter) where T : IDBQueryFilterable
        {
            query.Filter = filter;
            return query;
        }

        public static T SetPrefix<T>(this T query, string prefix) where T : IDBQuery
        {
            query.Prefix = prefix;
            return query;
        }

        public static T AddJoin<T>(this T query, JoinKind kind, string rightTable, Action<ICollection<JoinMatch>> matches, Action<ICollection<SelectColumn>> columns) where T : IDBQueryJoin
        {
            var joinMatches = new List<JoinMatch>();
            matches(joinMatches);

            var columnsCollection = new List<SelectColumn>();
            columns(columnsCollection);

            query.Joins.Add(new JoinTable
            {
                Table = rightTable,
                Kind = kind,
                Matches = joinMatches,
                Columns = columnsCollection,
            });

            return query;
        }

        public static T AddJoin<T>(this T query, JoinKind kind, string rightTable, string leftField, string rightField, Action<ICollection<SelectColumn>> columns) where T : IDBQueryJoin
        {
            return AddJoin<T>(query, kind, rightTable, m => m.Add(leftField, rightField), columns);
        }

        public static ICollection<JoinMatch> Add(this ICollection<JoinMatch> matches, string leftField, string rightField)
        {
            matches.Add(new JoinMatch
            {
                LeftField = leftField,
                RightField = rightField,
            });
            return matches;
        }



        public static T SetParam<T>(this T query, string parameter, object value) where T : IDBQuery
        {
            query.Parameters.Set(parameter, value);
            return query;
        }
    }
}