using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTFK.Extensions.Collections.Dictionaries
{
    public static class DictionaryExtension
    {
        public static T Get<T>(this IDictionary<string, object> dic, string key)
        {
            return Get<T>(dic, key, default(T));
        }
        public static T Get<T>(this IDictionary<string, object> dic, string key, T defaultValue)
        {
            object value = null;
            if (dic.TryGetValue(key, out value))
                return (T)value;
            else
                return defaultValue;
        }

        public static IDictionary<string, object> Get<T>(this IDictionary<string, object> dic, string key, Action<T> setter)
        {
            Get<T>(dic, key, default(T), setter);
            return dic;
        }
        public static IDictionary<string, object> Get<T>(this IDictionary<string, object> dic, string key, T defaultValue, Action<T> setter)
        {
            object value = null;
            if(dic.TryGetValue(key, out value))
                setter((T)value);
            else
                setter(defaultValue);
            return dic;
        }

        public static IDictionary<string, string> Get(this IDictionary<string, string> dic, string key, Action<string> setter)
        {
            string value = string.Empty;
            if (dic.TryGetValue(key, out value))
                setter(value);
            else
                setter(string.Empty);
            return dic;
        }
        public static string Get(this IDictionary<string, string> dic, string key)
        {
            string value = string.Empty;
            if (dic.TryGetValue(key, out value))
                return value;
            else
                return string.Empty;
        }
    }
}
