using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTFK.Services
{
    public interface IExplicitKeyRepository<TKey, TItem> where TItem : new()
    {
        IQueryable<TItem> Get(Func<TItem, bool> filter);
        TItem Get(TKey id);
        TKey Add(TItem item);
        bool Set(TItem item);
        bool Remove(TKey id);
    }
}
