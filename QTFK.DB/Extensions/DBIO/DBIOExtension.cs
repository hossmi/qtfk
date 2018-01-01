using QTFK.Extensions.Collections.Filters;
using QTFK.Extensions.Collections.Dictionaries;
using QTFK.Extensions.DBCommand;
using QTFK.Extensions.Mapping.AutoMapping;
using QTFK.Models;
using QTFK.Services;
using System;
using System.Collections.Generic;
using System.Data;
using QTFK.Extensions.DBIO.DBQueries;

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
            return prv_set(dbio, query.Compile(), query.getUniqueParameters());
        }

        public static int Set(this IDBIO dbio, string query)
        {
            return prv_set(dbio, query, prv_buildParams());
        }

        public static int Set(this IDBIO dbio, string query, IDictionary<string, object> parameters)
        {
            return prv_set(dbio, query, parameters);
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
            return dbio.Get(query, prv_buildParams());
        }
        
        public static IEnumerable<T> Get<T>(this IDBIO dbio, string query, Func<IDataRecord, T> buildDelegate)
        {
            return prv_get<T>(dbio, query, prv_buildParams(), buildDelegate);
        }

        public static IEnumerable<T> Get<T>(this IDBIO dbio, string query) where T : new()
        {
            return prv_get<T>(dbio, query, prv_buildParams(), AutoMapExtension.AutoMap<T>);
        }

        public static IEnumerable<T> Get<T>(this IDBIO dbio, string query, IDictionary<string, object> parameters) where T : new()
        {
            return prv_get<T>(dbio, query, parameters, AutoMapExtension.AutoMap<T>);
        }
        
        public static IEnumerable<T> Get<T>(this IDBIO dbio, IDBQuery query) where T : new()
        {
            return prv_get<T>(dbio, query.Compile(), query.getUniqueParameters(), AutoMapExtension.AutoMap<T>);
        }

        public static IEnumerable<T> Get<T>(this IDBIO dbio, IDBQuery query, Func<IDataRecord, T> buildDelegate)
        {
            return prv_get<T>(dbio, query.Compile(), query.getUniqueParameters(), buildDelegate);
        }

        public static IDictionary<string, object> Params(this IDBIO dbio)
        {
            return prv_buildParams();
        }

        private static IDictionary<string, object> prv_buildParams()
        {
            return DictionaryExtension.New<string, object>();
        }

        private static IEnumerable<T> prv_get<T>(IDBIO dbio, string query, IDictionary<string, object> parameters, Func<IDataRecord, T> buildDelegate)
        {
            return dbio.Get<T>(query, parameters, buildDelegate);
        }

        private static int prv_set(IDBIO dbio, string query, IDictionary<string, object> parameters)
        {
            return dbio.Set(cmd =>
            {
                cmd.CommandText = query;
                cmd.AddParameters(parameters);
                return cmd.ExecuteNonQuery();
            });
        }
    }
}
