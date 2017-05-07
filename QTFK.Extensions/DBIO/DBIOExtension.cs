
using QTFK.Extensions.Collections.Filters;
using QTFK.Extensions.Collections.Dictionaries;
using QTFK.Extensions.DBCommand;
using QTFK.Extensions.Mapping.AutoMapping;
using QTFK.Models;
using QTFK.Services;
using System;
using System.Collections.Generic;
using System.Data;

namespace QTFK.Extensions.DBIO
{
    public static class DBIOExtension
    {
        public static void Set(this IDBIO dbio, Action<IDbCommand> instructions)
        {
            dbio.Set(cmd =>
            {
                instructions(cmd);
                return 0;
            });
        }

        public static int Set(this IDBIO dbio, IDBQuery query)
        {
            return Set(dbio, query.Compile(), query.Parameters);
        }

        public static int Set(this IDBIO dbio, IDBQuery query, IDictionary<string, object> parameters)
        {
            return Set(dbio, query.Compile(), parameters);
        }

        public static int Set(this IDBIO dbio, string query)
        {
            return Set(dbio, query, dbio.Params());
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

        public static int Set(this IDBIO dbio, IEnumerable<string> queries, bool throwOnException = true)
        {
            return dbio.Set(cmd =>
            {
                int affectedRows = 0;
                foreach (string query in queries.NotEmpty())
                {
                    cmd.CommandText = query;
                    var res = new Result<int>(() => cmd.ExecuteNonQuery());
                    affectedRows += res.Value;

                    if (!res.Ok && throwOnException)
                        throw res.Exception;
                }
                return affectedRows;
            });
        }



        public static DataSet Get(this IDBIO dbio, string query)
        {
            return dbio.Get(query, dbio.Params());
        }



        public static IEnumerable<T> Get<T>(this IDBIO dbio, string query, Func<IDataRecord, T> buildDelegate)
        {
            return dbio.Get<T>(query, dbio.Params(), buildDelegate);
        }

        public static IEnumerable<T> Get<T>(this IDBIO dbio, string query) where T : new()
        {
            return dbio.Get<T>(query, AutoMapExtension.AutoMap<T>);
        }

        public static IEnumerable<T> Get<T>(this IDBIO dbio, string query, IDictionary<string, object> parameters) where T : new()
        {
            return dbio.Get<T>(query, parameters, AutoMapExtension.AutoMap<T>);
        }



        public static IEnumerable<T> Get<T>(this IDBIO dbio, IDBQuery query) where T : new()
        {
            return dbio.Get<T>(query.Compile(), query.Parameters);
        }

        public static IEnumerable<T> Get<T>(this IDBIO dbio, IDBQuery query, IDictionary<string, object> parameters) where T : new()
        {
            return dbio.Get<T>(query.Compile(), parameters);
        }

        public static IEnumerable<T> Get<T>(this IDBIO dbio, IDBQuery query, Func<IDataRecord, T> buildDelegate)
        {
            return dbio.Get<T>(query.Compile(), query.Parameters, buildDelegate);
        }

        public static IEnumerable<T> Get<T>(this IDBIO dbio, IDBQuery query, IDictionary<string, object> parameters, Func<IDataRecord, T> buildDelegate)
        {
            return dbio.Get<T>(query.Compile(), parameters, buildDelegate);
        }



        public static IDictionary<string, object> Params(this IDBIO dbio)
        {
            return DictionaryExtension.New<string, object>();
        }
    }
}
