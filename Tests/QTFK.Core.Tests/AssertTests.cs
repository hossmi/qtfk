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

            Asserts.isFilled(someString, $"'{nameof(someString)}' cannot be empty.");
            Asserts.check(number > 0, $"'{nameof(number)}' must be greater than zero.");
            Asserts.isSomething(someType, "'{nameof(number)}' cannot be null.");
            Asserts.isSomething(someObject, "'{nameof(number)}' cannot be null.");
            Asserts.isValidEnum<LogLevel>(someLogLevel, "'{nameof(number)}' cannot be null.");
            Asserts.isValidEnum<LogLevel>(someLogLevelAsString, "'{nameof(number)}' cannot be null.");
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
                Asserts.isValidEnum<LogLevel>(someLogLevel, "'{nameof(number)}' cannot be null.");
                Assert.Fail();
            }
            catch { }

            try
            {
                Asserts.isValidEnum<LogLevel>(someLogLevelAsString, "'{nameof(number)}' cannot be null.");
                Assert.Fail();
            }
            catch { }

        }
    }
}
