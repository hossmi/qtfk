using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTFK.Services.CRUDRepositories
{
    public class InMemoryLambdaCRUDRepository<TKey, TItem> : InMemoryCRUDRepositoryBase<TKey, TItem> where TItem : new()
    {
        private readonly Func<TItem, TKey> _generateNewID;
        private readonly Func<TItem, TKey> _getItemID;
        private readonly Func<TItem, bool> _itemHasID;
        private readonly Action<TItem, TItem> _map;

        public InMemoryLambdaCRUDRepository(
            IDictionary<TKey, TItem> items
            , Func<TItem, TKey> getItemID
            , Func<TItem, bool> itemHasID
            , Action<TItem, TItem> map
            , Func<TItem, TKey> generateNewID
            ) : base(items)
        {
            _getItemID = getItemID;
            _itemHasID = itemHasID;
            _map = map;

            UsesAutoID = true;
            _generateNewID = generateNewID;
        }

        public InMemoryLambdaCRUDRepository(
            IDictionary<TKey, TItem> items
            , Func<TItem, TKey> getItemID
            , Func<TItem, bool> itemHasID
            , Action<TItem, TItem> map
            ) : base(items)
        {
            _getItemID = getItemID;
            _itemHasID = itemHasID;
            _map = map;

            UsesAutoID = false;
            _generateNewID = item => {
                throw new TypeInitializationException(this.GetType().FullName
                    , new Exception($"This {nameof(InMemoryLambdaCRUDRepository<TKey, TItem>)} has been initialized without {nameof(GenerateNewID)} function, but it has been called. Possible bug? O__o"));
            };
        }

        internal override TKey GenerateNewID(TItem item)
        {
            return _generateNewID(item);
        }

        public override TKey GetItemID(TItem item)
        {
            return _getItemID(item);
        }

        internal override bool ItemHasID(TItem item)
        {
            return _itemHasID(item);
        }

        internal override void Map(TItem source, TItem target)
        {
            _map(source, target);
        }
    }
}
