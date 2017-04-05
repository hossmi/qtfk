using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTFK.Services
{
    public interface IDBIO
    {
        int Set(Func<IDbCommand, int> instructions);
        IEnumerable<T> Get<T>(string query, IDictionary<string, object> parameters, Func<IDataReader,T> build);
        DataSet Get(string query, IDictionary<string, object> parameters);
        object GetLastID(IDbCommand cmd);
    }
}
