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
        public static void AddParameter(this IDbCommand cmd, string name, object value)
        {
            var p = cmd.CreateParameter();
            p.ParameterName = name;
            p.Value = value ?? DBNull.Value;
            cmd.Parameters.Add(p);
        }
        public static void AddParameters(this IDbCommand cmd, IDictionary<string,object> parameters)
        {
            foreach (var p in parameters)
                cmd.AddParameter(p.Key, p.Value);
        }

        public static IDbCommand ClearParameters(this IDbCommand cmd)
        {
            cmd.Parameters.Clear();
            return cmd;
        }
    }
}
