using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QTFK.Extensions.Tests.Models;
using QTFK.Extensions.Collections.Casting;

namespace QTFK.Extensions.Tests
{
    [TestClass]
    public class CastExtensionTest
    {
        [TestMethod]
        public void CastExtension_tests()
        {
            var simple = new SimpleTestClass
            {
                Name = "pepe",
                BirthDate = DateTime.Today,
            };

            var inherited = new InheritedTestClass
            {
                Name = "Tronco",
                BirthDate = new DateTime(2015, 10, 21),
                NullableInt = 4
            };

            var simple1 = simple.As<InheritedTestClass>();
            Assert.IsNull(simple1);

            var inherited1 = inherited.As<InheritedTestClass>();
            Assert.IsNotNull(inherited1);
            Assert.IsTrue(object.ReferenceEquals(inherited, inherited1));

            var inherited2 = inherited.As<SimpleTestClass>();
            Assert.IsNotNull(inherited2);
            Assert.IsTrue(object.ReferenceEquals(inherited, inherited2));
        }
    }
}
