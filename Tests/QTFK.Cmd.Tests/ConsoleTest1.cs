using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QTFK.Services;
using System.Collections.Generic;
using QTFK.Models;
using System.Linq;
using QTFK.Extensions;

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
                .SetErrorHandler(error => Assert.Fail($"Unexpected Argument Error! {error.Message}"))
                .Parse(args, builder => new
                {
                    RequiredString = builder.Get("someString"),
                    RequiredNumber = double.Parse(builder.Get("someNumber")),
                    Input2 = builder.Get(2),
                    Input1 = builder.Get(1),
                    Input3_optional = int.Parse(builder.Get(3, 13)),
                    Flag = builder.HasFlag("someFlag"),
                    OptionalDate = DateTime.Parse(builder.Get("someOptionalDate", new DateTime(2099, 12, 31)))
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
