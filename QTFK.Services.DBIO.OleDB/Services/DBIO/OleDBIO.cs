using QTFK.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using QTFK.Models;
using QTFK.Services.Loggers;
using System.Data.OleDb;
using QTFK.Extensions.DBCommand;
using QTFK.Extensions.DataReader;

namespace QTFK.Services.DBIO
{
    public class OleDBIO : IDBIO
    {
        private readonly string _connectionString;
        private readonly ILogger<LogLevel> _log;

        public OleDBIO(
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
            using (OleDbConnection conn = new OleDbConnection(_connectionString))
            using (OleDbDataAdapter da = new OleDbDataAdapter(query, conn))
            {
                da.SelectCommand.AddParameters(parameters);
                try
                {
                    ds = new DataSet();
                    da.Fill(ds);
                }
                catch (Exception ex)
                {
                    string message = $@"Error ocurred executing SQL instructions:
Exception: {ex.GetType().Name}
Message: {ex.Message}
Query: '{query ?? ""}'";
                    _log.Log(LogLevel.Error, message);
                    throw new DBIOException(message, ex);
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
            using (OleDbConnection conn = new OleDbConnection(_connectionString))
            using (OleDbCommand command = new OleDbCommand() { Connection = conn })
            {
                OleDbTransaction trans = null;
                IDataReader reader = null;
                try
                {
                    conn.Open();
                    trans = conn.BeginTransaction();
                    command.Transaction = trans;

                    command.CommandText = query;
                    command.AddParameters(parameters);

                    reader = command.ExecuteReader();
                    return reader
                        .GetRecords()
                        .Select(buildDelegate)
                        .ToList()
                        ;
                }
                catch (Exception ex)
                {
                    string message = $@"Error ocurred executing SQL instruction:
Exception: {ex.GetType().Name}
Message: {ex.Message}
Current command: {command?.CommandText ?? ""}";
                    _log.Log(LogLevel.Error, message);
                    throw new DBIOException(message, ex);
                }
                finally
                {
                    if (reader != null && !reader.IsClosed)
                        reader.Close();
                    trans?.Rollback();
                    conn?.Close();
                }
            }
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
                string message = $@"Error ocurred getting las ID:
Exception: {ex.GetType().Name}
Message: {ex.Message}
Current command: {cmd?.CommandText ?? ""}";
                _log.Log(LogLevel.Error, message);
                throw new DBIOException(message, ex);
            }
        }

        public int Set(Func<IDbCommand, int> instructions)
        {
            int affectedRows = 0;
            using (OleDbConnection conn = new OleDbConnection(_connectionString))
            using (OleDbCommand command = new OleDbCommand() { Connection = conn })
            {
                OleDbTransaction trans = null;
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

                    string message = $@"Error ocurred executing SQL instructions:
Exception: {ex.GetType().Name}
Message: {ex.Message}
Current command: {command?.CommandText ?? ""}";
                    _log.Log(LogLevel.Error, message);
                    throw new DBIOException(message, ex);
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
