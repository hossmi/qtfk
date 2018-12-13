using QTFK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTFK.Services
{
    public interface IQueryExecutor
    {
        IEnumerable<T> select<T>(Action<IDBQuerySelect> queryBuild) where T : new();
        int insert(Action<IDBQueryInsert> queryBuild);
        int update(Action<IDBQueryUpdate> queryBuild);
        int delete(Action<IDBQueryDelete> queryBuild);
    }
}
