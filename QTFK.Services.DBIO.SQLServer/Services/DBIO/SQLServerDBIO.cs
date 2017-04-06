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
    public class SQLServerDBIO : IDBIO
    {
        private readonly string _connectionString;
        private readonly ILogger<LogLevel> _log;

        public SQLServerDBIO(
            string connectionString
            , ILogger<LogLevel> log = null
            )
        {
            _connectionString = connectionString;
            _log = log ?? NullLogger.Instance;
        }

        public void Dispose()
        {
        }

        public DataSet Get(string query, IDictionary<string, object> parameters)
        {
            DataSet ds = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlDataAdapter da = new SqlDataAdapter(query, conn))
            {
                da.SelectCommand.AddParameters(parameters);
                try
                {
                    ds = new DataSet();
                    da.Fill(ds);
                }
                catch (Exception ex)
                {
                    throw new DBIOException($@"Error ocurred executing SQL instructions:
Exception: {ex.GetType().Name}
Query: '{query ?? ""}'", ex);
                }
                finally
                {
                    conn?.Close();
                }
            }
            return ds;
        }

        public IEnumerable<T> Get<T>(string query, IDictionary<string, object> parameters, Func<IDataRecord, T> buildDelegate)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand() { Connection = conn })
            {
                SqlTransaction trans = null;
                try
                {
                    conn.Open();
                    trans = conn.BeginTransaction();
                    command.Transaction = trans;

                    command.CommandText = query;
                    command.AddParameters(parameters);

                    return command
                        .ExecuteReader()
                        .GetRecords()
                        .Select(buildDelegate)
                        .ToList()
                        ;
                }
                catch (Exception ex)
                {
                    throw new DBIOException($@"Error ocurred executing SQL instruction:
Exception: {ex.GetType().Name}
Current command: {command?.CommandText ?? ""}", ex);
                }
                finally
                {
                    trans?.Rollback();
                    conn?.Close();
                }
            }
        }

        public object GetLastID(IDbCommand cmd)
        {
            cmd.CommandText = " SELECT @@IDENTITY ";
            return cmd
                .ClearParameters()
                .ExecuteScalar();
        }

        public int Set(Func<IDbCommand, int> instructions)
        {
            int affectedRows = 0;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand() { Connection = conn })
            {
                SqlTransaction trans = null;
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

                    throw new DBIOException($@"Error ocurred executing SQL instructions:
Exception: {ex.GetType().Name}
Current command: {command?.CommandText ?? ""}", ex);
                }
                finally
                {
                    conn?.Close();
                }

            }
            return affectedRows;
        }
    }
}
