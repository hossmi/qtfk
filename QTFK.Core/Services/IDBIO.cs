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
        int Set(Func<IDbCommand, int> instruction);
        IEnumerable<T> Get<T>(string query, Func<IDataReader,T> build);
        DataSet Get(string query);
    }
}
