using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace QTFK.Extensions.Collections.Dictionaries
{
    public static class DictionaryExtension
    {
        public static T get<T>(this IDictionary<string, object> dic, string key)
        {
            return prv_get<T>(dic, key, default(T));
        }

        public static T get<T>(this IDictionary<string, object> dic, string key, T defaultValue)
        {
            return prv_get<T>(dic, key, defaultValue);
        }

        public static T toEntity<T>(this IDictionary<string, object> source) where T : new()
        {
            T item;

            item = new T();

            typeof(T)
                .GetProperties()
                .Where(p => p.CanWrite && p.CanRead)
                .ToList()
                .ForEach(p =>
                {
                    object propertyValue;

                    if (source.TryGetValue(p.Name, out propertyValue))
                        p.SetValue(item, propertyValue);
                });

            return item;
        }

        public static IDictionary<string, object> toDictionary<T>(this T item)
        {
            return typeof(T)
                .GetProperties()
                .Where(p => p.CanWrite && p.CanRead)
                .ToDictionary(p => p.Name, p => p.GetValue(item));
        }


        private static T prv_get<T>(this IDictionary<string, object> dic, string key, T defaultValue)
        {
            object value = null;
            if (dic.TryGetValue(key, out value))
                return (T)value;
            else
                return defaultValue;
        }
    }
}
