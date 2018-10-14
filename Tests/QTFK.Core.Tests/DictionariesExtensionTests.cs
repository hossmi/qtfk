using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using QTFK.Extensions.Collections.Dictionaries;

namespace QTFK.Core.Tests
{
    [TestClass]
    public class DictionariesExtensionTests
    {
        [TestMethod]
        [TestCategory("Extensions")]
        public void DictionariesExtension_Tests_1()
        {
            IDictionary<string, object> d;

            d = new Dictionary<string, object>
            {
                { "pepe", 3.14m },
                { "tronco", "uno que llega" },
            };

            string tronco = "";
            decimal pepe = 0;
            double doubleNumber = 3;
            double intNumber = 0;

            pepe = d.get<decimal>("pepe");
            tronco = d.get<string>("tronco");

            Assert.AreEqual(3.14m, pepe);
            Assert.AreEqual(@"uno que llega", tronco);

            doubleNumber = d.get<double>("notExist");
            Assert.AreEqual(0.0, doubleNumber);

            doubleNumber = d.get<double>("notExist", 34.0);
            Assert.AreEqual(34.0, doubleNumber);

            try
            {
                intNumber = d.get<int>("tronco", 55);
                Assert.Fail("It must fail on wrong data type?");
            }
            catch
            {
            }
        }

        [TestMethod]
        [TestCategory("Extensions")]
        public void DictionariesExtension_Tests_3()
        {
            var d = new Dictionary<string, object>
            {
                { "pepe", 3.14m },
                { "tronco", "uno que llega" },
            };

            string tronco = "";
            decimal pepe = 0;
            double doubleNumber = 3;
            double intNumber = 0;

            pepe = d.get<decimal>("pepe");
            tronco = d.get<string>("tronco");

            Assert.AreEqual(3.14m, pepe);
            Assert.AreEqual(@"uno que llega", tronco);

            doubleNumber = d.get<double>("notExist");
            Assert.AreEqual(0.0, doubleNumber);

            doubleNumber = d.get<double>("notExist", 34.0);
            Assert.AreEqual(34.0, doubleNumber);

            try
            {
                intNumber = d.get<int>("tronco", 55);
                Assert.Fail("It must fail on wrong data type?");
            }
            catch
            {
            }
        }

        [TestMethod]
        [TestCategory("Extensions")]
        public void DictionariesExtension_Tests_5()
        {
            IDictionary<string, object> d;

            d = new Dictionary<string, object>
            {
                { "pepe", 3.14m },
                { "tronco", "uno que llega" },
            };

            string tronco = "";
            decimal pepe = 0;
            double doubleNumber = 3;
            double intNumber = 0;

            pepe = d.get<decimal>("pepe");
            tronco = d.get<string>("tronco");

            Assert.AreEqual(3.14m, pepe);
            Assert.AreEqual(@"uno que llega", tronco);

            doubleNumber = d.get<double>("notExist");
            Assert.AreEqual(0.0, doubleNumber);

            doubleNumber = d.get<double>("notExist", 34.0);
            Assert.AreEqual(34.0, doubleNumber);

            try
            {
                intNumber = d.get<int>("tronco", 55);
                Assert.Fail("It must fail on wrong data type?");
            }
            catch
            {
            }
        }

        [TestMethod]
        [TestCategory("Extensions")]
        public void DictionariesExtension_Tests_6()
        {
            IDictionary<string, object> d;

            d = new Dictionary<string, object>
            {
                { "pepe", 3.14m },
                { "tronco", "uno que llega" },
            };

            string tronco = "";
            decimal pepe = 0;
            double doubleNumber = 3;
            double intNumber = 0;

            pepe = d.get<decimal>("pepe");
            tronco = d.get<string>("tronco");

            Assert.AreEqual(3.14m, pepe);
            Assert.AreEqual(@"uno que llega", tronco);

            doubleNumber = d.get<double>("notExist");
            Assert.AreEqual(0.0, doubleNumber);

            doubleNumber = d.get<double>("notExist", 34.0);
            Assert.AreEqual(34.0, doubleNumber);

            try
            {
                intNumber = d.get<int>("tronco", 55);
                Assert.Fail("It must fail on wrong data type?");
            }
            catch
            {
            }
        }
    }
}
