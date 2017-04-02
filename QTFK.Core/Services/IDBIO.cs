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
        T Get<T>(string query, IDataReader reader);
        DataSet Get(string query);
    }
}
