using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QTFK.Models;

namespace QTFK.Core.Tests
{
    enum EnumSample
    {
        valueMin = 5,
        valueMed = 8,
        valueMax = 13,
    }

    [TestClass]
    public class AssertTests
    {
        [TestCategory("Asserts")]
        [TestMethod]
        public void Asserting_good_values()
        {
            string someString, someEnumSampleAsString;
            int number;
            Type someType;
            EnumSample someEnumSample;
            object someObject;

            someString = "pepe";
            number = 5;
            someType = typeof(AssertTests);
            someObject = someType;

            someEnumSample = EnumSample.valueMin;
            someEnumSampleAsString = EnumSample.valueMax.ToString();

            Asserts.isFilled(someString, $"'{nameof(someString)}' cannot be empty.");
            Asserts.check(number > 0, $"'{nameof(number)}' must be greater than zero.");
            Asserts.isSomething(someType, "'{nameof(number)}' cannot be null.");
            Asserts.isSomething(someObject, "'{nameof(number)}' cannot be null.");
            Asserts.isValidEnum<EnumSample>(someEnumSample, "'{nameof(number)}' cannot be null.");
            Asserts.isValidEnum<EnumSample>(someEnumSampleAsString, "'{nameof(number)}' cannot be null.");
        }

        [TestCategory("Asserts")]
        [TestMethod]
        public void Asserting_bad_values()
        {
            string someString, someEnumSampleAsString;
            int number;
            Type someType;
            EnumSample someEnumSample;
            object someObject;

            someString = null;
            number = 0;
            someType = null;
            someObject = someType;

            someEnumSample = (EnumSample)1000;
            someEnumSampleAsString = "Uno_que_llega";

            try
            {
                Asserts.isFilled(someString, $"'{nameof(someString)}' cannot be empty.");
                Assert.Fail();
            }
            catch { }

            try
            {
                Asserts.check(number > 0, $"'{nameof(number)}' must be greater than zero.");
                Assert.Fail();
            }
            catch { }

            try
            {
                Asserts.isSomething(someType, "'{nameof(number)}' cannot be null.");
                Assert.Fail();
            }
            catch { }

            try
            {
                Asserts.isSomething(someObject, "'{nameof(number)}' cannot be null.");
                Assert.Fail();
            }
            catch { }

            try
            {
                Asserts.isValidEnum<EnumSample>(someEnumSample, "'{nameof(number)}' cannot be null.");
                Assert.Fail();
            }
            catch { }

            try
            {
                Asserts.isValidEnum<EnumSample>(someEnumSampleAsString, "'{nameof(number)}' cannot be null.");
                Assert.Fail();
            }
            catch { }

        }
    }
}
