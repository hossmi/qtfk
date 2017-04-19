using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTFK.Extensions.Collections.Dictionaries
{
    public static class DictionaryExtension
    {
        public static void Get<T>(this IDictionary<string, object> dic, string key, Action<T> setter)
        {
            Get<T>(dic, key, default(T), setter);
        }
        public static void Get<T>(this IDictionary<string, object> dic, string key, T defaultValue, Action<T> setter)
        {
            object value = null;
            if(dic.TryGetValue(key, out value))
                setter((T)value);
            else
                setter(defaultValue);
        }
    }
}
