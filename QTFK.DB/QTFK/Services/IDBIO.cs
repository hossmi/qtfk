using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTFK.Services
{
    public interface IDBIO : IDisposable
    {
        int Set(Func<IDbCommand, int> instructions);
        IEnumerable<T> Get<T>(string query, IEnumerable<KeyValuePair<string, object>> parameters, Func<IDataRecord, T> buildDelegate);
        DataSet Get(string query, IEnumerable<KeyValuePair<string, object>> parameters);
        object GetLastID(IDbCommand cmd);
        T GetScalar<T>(string query, IEnumerable<KeyValuePair<string, object>> parameters) where T: struct;
    }
}
