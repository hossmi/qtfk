using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QTFK.Models;

namespace QTFK.Core.Tests
{
    enum EnumSample
    {
        min = 5,
        average = 8,
        max = 13,
    }

    [TestClass]
    public class AssertTests
    {
        [TestMethod]
        public void when_assert_good_values_code_does_not_throws_exception()
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

            someEnumSample = EnumSample.min;
            someEnumSampleAsString = EnumSample.max.ToString();

            Asserts.stringIsNotEmpty(someString);
            Asserts.isTrue(number > 0);
            Asserts.isNotNull(someType);
            Asserts.isNotNull(someObject);
            Asserts.isValidEnum<EnumSample>(someEnumSample);
            Asserts.isValidEnum<EnumSample>(someEnumSampleAsString);
        }

        [TestMethod]
        public void when_assert_bad_values_code_throws_exception()
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
                Asserts.stringIsNotEmpty(someString);
                Assert.Fail();
            }
            catch { }

            try
            {
                Asserts.isTrue(number > 0);
                Assert.Fail();
            }
            catch { }

            try
            {
                Asserts.isNotNull(someType);
                Assert.Fail();
            }
            catch { }

            try
            {
                Asserts.isNotNull(someObject);
                Assert.Fail();
            }
            catch { }

            try
            {
                Asserts.isValidEnum<EnumSample>(someEnumSample);
                Assert.Fail();
            }
            catch { }

            try
            {
                Asserts.isValidEnum<EnumSample>(someEnumSampleAsString);
                Assert.Fail();
            }
            catch { }

        }
    }
}
