using System;
using System.Collections.Generic;
using System.Data;

namespace QTFK.Extensions.DBCommand
{
    public static class DBCommandExtension
    {
        public static IDbCommand setCommandText(this IDbCommand cmd, string commandText)
        {
            cmd.CommandText = commandText;
            return cmd;
        }

        public static IDbCommand addParameter(this IDbCommand cmd, string name, object value)
        {
            return prv_addParameter(cmd, name, value);
        }

        public static IDbCommand addParameter(this IDbCommand cmd, KeyValuePair<string, object> parameter)
        {
            return prv_addParameter(cmd, parameter.Key, parameter.Value);
        }

        public static IDbCommand addParameters(this IDbCommand cmd, IEnumerable<KeyValuePair<string, object>> parameters)
        {
            foreach (var p in parameters)
                cmd.addParameter(p.Key, p.Value);

            return cmd;
        }

        public static IDbCommand clearParameters(this IDbCommand cmd)
        {
            cmd.Parameters.Clear();
            return cmd;
        }

        private static IDbCommand prv_addParameter(IDbCommand cmd, string name, object value)
        {
            var p = cmd.CreateParameter();
            p.ParameterName = name;
            p.Value = value ?? DBNull.Value;
            cmd.Parameters.Add(p);

            return cmd;
        }

    }
}
