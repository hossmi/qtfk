using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTFK.Extensions.Collections.SwitchCase
{
    public class CaseCollection<T, TResult>
    {
        public IEnumerable<T> Items { get; set; }
        public ICollection<KeyValuePair<Func<T, bool>, Func<T, TResult>>> Cases { get; set; }
    }

    public static class SwitchCaseExtension
    {
        public static CaseCollection<T,TResult> Case<T, TResult>(
            this IEnumerable<T> items
            , Func<T, bool> condition
            , Func<T, TResult> selector
            )
        {
            return new CaseCollection<T, TResult>
            {
                Items = items,
                Cases = new List<KeyValuePair<Func<T, bool>, Func<T, TResult>>>()
                    .Push(new KeyValuePair<Func<T, bool>, Func<T, TResult>>(condition,selector)),
            };
        }

        public static CaseCollection<T, TResult> Case<T, TResult>(
            this CaseCollection<T, TResult> cases
            , Func<T, bool> condition
            , Func<T, TResult> selector
            )
        {
            cases.Cases.Add(new KeyValuePair<Func<T, bool>, Func<T, TResult>>(condition, selector));
            return cases;
        }

        public static IEnumerable<TResult> CaseEnd<T, TResult>(
            this CaseCollection<T, TResult> cases
            , int matches = 1
            )
        {
            foreach (T item in cases.Items)
            {
                int matchesFound = 0;
                foreach (var option in cases.Cases)
                {
                    if (option.Key(item))
                    {
                        ++matchesFound;
                        yield return option.Value(item);
                    }
                    if (matchesFound >= matches)
                        break;
                }
            }
        }
    }
}
