using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QTFK.Services;
using QTFK.Models;
using QTFK.Services.Loggers;
using System.Collections.Generic;

namespace QTFK.Core.Tests
{
    [TestClass]
    public class ConsoleLoggerTests
    {
        [TestMethod]
        [TestCategory("Loggers")]
        public void ConsoleLogger_Tests()
        {
            var buffer = new System.IO.StringWriter();
            Console.SetOut(buffer);

            ILogger<LogLevel> log = new ConsoleLogger<LogLevel>(new Dictionary<LogLevel,ConsoleColor>
            {
                { LogLevel.Error, ConsoleColor.Red },
                { LogLevel.Fatal, ConsoleColor.Red },
            });

            log.Log(LogLevel.Debug, "loggin debug message");
            log.Log(LogLevel.Info, "loggin information message");
            log.Log(LogLevel.Warning, "loggin a warning");
            log.Log(LogLevel.Error, "Ups! an Error!");
            log.Log(LogLevel.Fatal, "Booooom!!");

            buffer.Flush();
            string output = buffer.ToString();
            string expected = $@"loggin debug message
loggin information message
loggin a warning
Ups! an Error!
Booooom!!
";
            Assert.AreEqual(expected, output);
        }
    }
}
