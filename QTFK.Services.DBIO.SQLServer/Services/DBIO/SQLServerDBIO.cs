using QTFK.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using QTFK.Models;
using QTFK.Extensions.DBCommand;
using QTFK.Extensions.DataReader;
using QTFK.Services.Loggers;

namespace QTFK.Services.DBIO
{
    public class SQLServerDBIO : IDBIO, ISQLServer
    {
        private const string ERROR_MESSAGE_DEFAULT = "Error ocurred executing SQL instructions";
        private const string ERROR_MESSAGE_GETTING_ID = "Error ocurred getting las ID";

        protected readonly string connectionString;
        protected readonly ILogger<LogLevel> log;

        public SQLServerDBIO(string connectionString)
            : this(connectionString, NullLogger.Instance)
        {
        }

        public SQLServerDBIO(
            string connectionString
            , ILogger<LogLevel> log
            )
        {
            Asserts.isSomething(this.log, "Argument 'log' cannot be null.");
            Asserts.isFilled(this.connectionString, "Argument 'connectionString' cannot be empty");

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

            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlDataAdapter da = new SqlDataAdapter(query, conn))
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
            SqlTransaction trans;
            IDataReader reader;

            Asserts.isFilled(query, "Argument 'query' cannot be empty.");
            Asserts.isSomething(parameters, "Argument 'parameters' cannot be null.");
            Asserts.isSomething(buildDelegate, "Argument 'buildDelegate' cannot be null.");

            result = null;

            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand command = new SqlCommand() { Connection = conn })
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
            SqlTransaction trans ;

            Asserts.isSomething(instructions, "Argument 'instructions' cannot be null.");

            affectedRows = 0;

            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand command = new SqlCommand() { Connection = conn })
            {
                trans = null;

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
