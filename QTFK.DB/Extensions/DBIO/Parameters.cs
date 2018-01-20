using System;
using System.Collections.Generic;

namespace QTFK.Extensions.DBIO
{
    public static class Parameters
    {
        public static ICollection<KeyValuePair<string, object>> start()
        {
            return prv_new();
        }

        public static ICollection<KeyValuePair<string, object>> push(string name, object value)
        {
            ICollection<KeyValuePair<string, object>> parameters;

            parameters = prv_new();

            return prv_push(parameters, name, value);
        }

        public static ICollection<KeyValuePair<string, object>> push(this ICollection<KeyValuePair<string, object>> parameters, string name, object value)
        {
            return prv_push(parameters, name, value);
        }

        private static ICollection<KeyValuePair<string, object>> prv_push(ICollection<KeyValuePair<string, object>> parameters, string name, object value)
        {
            KeyValuePair<string, object> parameter;

            parameter = new KeyValuePair<string, object>(name, value);
            parameters.Add(parameter);

            return parameters;
        }

        private static ICollection<KeyValuePair<string, object>> prv_new()
        {
            ICollection<KeyValuePair<string, object>> parameters;

            parameters = new List<KeyValuePair<string, object>>();

            return parameters;
        }

    }
}