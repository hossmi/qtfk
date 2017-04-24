﻿using System;
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
        public void DictionariesExtension_Tests_1()
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

            d
                .Get<decimal>("pepe", v => pepe = v)
                .Get<string>("tronco", v => tronco = v)
                ;

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

        [TestMethod]
        [TestCategory("Extensions")]
        public void DictionariesExtension_Tests_2()
        {
            var d = new Dictionary<string, string>
            {
                { "pepe", "lorem ipsum dolor shit" },
                { "tronco", "uno que llega" },
            };

            string tronco = "", pepe = "";
            string notExists = "pepe";

            d
                .Get("pepe", v => pepe = v)
                .Get("tronco", v => tronco = v)
                ;

            Assert.AreEqual("lorem ipsum dolor shit", pepe);
            Assert.AreEqual(@"uno que llega", tronco);

            d.Get("notExist", v => notExists = v);
            Assert.AreEqual(string.Empty, notExists);
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

            pepe = d.Get<decimal>("pepe");
            tronco = d.Get<string>("tronco");

            Assert.AreEqual(3.14m, pepe);
            Assert.AreEqual(@"uno que llega", tronco);

            doubleNumber = d.Get<double>("notExist");
            Assert.AreEqual(0.0, doubleNumber);

            doubleNumber = d.Get<double>("notExist", 34.0);
            Assert.AreEqual(34.0, doubleNumber);

            try
            {
                intNumber = d.Get<int>("tronco", 55);
                Assert.Fail("It must fail on wrong data type?");
            }
            catch
            {
            }
        }

        [TestMethod]
        [TestCategory("Extensions")]
        public void DictionariesExtension_Tests_4()
        {
            var d = new Dictionary<string, string>
            {
                { "pepe", "lorem ipsum dolor shit" },
                { "tronco", "uno que llega" },
            };

            string tronco = "", pepe = "";
            string notExists = "pepe";

            pepe = d.Get("pepe");
            tronco = d.Get("tronco");

            Assert.AreEqual("lorem ipsum dolor shit", pepe);
            Assert.AreEqual(@"uno que llega", tronco);

            notExists = d.Get("notExist");
            Assert.AreEqual(string.Empty, notExists);
        }
    }
}
