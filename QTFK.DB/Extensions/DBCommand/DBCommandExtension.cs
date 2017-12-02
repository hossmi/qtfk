using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTFK.Extensions.DBCommand
{
    public static class DBCommandExtension
    {
        public static IDbCommand SetCommandText(this IDbCommand cmd, string query)
        {
            cmd.CommandText = query;
            return cmd;
        }
        public static IDbCommand AddParameter(this IDbCommand cmd, string name, object value)
        {
            var p = cmd.CreateParameter();
            p.ParameterName = name;
            p.Value = value ?? DBNull.Value;
            cmd.Parameters.Add(p);

            return cmd;
        }
        public static IDbCommand AddParameters(this IDbCommand cmd, IDictionary<string,object> parameters)
        {
            foreach (var p in parameters)
                cmd.AddParameter(p.Key, p.Value);

            return cmd;
        }

        public static IDbCommand ClearParameters(this IDbCommand cmd)
        {
            cmd.Parameters.Clear();
            return cmd;
        }
    }
}
