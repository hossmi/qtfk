using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTFK.Extensions.Collections.Filters
{
    public static class EnumerableFilterExtension
    {
        public static IEnumerable<T> NotNull<T>(this IEnumerable<T> collection)
        {
            return collection
                .Where(i => i != null)
                ;
        }

        public static IEnumerable<string> NotEmpty(this IEnumerable<string> collection)
        {
            return collection
                .Where(i => !string.IsNullOrWhiteSpace(i))
                ;
        }

        public static IEnumerable<string> Trim(this IEnumerable<string> collection)
        {
            return collection
                .Select(i => i.Trim())
                ;
        }
    }
}
