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
    }
}
