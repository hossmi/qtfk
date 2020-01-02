using QTFK.Extensions.DataSets;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

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
            return prv_autoMap<T>(record, typeof(T).prv_getReadWriteProperties());
        }

        public static T AutoMap<T>(this IDataRecord record, Action<T> configureDelegate) where T : new()
        {
            T item = prv_autoMap<T>(record, typeof(T).prv_getReadWriteProperties());
            configureDelegate(item);
            return item;
        }

        private static IEnumerable<T> prv_autoMap<T>(IEnumerable<IDataRecord> records, Action<IDataRecord, T> configureDelegate) where T : new()
        {
            IList<PropertyInfo> properties;

            properties = typeof(T)
                .prv_getReadWriteProperties()
                .ToList();

            return records.Select(r =>
            {
                T item = prv_autoMap<T>(r, properties);
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
            T item;
            IDictionary<string, DataColumn> columns;
            
            item = new T();
            columns = row
                .getColumns()
                .ToDictionary(c => c.ColumnName);

            foreach (PropertyInfo p in typeof(T).prv_getReadWriteProperties())
            {
                DataColumn column;

                if (columns.TryGetValue(p.Name, out column))
                    p.SetValue(item, row[column]);
            }

            return item;
        }

        private static T prv_autoMap<T>(IDataRecord record, IEnumerable<PropertyInfo> props) where T : new()
        {
            T item = new T();

            foreach (PropertyInfo p in props)
            {
                try
                {
                    int columnIndex;

                    columnIndex = record.GetOrdinal(p.Name);

                    if(0 <= columnIndex)
                        p.SetValue(item, record[columnIndex]);
                }
                catch (IndexOutOfRangeException)
                {
                }
            }

            return item;
        }

        private static IEnumerable<PropertyInfo> prv_getReadWriteProperties(this Type type)
        {
            return type
                .GetProperties()
                .Where(p => p.CanWrite && p.CanRead);
        }
    }
}