using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTFK.Extensions.Collections
{
    public static class CollectionExtension
    {
        public static ICollection<T> Push<T>(this ICollection<T> items, T item)
        {
            items.Add(item);
            return items;
        }

        public static IEnumerable<T> apply<T>(this IEnumerable<T> items, Action<T> actionDelegate) where T : class
        {
            foreach (T item in items)
            {
                actionDelegate(item);
                yield return item;
            }
        }
    }
}
