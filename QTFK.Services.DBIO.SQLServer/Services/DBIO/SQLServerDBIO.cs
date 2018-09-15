using System.Data.SqlClient;
using QTFK.Models;
using QTFK.Attributes;

namespace QTFK.Services.DBIO
{
    [SqlServer]
    public class SQLServerDBIO : AbstractDBIO<SqlConnection, SqlCommand, SqlDataAdapter>, ISqlServerDBIO
    {
        public SQLServerDBIO(string connectionString) 
            : base(connectionString)
        {
        }

        public SQLServerDBIO(string connectionString, ILogger<LogLevel> log) 
            : base(connectionString, log)
        {
        }

        protected override SqlCommand prv_buildCommand(SqlConnection connection)
        {
            return new SqlCommand()
            {
                Connection = connection
            };
        }

        protected override SqlConnection prv_buildConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }

        protected override SqlDataAdapter prv_buildDataAdapter(string query, SqlConnection connection)
        {
            return new SqlDataAdapter(query, connection);
        }
    }
}
