using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QTFK.Services;
using System.Collections.Generic;
using QTFK.Models;
using System.Linq;
using QTFK.Extensions;
using QTFK.Extensions.Collections.Casting;
using QTFK.Cmd.Tests.Models;
using QTFK.Services.ConsoleArgsServices;

namespace QTFK.Cmd.Tests
{
    [TestClass]
    public class ConsoleTest1
    {
        [TestMethod]
        [TestCategory("Console")]
        public void Console_Test1()
        {
            var someDate = new DateTime(2012, 1, 1);
            var args = new string[]
            {
                "--somestring", "pepe",
                "--somenumber", "3,1415",
                @"c:\source",
                "--someOptionalDate", someDate.ToShortDateString(),
                @"d:\target",
                "5",
                "--someFlag",
            };

            var appArgs = new ConsoleArgsService()
                .setCaseSensitive(false)
                .setPrefix("--")
                .setErrorHandler(error => Assert.Fail($"Unexpected Argument Error! {error.Message}"))
                .Parse(args, builder => new
                {
                    RequiredString = builder.getRequired("someString", "Description for someString option."),
                    RequiredNumber = builder.setRequired<double>("someNumber", "Description for someNumber option."),
                    Input2 = builder.getRequired(2, "source", "Source path"),
                    Input1 = builder.getRequired(1, "target", "Target path"),
                    Input3_optional = builder.setOptional(3, "iterations", "Number of times to try copy", 13),
                    Flag = builder.getFlag("someFlag", "Description for someFlag."),
                    OptionalDate = builder.setOptional("someOptionalDate", "Description for someOptionalDate.", new DateTime(2099, 12, 31))
                });

            Assert.AreEqual("pepe", appArgs.RequiredString);
            Assert.AreEqual(3.1415, appArgs.RequiredNumber);
            Assert.AreEqual(@"c:\source", appArgs.Input1);
            Assert.AreEqual(@"d:\target", appArgs.Input2);
            Assert.AreEqual(5, appArgs.Input3_optional);
            Assert.AreEqual(true, appArgs.Flag);
            Assert.AreEqual(someDate, appArgs.OptionalDate);
        }

        [TestMethod]
        [TestCategory("Console")]
        public void Console_Test_Case_Sensitive()
        {
            var errors = new List<Exception>();

            var service = new ConsoleArgsService()
                .setCaseSensitive(true)
                .setPrefix("/")
                .setErrorHandler(errors.Add)
                ;

            string[] args = new string[] { "/somearg", "3" };
            var result = service.Parse(args, builder => new
            {
                Arg1 = builder.getRequired("someArg", "Description for someArg option."),
            });

            Assert.AreEqual(1, errors.Count());

            errors.Clear();
            service.CaseSensitive = false;
            result = service.Parse(args, builder => new
            {
                Arg1 = builder.getRequired("someArg", "Description for someArg option."),
            });

            Assert.AreEqual(0, errors.Count());
        }
    }
}
