using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using QTFK.Extensions.Collections.Strings;

namespace QTFK.Core.Tests
{
    [TestClass]
    public class StringExtensionTest
    {
        [TestMethod]
        [TestCategory("Extensions")]
        public void Stringify_collection()
        {
            var cadenas = new string[] { "uno que llega", @"lorem ipsum", null, "" };
            var enteros = new int[] { 1, 2, 3, 5, 8, 13 };

            string result = StringExtension.Stringify(cadenas, s => s);
            Assert.AreEqual(@"uno que llega, lorem ipsum", result);

            result = StringExtension.Stringify<double>(null, s => (s + 3).ToString());
            Assert.AreEqual(@"", result);

            result = StringExtension.Stringify(enteros, s => (-s * 10).ToString());
            Assert.AreEqual(@"-10, -20, -30, -50, -80, -130", result);

            result = StringExtension.Stringify(enteros, s => (-s * 10).ToString(), "|");
            Assert.AreEqual(@"-10|-20|-30|-50|-80|-130", result);

            result = StringExtension.Stringify(Enumerable.Empty<decimal>(), s => (-s * 10m).ToString());
            Assert.AreEqual(@"", result);

            result = StringExtension.Stringify(enteros, s => (-s * 10).ToString(), null);
            Assert.AreEqual(@"-10-20-30-50-80-130", result);
        }
    }
}
