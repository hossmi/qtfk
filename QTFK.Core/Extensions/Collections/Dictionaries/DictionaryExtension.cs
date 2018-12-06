using System.Collections.Generic;

namespace QTFK.Extensions.Collections.Dictionaries
{
    public static class DictionaryExtension
    {
        private static T prv_get<T>(this IDictionary<string, object> dic, string key, T defaultValue)
        {
            object value = null;
            if (dic.TryGetValue(key, out value))
                return (T)value;
            else
                return defaultValue;
        }

        public static T get<T>(this IDictionary<string, object> dic, string key)
        {
            return prv_get<T>(dic, key, default(T));
        }

        public static T get<T>(this IDictionary<string, object> dic, string key, T defaultValue)
        {
            return prv_get<T>(dic, key, defaultValue);
        }
    }
}
