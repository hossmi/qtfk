using QTFK.Extensions.DataSets;
using QTFK.Models;
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
            return rows.Select(AutoMap<T>);
        }

        public static IEnumerable<T> AutoMap<T>(this IEnumerable<DataRow> rows, Action<DataRow, T> configureDelegate) where T : new()
        {
            return rows.Select(r =>
            {
                T item = AutoMap<T>(r);
                configureDelegate(r, item);
                return item;
            });
        }

        public static T AutoMap<T>(this DataRow row) where T : new()
        {
            return AutoMap<T>(row.ToDictionary());
        }

        public static IEnumerable<T> AutoMap<T>(this IEnumerable<IDataRecord> records) where T : new()
        {
            var props = _GetProperties(typeof(T)).ToList();
            return records.Select(r => _AutoMap<T>(r, props));
        }

        public static IEnumerable<T> AutoMap<T>(this IEnumerable<IDataRecord> records, Action<IDataRecord, T> configureDelegate) where T : new()
        {
            var props = _GetProperties(typeof(T)).ToList();
            return records.Select(r => 
            {
                T item = _AutoMap<T>(r, props);
                configureDelegate(r, item);
                return item;
            });
        }

        public static T AutoMap<T>(this IDataRecord record) where T : new()
        {
            return _AutoMap<T>(record, _GetProperties(typeof(T)));
        }

        private static T _AutoMap<T>(IDataRecord record, IEnumerable<PropertyInfo> props) where T : new()
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

        public static T AutoMap<T>(this IDictionary<string, object> source) where T : new()
        {
            T item = new T();
            var props = _GetProperties(item.GetType());
            
            foreach (var p in props)
            {
                if (source.ContainsKey(p.Name))
                    p.SetValue(item, source[p.Name]);
            }

            return item;
        }

        private static bool _SkipNothing(PropertyInfo property)
        {
            return false;
        }

        public static void AutoMap<T>(this T source, T target)
        {
            AutoMap<T>(source, target, _SkipNothing);
        }
        public static void AutoMap<T>(this T source, T target, Func<PropertyInfo, bool> skip)
        {
            var props = _GetProperties(source.GetType())
                .Where(prop => !skip(prop))
                ;

            foreach (var p in props)
                p.SetValue(target, p.GetValue(source));
        }

        public static T AutoMap<T>(this T source) where T : new()
        {
            return AutoMap<T>(source, _SkipNothing);
        }

        public static T AutoMap<T>(this T source, Func<PropertyInfo, bool> skip) where T : new()
        {
            T target = new T();
            AutoMap<T>(source, target, skip);
            return target;
        }

        private static IEnumerable<PropertyInfo> _GetProperties(Type type)
        {
            return type
                .GetProperties()
                .Where(p => p.CanWrite && p.CanRead)
                ;
        }
    }
}