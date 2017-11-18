using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QTFK.Models;

namespace QTFK.Core.Tests
{
    [TestClass]
    public class AssertTests
    {
        [TestCategory("Asserts")]
        [TestMethod]
        public void Asserting_good_values()
        {
            string someString, someLogLevelAsString;
            int number;
            Type someType;
            LogLevel someLogLevel;
            object someObject;

            someString = "pepe";
            number = 5;
            someType = typeof(AssertTests);
            someObject = someType;

            someLogLevel = LogLevel.Info;
            someLogLevelAsString = LogLevel.Fatal.ToString();

            Asserts.IsFilled(someString, $"'{nameof(someString)}' cannot be empty.");
            Asserts.IsGreaterThanZero(number, $"'{nameof(number)}' must be greater than zero.");
            Asserts.IsInstance(someType, "'{nameof(number)}' cannot be null.");
            Asserts.IsInstance(someObject, "'{nameof(number)}' cannot be null.");
            Asserts.IsValidEnum(someLogLevel, "'{nameof(number)}' cannot be null.");
            Asserts.IsValidEnum<LogLevel>(someLogLevelAsString, "'{nameof(number)}' cannot be null.");
        }

        [TestCategory("Asserts")]
        [TestMethod]
        public void Asserting_bad_values()
        {
            string someString, someLogLevelAsString;
            int number;
            Type someType;
            LogLevel someLogLevel;
            object someObject;

            someString = null;
            number = 0;
            someType = null;
            someObject = someType;

            someLogLevel = (LogLevel)1000;
            someLogLevelAsString = "Uno_que_llega";

            try
            {
                Asserts.IsFilled(someString, $"'{nameof(someString)}' cannot be empty.");
                Assert.Fail();
            }
            catch { }

            try
            {
                Asserts.IsGreaterThanZero(number, $"'{nameof(number)}' must be greater than zero.");
                Assert.Fail();
            }
            catch { }

            try
            {
                Asserts.IsInstance(someType, "'{nameof(number)}' cannot be null.");
                Assert.Fail();
            }
            catch { }

            try
            {
                Asserts.IsInstance(someObject, "'{nameof(number)}' cannot be null.");
                Assert.Fail();
            }
            catch { }

            try
            {
                Asserts.IsValidEnum(someLogLevel, "'{nameof(number)}' cannot be null.");
                Assert.Fail();
            }
            catch { }

            try
            {
                Asserts.IsValidEnum<LogLevel>(someLogLevelAsString, "'{nameof(number)}' cannot be null.");
                Assert.Fail();
            }
            catch { }

        }
    }
}
