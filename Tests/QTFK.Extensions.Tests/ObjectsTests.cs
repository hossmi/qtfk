using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using System.Collections.Generic;
using QTFK.Extensions.Tests.Models;
using QTFK.Extensions.DataSets;
using QTFK.Services.EntityMappers;
using QTFK.Extensions.Objects.Factory;

namespace QTFK.Extensions.Tests
{
    [TestClass]
    public class FactoryTests
    {
        [TestMethod]
        [TestCategory("Factory")]
        public void GenericExtension_tests()
        {
            var source = Dummy.WithOnes();
            var copy = source.ShallowClone();

            Assert.AreEqual(1, copy.FullAccessProperty);
            Assert.AreEqual(0, copy.ReadOnlyProperty);
            Assert.AreEqual(0, copy._write);

            var createdObject = GenericFactory.New(typeof(Dummy));

            Assert.IsInstanceOfType(createdObject, typeof(Dummy));
        }

        internal class Dummy
        {
            public int FullAccessProperty { get; set; } = 0;
            public int ReadOnlyProperty { get; } = readOnlyValue;
            public int WriteOnlyProperty { set { _write = value; } }
            public int _write = 0;

            private static int readOnlyValue = 0;
            private static readonly Object locker = new Object();

            public Dummy() { }

            public static Dummy WithOnes()
            {
                Dummy result = null;
                lock (locker)
                {
                    readOnlyValue = 1;
                    result = new Dummy()
                    {
                        WriteOnlyProperty = 1,
                        FullAccessProperty = 1,
                    };
                    readOnlyValue = 0;
                }
                return result;
            }

            public static Dummy WithTwos()
            {
                Dummy result = null;
                lock (locker)
                {
                    readOnlyValue = 2;
                    result = new Dummy()
                    {
                        WriteOnlyProperty = 2,
                        FullAccessProperty = 2,
                    };
                    readOnlyValue = 0;
                }
                return result;
            }
        }
    }
}
