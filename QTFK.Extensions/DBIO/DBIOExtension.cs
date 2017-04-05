
using QTFK.Extensions.Collections.Filters;
using QTFK.Extensions.DBCommand;
using QTFK.Models;
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

        public static void Set(this IDBIO dbio, Action<IDbCommand> instructions)
        {
            dbio.Set(cmd =>
            {
                instructions(cmd);
                return 0;
            });
        }

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

        public static int SetIndividually(this IDBIO dbio, IEnumerable<string> queries, bool throwException = true)
        {
            return queries
                .NotEmpty()
                .Sum(q =>
                {
                    var res = new Result<int>(() => dbio.Set(q));
                    if (!res.Ok && throwException)
                        throw res.Exception;
                    return res.Value;
                })
                ;
        }

        public static int SetInBlock(this IDBIO dbio, IEnumerable<string> queries, bool throwException = true)
        {
            return dbio.Set(cmd =>
            {
                int affectedRows = 0;
                foreach (string query in queries.NotEmpty())
                {
                    cmd.CommandText = query;
                    var res = new Result<int>(() => cmd.ExecuteNonQuery());
                    affectedRows += res.Value;

                    if (!res.Ok && throwException)
                        throw res.Exception;
                }
                return affectedRows;
            });
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
