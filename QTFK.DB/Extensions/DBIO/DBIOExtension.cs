using QTFK.Extensions.Collections.Filters;
using QTFK.Extensions.DBCommand;
using QTFK.Extensions.Mapping.AutoMapping;
using QTFK.Models;
using QTFK.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace QTFK.Extensions.DBIO
{
    public static class DBIOExtension
    {
        private static IEnumerable<KeyValuePair<string, object>> emptyParameterCollection = Enumerable.Empty<KeyValuePair<string, object>>();

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
            QueryCompilation queryCompilation;

            queryCompilation = query.Compile();

            return prv_set(dbio, queryCompilation.Query, queryCompilation.Parameters);
        }

        public static int Set(this IDBIO dbio, string query)
        {
            return prv_set(dbio, query, emptyParameterCollection);
        }

        public static int Set(this IDBIO dbio, string query, IEnumerable<KeyValuePair<string, object>> parameters)
        {
            return prv_set(dbio, query, parameters);
        }

        public static int Set(this IDBIO dbio, IEnumerable<string> queries, bool throwOnException = true)
        {
            return dbio.Set(cmd =>
            {
                Result<int> queryExecutionResult;
                int affectedRows;

                affectedRows = 0;

                foreach (string query in queries.NotEmpty())
                {
                    cmd.CommandText = query;
                    queryExecutionResult = new Result<int>(cmd.ExecuteNonQuery);

                    if(queryExecutionResult.Ok)
                        affectedRows += queryExecutionResult.Value;
                    else if(throwOnException)
                        throw queryExecutionResult.Exception;
                }

                return affectedRows;
            });
        }

        public static DataSet Get(this IDBIO dbio, string query)
        {
            return dbio.Get(query, emptyParameterCollection);
        }

        public static IEnumerable<T> Get<T>(this IDBIO dbio, string query, Func<IDataRecord, T> buildDelegate)
        {
            return prv_get<T>(dbio, query, emptyParameterCollection, buildDelegate);
        }

        public static IEnumerable<T> Get<T>(this IDBIO dbio, string query) where T : new()
        {
            return prv_get<T>(dbio, query, emptyParameterCollection, AutoMapExtension.AutoMap<T>);
        }

        public static IEnumerable<T> Get<T>(this IDBIO dbio, string query, IEnumerable<KeyValuePair<string, object>> parameters) where T : new()
        {
            return prv_get<T>(dbio, query, parameters, AutoMapExtension.AutoMap<T>);
        }

        public static IEnumerable<T> Get<T>(this IDBIO dbio, IDBQuery query) where T : new()
        {
            return prv_get<T>(dbio, query, AutoMapExtension.AutoMap<T>);
        }

        public static IEnumerable<T> Get<T>(this IDBIO dbio, IDBQuery query, Func<IDataRecord, T> buildDelegate)
        {
            return prv_get<T>(dbio, query, buildDelegate);
        }

        private static IEnumerable<T> prv_get<T>(IDBIO dbio, IDBQuery query, Func<IDataRecord, T> buildDelegate)
        {
            QueryCompilation queryCompilation;

            queryCompilation = query.Compile();

            return prv_get<T>(dbio, queryCompilation.Query, queryCompilation.Parameters, buildDelegate);
        }

        private static IEnumerable<T> prv_get<T>(IDBIO dbio, string query, IEnumerable<KeyValuePair<string, object>> parameters, Func<IDataRecord, T> buildDelegate)
        {
            return dbio.Get<T>(query, parameters, buildDelegate);
        }

        private static int prv_set(IDBIO dbio, string query, IEnumerable<KeyValuePair<string, object>> parameters)
        {
            return dbio.Set(cmd =>
            {
                cmd.CommandText = query;
                cmd.addParameters(parameters);

                return cmd.ExecuteNonQuery();
            });
        }
    }
}
