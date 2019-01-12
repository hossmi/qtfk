using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using QTFK.Models;
using QTFK.Extensions.DBCommand;
using QTFK.Extensions.DataReader;

namespace QTFK.Services.DBIO
{
    public abstract class AbstractDBIO<TConnection, TCommand, TDataAdapter> : IDBIO
        where TConnection : IDbConnection
        where TCommand : IDbCommand
        where TDataAdapter : IDbDataAdapter, IDisposable
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        protected static DBIOException prv_wrapException(IDbCommand cmd, Exception ex, string subject)
        {
            return prv_wrapException(cmd?.CommandText ?? "", ex, subject);
        }

        protected static DBIOException prv_wrapException(string query, Exception ex, string subject)
        {
            string message = $@"{subject}:
Exception: {ex.GetType().Name}
Message: {ex.Message}
Command: '{query}'";

            logger.Error(message);

            return new DBIOException(message, ex);
        }

        protected const string ERROR_MESSAGE_DEFAULT = "Error ocurred executing SQL instructions";
        protected const string ERROR_MESSAGE_GETTING_ID = "Error ocurred getting las ID";

        protected readonly string connectionString;

        public AbstractDBIO(string connectionString)
        {
            Asserts.stringIsNotEmpty(connectionString);

            this.connectionString = connectionString;
        }

        protected abstract TConnection prv_buildConnection(string connectionString);
        protected abstract TDataAdapter prv_buildDataAdapter(string query, TConnection connection);
        protected abstract TCommand prv_buildCommand(TConnection connection);

        public virtual void Dispose()
        {
        }

        public virtual DataSet Get(string query, IEnumerable<KeyValuePair<string, object>> parameters)
        {
            DataSet ds;

            Asserts.stringIsNotEmpty(query);
            Asserts.isNotNull(parameters);

            ds = null;

            using (TConnection conn = prv_buildConnection(this.connectionString))
            using (TDataAdapter da = prv_buildDataAdapter(query, conn))
            {
                try
                {
                    da.SelectCommand.addParameters(parameters);
                    ds = new DataSet();
                    da.Fill(ds);
                }
                catch (Exception ex)
                {
                    throw prv_wrapException(query, ex, ERROR_MESSAGE_DEFAULT);
                }
                finally
                {
                    conn?.Close();
                }
            }

            Asserts.isNotNull(ds);
            return ds;
        }

        public virtual IEnumerable<T> Get<T>(string query, IEnumerable<KeyValuePair<string, object>> parameters, Func<IDataRecord, T> buildDelegate)
        {
            IEnumerable<T> result;
            IDbTransaction trans;
            IDataReader reader;

            Asserts.stringIsNotEmpty(query);
            Asserts.isNotNull(parameters);
            Asserts.isNotNull(buildDelegate);

            result = null;

            using (TConnection conn = prv_buildConnection(this.connectionString))
            using (TCommand command = prv_buildCommand(conn))
            {
                trans = null;
                reader = null;

                try
                {
                    conn.Open();
                    trans = conn.BeginTransaction();

                    command.Transaction = trans;
                    command.CommandText = query;
                    command.addParameters(parameters);

                    reader = command.ExecuteReader();
                    result = reader
                        .GetRecords()
                        .Select(buildDelegate)
                        .ToList()
                        ;
                }
                catch (Exception ex)
                {
                    throw prv_wrapException(command, ex, ERROR_MESSAGE_DEFAULT);
                }
                finally
                {
                    if (reader != null && !reader.IsClosed)
                        reader.Close();
                    trans?.Rollback();
                    conn?.Close();
                }
            }

            Asserts.isNotNull(result);
            return result;
        }

        public virtual object GetLastID(IDbCommand cmd)
        {
            try
            {
                return cmd
                    .setCommandText(" SELECT @@IDENTITY ")
                    .clearParameters()
                    .ExecuteScalar()
                    ;
            }
            catch (Exception ex)
            {
                throw prv_wrapException(cmd, ex, ERROR_MESSAGE_GETTING_ID);
            }
        }

        public virtual int Set(Func<IDbCommand, int> instructions)
        {
            int affectedRows;
            IDbTransaction trans;

            Asserts.isNotNull(instructions);

            affectedRows = 0;

            using (TConnection conn = prv_buildConnection(this.connectionString))
            using (TCommand command = prv_buildCommand(conn))
            {
                trans = null;

                try
                {
                    conn.Open();
                    trans = conn.BeginTransaction();
                    command.Transaction = trans;
                    affectedRows = instructions(command);
                    if(trans.Connection != null)
                        trans.Commit();
                }
                catch (Exception ex)
                {
                    trans?.Rollback();
                    throw prv_wrapException(command, ex, ERROR_MESSAGE_DEFAULT);
                }
                finally
                {
                    conn?.Close();
                }
            }
            return affectedRows;
        }

        public T GetScalar<T>(string query, IEnumerable<KeyValuePair<string, object>> parameters) where T : struct
        {
            T value;

            Asserts.stringIsNotEmpty(query);
            Asserts.isNotNull(parameters);

            value = default(T);

            using (TConnection conn = prv_buildConnection(this.connectionString))
            using (TCommand cmd = prv_buildCommand(conn))
            {
                try
                {
                    conn.Open();
                    value = (T)cmd
                        .setCommandText(query)
                        .addParameters(parameters)
                        .ExecuteScalar();
                }
                catch (Exception ex)
                {
                    throw prv_wrapException(query, ex, ERROR_MESSAGE_DEFAULT);
                }
                finally
                {
                    conn?.Close();
                }
            }

            return value;
        }
    }
}
