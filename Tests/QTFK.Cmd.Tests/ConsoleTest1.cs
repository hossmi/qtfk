using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QTFK.Services;
using System.Collections.Generic;
using QTFK.Models;
using System.Linq;
using QTFK.Extensions;
using QTFK.Extensions.Collections.Casting;
using QTFK.Cmd.Tests.Models;

namespace QTFK.Cmd.Tests
{
    [TestClass]
    public class ConsoleTest1
    {
        [TestMethod]
        public void Console_Test1()
        {
            var someDate = new DateTime(2012, 1, 1);
            var args = new string[]
            {
                "--someString", "pepe",
                "--someNumber", "3,1415",
                @"c:\source",
                "--someOptionalDate", someDate.ToShortDateString(),
                @"d:\target",
                "5",
                "--someFlag",
            };

            var appArgs = new ConsoleArgsService()
                .SetCaseSensitive(false)
                .SetPrefix("--")
                .AddErrorHandler(error => Assert.Fail($"Unexpected Argument Error! {error.Message}"))
                .Parse(args, builder => new
                {
                    RequiredString = builder.ByName("someString", "Description for someString option."),
                    RequiredNumber = double.Parse(builder.ByName("someNumber", "Description for someNumber option.")),
                    Input2 = builder.ByIndex(2, "source", "Source path"),
                    Input1 = builder.ByIndex(1, "target", "Target path"),
                    Input3_optional = builder.ByIndex(3, "iterations", "Number of times to try copy", 13),
                    Flag = builder.Flag("someFlag", "Description for someFlag."),
                    OptionalDate = builder.ByName("someOptionalDate", "Description for someOptionalDate.", new DateTime(2099, 12, 31))
                });

            Assert.AreEqual("pepe", appArgs.RequiredString);
            Assert.AreEqual(3.1415, appArgs.RequiredNumber);
            Assert.AreEqual(@"c:\source", appArgs.Input1);
            Assert.AreEqual(@"d:\target", appArgs.Input2);
            Assert.AreEqual(5, appArgs.Input3_optional);
            Assert.AreEqual(true, appArgs.Flag);
            Assert.AreEqual(someDate, appArgs.OptionalDate);
        }
    }
}
