using System;
using System.Collections.Generic;
using System.Linq;

namespace QTFK.Extensions.Collections
{
    public static class EnumerableExtension
    {
        public static ICollection<T> push<T>(this ICollection<T> items, T item)
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

        public static IEnumerable<T> notNull<T>(this IEnumerable<T> collection)
        {
            return collection.Where(i => i != null);
        }

        public static IEnumerable<string> notEmpty(this IEnumerable<string> collection)
        {
            return collection.Where(i => !string.IsNullOrWhiteSpace(i));
        }

        public static IEnumerable<string> trim(this IEnumerable<string> collection)
        {
            return collection.Select(i => i.Trim());
        }
    }
}
