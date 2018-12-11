using System;
using System.Collections.Generic;

namespace QTFK.Extensions.Randoms
{
    public static class RandomExtensions
    {
        public static bool isTrue(this Random random)
        {
            return random.NextDouble() >= 0.5;
        }

        public static bool isTrue(this Random random, double threshold)
        {
            return random.NextDouble() >= (1.0 - threshold);
        }

        public static T next<T>(this Random random, IList<T> items)
        {
            int count, r;

            count = items.Count;
            r = random.Next(0, count);

            return items[r];
        }

        public static void shuffle<T>(this Random random, T[] items)
        {
            prv_shuffle<T>(random, items);
        }

        public static T[] shuffle<T>(this T[] items, Random random)
        {
            prv_shuffle<T>(random, items);
            return items;
        }

        private static void prv_shuffle<T>(Random random, T[] items)
        {
            int n = random.Next(0, items.Length);

            for (int i = 0; i < n; i++)
            {
                int x = random.Next(0, items.Length);
                int y = random.Next(0, items.Length);

                T item;

                item = items[x];
                items[x] = items[y];
                items[y] = item;
            }
        }
    }
}
