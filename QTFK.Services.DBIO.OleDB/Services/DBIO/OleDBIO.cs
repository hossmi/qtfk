using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using QTFK.Models;
using System.Data.OleDb;
using QTFK.Extensions.DBCommand;
using QTFK.Extensions.DataReader;
using QTFK.Services.Loggers;

namespace QTFK.Services.DBIO
{
    public class OleDBIO : IDBIO, IOleDB
    {
        private const string ERROR_MESSAGE_DEFAULT = "Error ocurred executing SQL instructions";
        private const string ERROR_MESSAGE_GETTING_ID = "Error ocurred getting las ID";

        protected readonly string connectionString;
        protected readonly ILogger<LogLevel> log;

        public OleDBIO(string connectionString)
            : this(connectionString, NullLogger.Instance)
        {
        }

        public OleDBIO(
            string connectionString
            , ILogger<LogLevel> log
            )
        {
            Asserts.isSomething(log, "Argument 'log' cannot be null.");
            Asserts.isFilled(connectionString, "Argument 'connectionString' cannot be empty");

            this.connectionString = connectionString;
            this.log = log;
        }

        public void Dispose()
        {
        }

        public DataSet Get(string query, IDictionary<string, object> parameters)
        {
            DataSet ds;

            Asserts.isFilled(query, "Argument 'query' cannot be empty.");
            Asserts.isSomething(parameters, "Argument 'parameters' cannot be null.");

            ds = null;

            using (OleDbConnection conn = new OleDbConnection(this.connectionString))
            using (OleDbDataAdapter da = new OleDbDataAdapter(query, conn))
            {
                try
                {
                    da.SelectCommand.AddParameters(parameters);
                    ds = new DataSet();
                    da.Fill(ds);
                }
                catch (Exception ex)
                {
                    throw prv_wrapException(query, ex, this.log, ERROR_MESSAGE_DEFAULT);
                }
                finally
                {
                    conn?.Close();
                }
            }

            Asserts.isSomething(ds, "Result DataSet is null!");
            return ds;
        }

        public IEnumerable<T> Get<T>(string query, IDictionary<string, object> parameters, Func<IDataRecord, T> buildDelegate)
        {
            IEnumerable<T> result;
            OleDbTransaction trans;
            IDataReader reader;

            Asserts.isFilled(query, "Argument 'query' cannot be empty.");
            Asserts.isSomething(parameters, "Argument 'parameters' cannot be null.");
            Asserts.isSomething(buildDelegate, "Argument 'buildDelegate' cannot be null.");

            result = null;

            using (OleDbConnection conn = new OleDbConnection(this.connectionString))
            using (OleDbCommand command = new OleDbCommand() { Connection = conn })
            {
                trans = null;
                reader = null;

                try
                {
                    conn.Open();
                    trans = conn.BeginTransaction();
                    command.Transaction = trans;
                    command.CommandText = query;
                    command.AddParameters(parameters);

                    reader = command.ExecuteReader();
                    result = reader
                        .GetRecords()
                        .Select(buildDelegate)
                        .ToList()
                        ;
                }
                catch (Exception ex)
                {
                    throw prv_wrapException(command, ex, this.log, ERROR_MESSAGE_DEFAULT);
                }
                finally
                {
                    if (reader != null && !reader.IsClosed)
                        reader.Close();
                    trans?.Rollback();
                    conn?.Close();
                }
            }

            Asserts.isSomething(result, $"Output '{nameof(IEnumerable<T>)}' is null!");
            return result;
        }

        public object GetLastID(IDbCommand cmd)
        {
            try
            {
                return cmd
                    .SetCommandText(" SELECT @@IDENTITY ")
                    .ClearParameters()
                    .ExecuteScalar()
                    ;
            }
            catch (Exception ex)
            {
                throw prv_wrapException(cmd, ex, this.log, ERROR_MESSAGE_GETTING_ID);
            }
        }

        public int Set(Func<IDbCommand, int> instructions)
        {
            int affectedRows;
            OleDbTransaction trans;

            Asserts.isSomething(instructions, "Argument 'instructions' cannot be null.");

            affectedRows = 0;
            trans = null;

            using (OleDbConnection conn = new OleDbConnection(this.connectionString))
            using (OleDbCommand command = new OleDbCommand() { Connection = conn })
            {
                try
                {
                    conn.Open();
                    trans = conn.BeginTransaction();
                    command.Transaction = trans;
                    affectedRows = instructions(command);
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans?.Rollback();
                    throw prv_wrapException(command, ex, this.log, ERROR_MESSAGE_DEFAULT);
                }
                finally
                {
                    conn?.Close();
                }
            }

            return affectedRows;
        }

        private static DBIOException prv_wrapException(IDbCommand cmd, Exception ex, ILogger<LogLevel> logger, string subject)
        {
            return prv_wrapException(cmd?.CommandText ?? "", ex, logger, subject);
        }

        private static DBIOException prv_wrapException(string query, Exception ex, ILogger<LogLevel> logger, string subject)
        {
            string message = $@"{subject}:
Exception: {ex.GetType().Name}
Message: {ex.Message}
Command: '{query}'";

            logger.log(LogLevel.Error, message);

            return new DBIOException(message, ex);
        }
    }
}
