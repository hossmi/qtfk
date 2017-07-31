using QTFK.Services;
using QTFK.Services.ExplicitKeyRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTFK.Extensions.Collections.Repositories
{
    public static class ExplicitKeyRepositoryExtension
    {
        public static IQueryable<TItem> Get<TKey, TItem>(this IExplicitKeyRepository<TKey, TItem> repository) where TItem : new()
        {
            return repository.Get(item => true);
        }

        public static TItem Get<TKey, TItem>(this InMemoryRepositoryBase<TKey, TItem> repository, TItem item) where TItem : new()
        {
            return repository.Get(repository.GetItemID(item));
        }

        public static bool Remove<TKey, TItem>(this InMemoryRepositoryBase<TKey, TItem> repository, TItem item) where TItem : new()
        {
            return repository.Remove(repository.GetItemID(item));
        }
    }
}
