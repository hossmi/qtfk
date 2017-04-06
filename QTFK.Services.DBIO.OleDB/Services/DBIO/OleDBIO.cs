using QTFK.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using QTFK.Models;
using QTFK.Services.Loggers;

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
            throw new NotImplementedException();
        }

        public IEnumerable<T> Get<T>(string query, IDictionary<string, object> parameters, Func<IDataRecord, T> buildDelegate)
        {
            throw new NotImplementedException();
        }

        public object GetLastID(IDbCommand cmd)
        {
            throw new NotImplementedException();
        }

        public int Set(Func<IDbCommand, int> instructions)
        {
            throw new NotImplementedException();
        }
    }
}
