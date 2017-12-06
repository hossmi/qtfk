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
    public class SQLServerDBIO : AbstractDBIO<SqlConnection, SqlCommand, SqlDataAdapter>, IDBIO, ISQLServer
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
