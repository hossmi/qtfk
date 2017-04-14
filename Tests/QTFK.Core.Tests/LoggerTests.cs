using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QTFK.Services;
using QTFK.Models;
using QTFK.Services.Loggers;
using System.Collections.Generic;

namespace QTFK.Core.Tests
{
    [TestClass]
    public class LoggerTests
    {
        [TestMethod]
        [TestCategory("Loggers")]
        public void ConsoleLogger_T_Tests()
        {
            var buffer = new System.IO.StringWriter();
            Console.SetOut(buffer);

            ILogger<LogLevel> log = new ConsoleLogger<LogLevel>(new Dictionary<LogLevel, ConsoleColor>
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

        [TestMethod]
        [TestCategory("Loggers")]
        public void ConsoleLogger_Tests()
        {
            var buffer = new System.IO.StringWriter();
            Console.SetOut(buffer);

            ILogger<LogLevel> log = new ConsoleLogger(new Dictionary<LogLevel, ConsoleColor>
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

        [TestMethod]
        [TestCategory("Loggers")]
        public void MultiLogger_Tests()
        {
            var buffer = new System.IO.StringWriter();
            Console.SetOut(buffer);

            ILogger<LogLevel> log = new ConsoleLogger(new Dictionary<LogLevel, ConsoleColor>
            {
                { LogLevel.Error, ConsoleColor.Red },
                { LogLevel.Fatal, ConsoleColor.Red },
            });

            log = new MultiLogger(new ILogger<LogLevel>[] { log });

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

        [TestMethod]
        [TestCategory("Loggers")]
        public void FileLogger_Tests()
        {
            string path = "./log.txt";
            Func<LogLevel, string, string> builder = (LogLevel level, string message) => { return $"{level.ToString()} # {message}"; };

            if(System.IO.File.Exists(path))
                System.IO.File.Delete(path);

            Assert.IsFalse(System.IO.File.Exists(path), "Test couldn't be prepared. Unable to remove previous file");

            var log = new FileLogger(path, builder);
            log.Log(LogLevel.Debug, "probando");
            log.Log(LogLevel.Error, "Boooom!");

            Assert.IsTrue(System.IO.File.Exists(path), "File was not created");

            var output = System.IO.File.ReadAllLines(path);

            Assert.AreEqual(2, output.Length, "Unexptected number of lines generated");
            Assert.AreEqual("Debug # probando", output[0]);
            Assert.AreEqual("Error # Boooom!", output[1]);
        }
    }
}
