
using QTFK.Extensions.DBCommand;
using QTFK.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace QTFK.Extensions.DBIO
{
    public static class DBIOExtension
    {
        private static IDictionary<string, object> _defaultParameters = new Dictionary<string, object>();

        public static int Set(this IDBIO dbio, string query)
        {
            return Set(dbio, query, _defaultParameters);
        }

        public static int Set(this IDBIO dbio, string query, IDictionary<string, object> parameters)
        {
            return dbio.Set(cmd =>
            {
                cmd.CommandText = query;
                cmd.AddParameters(parameters);
                return cmd.ExecuteNonQuery();
            });
        }

        public static int SetIndividually(this IDBIO dbio, IEnumerable<string> queries)
        {
            return queries
                .Where(q => !string.IsNullOrWhiteSpace(q))
                .Sum(q => dbio.Set(q))
                ;
        }

        public static int SetInBlock(this IDBIO dbio, IEnumerable<string> queries)
        {
            return dbio.Set(cmd => queries
                .Where(q => !string.IsNullOrWhiteSpace(q))
                .Sum(q =>
                {
                    cmd.CommandText = q;
                    return cmd.ExecuteNonQuery();
                }));
        }

        public static DataSet Get(this IDBIO dbio, string query)
        {
            return dbio.Get(query, _defaultParameters);
        }

        public static IEnumerable<T> Get<T>(this IDBIO dbio, string query, Func<IDataReader, T> build)
        {
            return dbio.Get(query, _defaultParameters, build);
        }
    }
}
