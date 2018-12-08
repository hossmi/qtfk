using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using QTFK.Services;
using QTFK.Models;
using QTFK.Services.Loggers;
using QTFK.Extensions.Loggers;

namespace QTFK.Logs.Tests
{
    [TestClass]
    public class LoggerTests
    {
        [TestMethod]
        public void ConsoleLogger_T_Tests()
        {
            var buffer = new System.IO.StringWriter();
            Console.SetOut(buffer);

            ILogger<LogLevel> log = new ConsoleLogger<LogLevel>(new Dictionary<LogLevel, ConsoleColor>
            {
                { LogLevel.Error, ConsoleColor.Red },
                { LogLevel.Fatal, ConsoleColor.Red },
            });

            log.log(LogLevel.Debug, "loggin debug message");
            log.log(LogLevel.Info, "loggin information message");
            log.log(LogLevel.Warning, "loggin a warning");
            log.log(LogLevel.Error, "Ups! an Error!");
            log.log(LogLevel.Fatal, "Booooom!!");

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
        public void ConsoleLogger_Tests()
        {
            var buffer = new System.IO.StringWriter();
            Console.SetOut(buffer);

            ILogger<LogLevel> log = new ConsoleLogger(new Dictionary<LogLevel, ConsoleColor>
            {
                { LogLevel.Error, ConsoleColor.Red },
                { LogLevel.Fatal, ConsoleColor.Red },
            });

            log.log(LogLevel.Debug, "loggin debug message");
            log.log(LogLevel.Info, "loggin information message");
            log.log(LogLevel.Warning, "loggin a warning");
            log.log(LogLevel.Error, "Ups! an Error!");
            log.log(LogLevel.Fatal, "Booooom!!");

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
        public void Logger_Filter_Tests()
        {
            var buffer = new System.IO.StringWriter();
            Console.SetOut(buffer);

            ILogger<LogLevel> log = new ConsoleLogger(new Dictionary<LogLevel, ConsoleColor>
            {
                { LogLevel.Error, ConsoleColor.Red },
                { LogLevel.Fatal, ConsoleColor.Red },
            }, l => LogLevel.Warning <= l && l <= LogLevel.Error);

            log.log(LogLevel.Debug, "loggin debug message");
            log.log(LogLevel.Info, "loggin information message");
            log.log(LogLevel.Warning, "loggin a warning");
            log.log(LogLevel.Error, "Ups! an Error!");
            log.log(LogLevel.Fatal, "Booooom!!");

            buffer.Flush();
            string output = buffer.ToString();
            string expected = $@"loggin a warning
Ups! an Error!
";
            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void ConsoleLogger_extension_Tests()
        {
            var buffer = new System.IO.StringWriter();
            Console.SetOut(buffer);

            ILogger<LogLevel> log = new ConsoleLogger(new Dictionary<LogLevel, ConsoleColor>
            {
                { LogLevel.Error, ConsoleColor.Red },
                { LogLevel.Fatal, ConsoleColor.Red },
            });

            log.debug("loggin debug message");
            log.info("loggin information message");
            log.warning("loggin a warning");
            log.error("Ups! an Error!");
            log.fatal("Booooom!!");

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

            log.log(LogLevel.Debug, "loggin debug message");
            log.log(LogLevel.Info, "loggin information message");
            log.log(LogLevel.Warning, "loggin a warning");
            log.log(LogLevel.Error, "Ups! an Error!");
            log.log(LogLevel.Fatal, "Booooom!!");

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
        public void FileLogger_Tests()
        {
            string path;
            ILogger<LogLevel> log;
            string[] output;

            path = "./log.txt";

            if(System.IO.File.Exists(path))
                System.IO.File.Delete(path);

            Assert.IsFalse(System.IO.File.Exists(path), "Test couldn't be prepared. Unable to remove previous file");

            log = new FileLogger(path, (level, message) => $"{level.ToString()} # {message}");
            log.log(LogLevel.Debug, "testing...");
            log.log(LogLevel.Error, "Boooom!");

            Assert.IsTrue(System.IO.File.Exists(path), "File was not created");

            output = System.IO.File.ReadAllLines(path);

            Assert.AreEqual(2, output.Length, "Unexptected number of lines generated");
            Assert.AreEqual("Debug # testing...", output[0]);
            Assert.AreEqual("Error # Boooom!", output[1]);
        }
    }
}
