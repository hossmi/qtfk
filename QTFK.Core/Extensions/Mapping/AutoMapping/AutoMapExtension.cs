using QTFK.Extensions.DataSets;
using QTFK.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using QTFK.Extensions.Objects.Properties;

namespace QTFK.Extensions.Mapping.AutoMapping
{
    public static class AutoMapExtension
    {
        public static IEnumerable<T> AutoMap<T>(this IEnumerable<DataRow> rows) where T : new()
        {
            return prv_autoMap<T>(rows, null);
        }

        public static IEnumerable<T> AutoMap<T>(this IEnumerable<DataRow> rows, Action<DataRow, T> configureDelegate) where T : new()
        {
            return prv_autoMap<T>(rows, configureDelegate);
        }

        public static IEnumerable<T> AutoMap<T>(this IEnumerable<IDataRecord> records) where T : new()
        {
            return prv_autoMap<T>(records, null);
        }

        public static IEnumerable<T> AutoMap<T>(this IEnumerable<IDataRecord> records, Action<IDataRecord, T> configureDelegate) where T : new()
        {
            return prv_autoMap<T>(records, configureDelegate);
        }

        public static T AutoMap<T>(this DataRow row) where T : new()
        {
            return prv_autoMap<T>(row);
        }

        public static T AutoMap<T>(this IDataRecord record) where T : new()
        {
            return prv_autoMap<T>(record, typeof(T).getReadWriteProperties());
        }

        public static T AutoMap<T>(this IDataRecord record, Action<T> configureDelegate) where T : new()
        {
            T item = prv_autoMap<T>(record, typeof(T).getReadWriteProperties());
            configureDelegate(item);
            return item;
        }

        public static T AutoMap<T>(this IDictionary<string, object> source) where T : new()
        {
            return prv_autoMap<T>(source);
        }

        private static IEnumerable<T> prv_autoMap<T>(IEnumerable<IDataRecord> records, Action<IDataRecord, T> configureDelegate) where T : new()
        {
            var props = typeof(T)
                .getReadWriteProperties()
                .ToList()
                ;

            return records.Select(r =>
            {
                T item = prv_autoMap<T>(r, props);
                configureDelegate?.Invoke(r, item);
                return item;
            });
        }

        private static IEnumerable<T> prv_autoMap<T>(IEnumerable<DataRow> rows, Action<DataRow, T> configureDelegate) where T : new()
        {
            return rows.Select(r =>
            {
                T item = prv_autoMap<T>(r);
                configureDelegate?.Invoke(r, item);
                return item;
            });
        }

        private static T prv_autoMap<T>(DataRow row) where T : new()
        {
            return prv_autoMap<T>(row.ToDictionary());
        }

        private static T prv_autoMap<T>(IDataRecord record, IEnumerable<PropertyInfo> props) where T : new()
        {
            T item = new T();

            foreach (var p in props)
            {
                var result = new Result<int>(() => record.GetOrdinal(p.Name));
                if (result.Ok && result.Value >= 0)
                    p.SetValue(item, record[result.Value]);
            }

            return item;
        }

        private static T prv_autoMap<T>(IDictionary<string, object> source) where T : new()
        {
            T item = new T();
            var props = item
                .GetType()
                .getReadWriteProperties()
                .ToList()
                ;
            
            foreach (var p in props)
            {
                if (source.ContainsKey(p.Name))
                    p.SetValue(item, source[p.Name]);
            }

            return item;
        }
    }
}