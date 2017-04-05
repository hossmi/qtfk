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

namespace QTFK.Services.DBIO
{
    public class SQLServerDBIO : IDBIO
    {
        private readonly string _connectionString;

        public SQLServerDBIO(string connectionString)
        {
            _connectionString = connectionString;
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
                    conn.Close();
                }
            }
            return ds;
        }

        public IEnumerable<T> Get<T>(string query, IDictionary<string, object> parameters, Func<IDataReader, T> build)
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand command = new SqlCommand() { Connection = conn };
            conn.Open();
            command.AddParameters(parameters);
            command.CommandText = query;
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                T item = default(T);
                try
                {
                    item = build(reader);
                }
                catch (Exception ex)
                {
                    throw new DBIOException($@"Error ocurred building object from {nameof(IDataReader)}:
Exception: {ex.GetType().Name}
T: {typeof(T).Name}
Current command: {command?.CommandText ?? ""}", ex);
                }
                yield return item;
            }
            conn.Close();
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
                    conn.Close();
                }

            }
            return affectedRows;
        }
    }
}
