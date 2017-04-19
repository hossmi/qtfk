using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using QTFK.Extensions.Collections.Dictionaries;

namespace QTFK.Extensions.Tests
{
    [TestClass]
    public class DictionariesExtensionTests
    {
        [TestMethod]
        [TestCategory("Extensions")]
        public void DictionariesExtension_Tests()
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
            d.Get<decimal>("pepe", v => pepe = v);
            d.Get<string>("tronco", v => tronco = v);

            Assert.AreEqual(3.14m, pepe);
            Assert.AreEqual(@"uno que llega", tronco);

            d.Get<double>("notExist", v => doubleNumber = v);
            Assert.AreEqual(0.0, doubleNumber);

            d.Get<double>("notExist", 34.0, v => doubleNumber = v);
            Assert.AreEqual(34.0, doubleNumber);

            try
            {
                d.Get<int>("tronco", 55, v => intNumber = v);
                Assert.Fail("It must fail on wrong data type?");
            }
            catch 
            {
            }
        }
    }
}
