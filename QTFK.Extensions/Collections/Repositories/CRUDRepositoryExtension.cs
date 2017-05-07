using QTFK.Services;
using QTFK.Services.CRUDRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTFK.Extensions.Collections.Repositories
{
    public static class CRUDRepositoryExtension
    {
        public static IQueryable<TItem> Get<TKey, TItem>(this ICRUDRepository<TKey, TItem> repository) where TItem : new()
        {
            return repository.Get(item => true);
        }

        public static TItem Get<TKey, TItem>(this InMemoryCRUDRepositoryBase<TKey, TItem> repository, TItem item) where TItem : new()
        {
            return repository.Get(repository.GetItemID(item));
        }

        public static bool Remove<TKey, TItem>(this InMemoryCRUDRepositoryBase<TKey, TItem> repository, TItem item) where TItem : new()
        {
            return repository.Remove(repository.GetItemID(item));
        }
    }
}
