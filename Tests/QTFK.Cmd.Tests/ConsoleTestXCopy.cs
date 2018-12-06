﻿using System;
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
    public class ConsoleTestXCopy
    {
        private IConsoleArgsService _appArgs;
        private Func<IConsoleArgsBuilder, XCopyArgsTest> _builder;
        private IList<Exception> _errors;
        private IEnumerable<ArgumentInfo> _appOptions;
        private string _appDescription;

        [TestInitialize()]
        public void Init()
        {
            _errors = new List<Exception>();
            _appOptions = Enumerable.Empty<ArgumentInfo>();
            _appDescription = string.Empty;

            _appArgs = new ConsoleArgsService()
                .As<IConsoleArgsService>()
                .setCaseSensitive(false)
                .setDescription("Copy directory trees and files.")
                .setHelp("?", "Shows this help.")
                .setPrefix("/")
                .setErrorHandler(_errors.Add)
                .setUsageHandler((description,options) => { _appDescription = description; _appOptions = options; })
                .setShowHelpOnError(true)
                ;

            _builder = b => new XCopyArgsTest
            {
                Source = b.getRequired(1, "source", "Source files to be copied."),
                Target = b.getOptional(2, "target", "Destination path.", Environment.CurrentDirectory),
                Recursive = b.getFlag("s", "Recursive copy of subfoldes."),
                CopyEmptyFolders = b.getFlag("e", "With 's' option, copies empty subfolders."),
                VerifyEachFileSize = b.getFlag("v", "Verifies file size after each copy."),
                CopyAfter = b.setOptional("d", "Copy files modified during or after specified date.", DateTime.MinValue),
                Overwrite = b.getFlag("r", "Overwrites existing files."),
                Retries = b.setOptional("Retries", "Number of times to retry the copy operation", 3),
            };
        }

        [TestMethod]
        [TestCategory("Console")]
        public void Console_xcopy_test_empty()
        {
            var result = _appArgs.Parse(Enumerable.Empty<string>(), _builder);

            Assert.IsNull(result);

            Assert.AreEqual(1, _errors.Count());
            Assert.AreEqual("Copy directory trees and files.", _appDescription);
            Assert.AreEqual(9, _appOptions.Count());
        }

        [TestMethod]
        [TestCategory("Console")]
        public void Console_xcopy_test_required()
        {
            var result = _appArgs.Parse(new string[] { @"C:\pepe\tronco\*.txt" }, _builder);

            Assert.IsNotNull(result);
            Assert.AreEqual(@"C:\pepe\tronco\*.txt", result.Source);
            Assert.AreEqual(Environment.CurrentDirectory, result.Target);
            Assert.AreEqual(false, result.VerifyEachFileSize);
            Assert.AreEqual(3, result.Retries);
            Assert.AreEqual(false, result.Recursive);
            Assert.AreEqual(false, result.Overwrite);
            Assert.AreEqual(false, result.CopyEmptyFolders);
            Assert.AreEqual(DateTime.MinValue, result.CopyAfter);

            Assert.AreEqual(0, _errors.Count());
            Assert.AreEqual(string.Empty, _appDescription);
            Assert.AreEqual(0, _appOptions.Count());
        }

        [TestMethod]
        [TestCategory("Console")]
        public void Console_xcopy_test_full()
        {
            var result = _appArgs.Parse(new string[] 
            {
                @"C:\pepe\tronco\*.txt",
                "/s",
                "/e",
                @"d:\temp\",
                "/v",
                "/d", new DateTime(2099,12,31).ToShortDateString(),
                "/r",
                "/retries", "34"
            }, _builder);

            Assert.IsNotNull(result);
            Assert.AreEqual(@"C:\pepe\tronco\*.txt", result.Source);
            Assert.AreEqual(@"d:\temp\", result.Target);
            Assert.AreEqual(true, result.VerifyEachFileSize);
            Assert.AreEqual(34, result.Retries);
            Assert.AreEqual(true, result.Recursive);
            Assert.AreEqual(true, result.Overwrite);
            Assert.AreEqual(true, result.CopyEmptyFolders);
            Assert.AreEqual(new DateTime(2099, 12, 31), result.CopyAfter);

            Assert.AreEqual(0, _errors.Count());
            Assert.AreEqual(string.Empty, _appDescription);
            Assert.AreEqual(0, _appOptions.Count());
        }
    }
}
