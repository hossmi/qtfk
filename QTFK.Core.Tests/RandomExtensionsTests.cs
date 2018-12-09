using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QTFK.Extensions.Randoms;

namespace QTFK.Core.Tests
{
    [TestClass]
    public class RandomExtensionsTests
    {
        private static readonly Random r;
        private static readonly TimeSpan TIMEUP;

        static RandomExtensionsTests()
        {
            r = new Random((int)(DateTime.UtcNow.Ticks % int.MaxValue));
            TIMEUP = new TimeSpan(0, 1, 0);
        }

        [TestMethod]
        [TestCategory("One minute execution")]
        public void isTrue_returns_true_50_percent_of_times_after_1_minute_of_execution()
        {
            const double precision = 0.001;
            Stopwatch stopwatch;
            long trues, falses;
            double proportion;
            bool isInRange;

            stopwatch = Stopwatch.StartNew();

            trues = 0;
            falses = 0;
            for (long i = 0; stopwatch.Elapsed < TIMEUP; i++)
            {
                if (r.isTrue())
                    trues++;
                else
                    falses++;
            }

            proportion = (double)trues / (double)falses;
            isInRange = (1 - precision) <= proportion && proportion <= (1 + precision);
            Assert.IsTrue(isInRange);
        }

        [TestMethod]
        [TestCategory("One minute execution")]
        public void isTrue_returns_true_specified_percent_of_times_after_1_minute_of_execution()
        {
            const double PRECISION = 0.001;
            const double TREND = 0.75;
            Stopwatch stopwatch;
            long trues, falses;
            double proportion;
            bool isInRange;

            stopwatch = Stopwatch.StartNew();

            trues = 0;
            falses = 0;
            for (long i = 0; stopwatch.Elapsed < TIMEUP; i++)
            {
                if (r.isTrue(TREND))
                    trues++;
                else
                    falses++;
            }

            proportion = (double)trues / (double)(trues + falses);
            isInRange = (TREND - PRECISION) <= proportion && proportion <= (TREND + PRECISION);
            Assert.IsTrue(isInRange);
        }

        [TestMethod]
        [TestCategory("One minute execution")]
        public void next_for_randoms_returns_each_element_equally_after_1_minute_of_execution()
        {
            const double PRECISION = 0.001;

            string[] names = new string[]
            {
                "pepe",
                "tronco",
                "Rosa",
                "Irene",
                "Lucas",
                "Narciso",
            };

            IDictionary<string, int> choosenNames = new Dictionary<string, int>();
            Stopwatch stopwatch = Stopwatch.StartNew();

            foreach (string name in names)
                choosenNames.Add(name, 0);

            for (long i = 0; stopwatch.Elapsed < TIMEUP; i++)
            {
                string name;

                name = r.next<string>(names);
                choosenNames[name] += 1;
            }

            double[] namesCounts = choosenNames.Values
                .Select(v => (double)v)
                .ToArray();

            double average = namesCounts.Average();

            double[] deviations = namesCounts
                .Select(v => Math.Abs(v - average) / average)
                .ToArray();

            foreach (double deviation in deviations)
                Assert.IsTrue(deviation < PRECISION);
        }
    }
}
