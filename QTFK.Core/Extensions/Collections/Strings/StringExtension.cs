
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace QTFK.Extensions.Collections.Strings
{
    public static class StringExtension
    {
        public static string Stringify<T>(this IEnumerable<T> items, Func<T, string> selectorFunction, string separator = ", ")
        {
            return (items ?? Enumerable.Empty<T>())
                .Select(selectorFunction)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .DefaultIfEmpty(string.Empty)
                .Aggregate((acum, item) => $"{acum}{separator}{item}");
        }

        public static string Stringify(this IEnumerable<string> items, string separator = ", ")
        {
            return Stringify(items, item => item, separator);
        }

        public static string F(this string format, params object[] args)
        {
            return string.Format(format, args);
        }

        public static bool EqualsAccentInsensitive(this string s1, string s2)
        {
            return string.Compare(s1, s2, CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) == 0;
        }
    }
}
