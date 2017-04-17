using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QTFK.Extensions.Objects.Manipulator;

namespace QTFK.Extensions.Tests
{
    [TestClass]
    public class ManipulateTests
    {
        [TestMethod]
        [TestCategory("Extensions")]
        public void ManipulateExtension_tests()
        {
            var pepe = new Models.SimpleTestClass
            {
                Name = "pepe",
                LastName = "tronco",
                BirthDate = new DateTime(2015, 10, 21, 16, 29, 0),
                DecimalNumber = 0.00034m,
            };

            pepe.Manipulate()
                .Set("Name", "Chiquito")
                .Set(nameof(Models.SimpleTestClass.DoubleNumber), Math.PI)
                ;

            decimal x = 4;
            Assert.AreEqual("Chiquito", pepe.Name);
            Assert.AreEqual(Math.PI, pepe.DoubleNumber);

            pepe.Manipulate()
                .Set("Name", "Chiquito")
                .Set(nameof(Models.SimpleTestClass.DoubleNumber), Math.PI)
                .Get(nameof(Models.SimpleTestClass.DecimalNumber), out x)
                ;

            Assert.AreEqual("Chiquito", pepe.Name);
            Assert.AreEqual(Math.PI, pepe.DoubleNumber);
            Assert.AreEqual(0.00034m, pepe.DecimalNumber);

            pepe.DecimalNumber = 101m;
            pepe.Manipulate(m => m
                .Set("Name", "George")
                .Set(nameof(Models.SimpleTestClass.DoubleNumber), Math.E)
                .Get(nameof(Models.SimpleTestClass.DecimalNumber), out x)
                );

            Assert.AreEqual("George", pepe.Name);
            Assert.AreEqual(Math.E, pepe.DoubleNumber);
            Assert.AreEqual(101m, pepe.DecimalNumber);

            pepe.Manipulate(m =>
            {
                m.Set(nameof(Models.SimpleTestClass.DecimalNumber), 666m);
                x = m.Get<decimal>(nameof(Models.SimpleTestClass.DecimalNumber));
            });

            Assert.AreEqual(666m, pepe.DecimalNumber);
        }
    }
}
