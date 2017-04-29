using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTFK.Services.CRUDRepositories
{
    public abstract class InMemoryCRUDRepositoryBase<TKey, TItem> : ICRUDRepository<TKey, TItem> where TItem : new()
    {
        protected readonly IDictionary<TKey, TItem> _items;

        public bool UsesAutoID { get; protected set; }

        public InMemoryCRUDRepositoryBase(
            IDictionary<TKey, TItem> items
            )
        {
            _items = items;
        }

        public abstract TKey GetItemID(TItem item);
        internal abstract TKey GenerateNewID(TItem item);
        internal abstract bool ItemHasID(TItem item);
        internal abstract void Map(TItem source, TItem target);

        public TKey Add(TItem item)
        {
            TKey id;
            if (UsesAutoID)
            {
                if (ItemHasID(item))
                    throw new ArgumentException(nameof(item), $"Error adding item because repository uses AutoID and {nameof(item)} has already ID");
                id = GenerateNewID(item);
            }
            else
            {
                if (!ItemHasID(item))
                    throw new ArgumentException(nameof(item), $"Error adding item because repository no uses AutoID and {nameof(item)} has no ID");
                id = GetItemID(item);
            }
            TItem itemDB = Clone(item);
            _items.Add(id, itemDB);

            return id;
        }

        public TItem Get(TKey id)
        {
            return _items.ContainsKey(id)
                ? _items[id]
                : default(TItem)
                ;
        }

        TItem Clone(TItem item)
        {
            TItem x = new TItem();
            Map(item, x);
            return x;
        }

        public IQueryable<TItem> Get(Func<TItem, bool> filter)
        {
            return _items
                .Values
                .Where(filter)
                .Select(Clone)
                .AsQueryable()
                ;
        }

        public bool Remove(TKey id)
        {
            return _items.Remove(id);
        }

        public bool Set(TItem item)
        {
            if (!ItemHasID(item))
                throw new ArgumentNullException(nameof(item), $"Error updating repository because {nameof(item)} has no ID");

            TKey id = GetItemID(item);
            if (!_items.ContainsKey(id))
                return false;

            TItem itemDB = _items[id];
            Map(item, itemDB);
            return true;
        }
    }
}
