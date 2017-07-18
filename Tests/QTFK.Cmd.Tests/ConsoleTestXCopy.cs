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
    public class ConsoleTestXCopy
    {
        private Func<IConsoleArgsBuilder, XCopyArgsTest> _builder;

        [TestInitialize()]
        public void Init()
        {
            _builder = b => new XCopyArgsTest
            {
                Source = b.ByIndex(1, "source", "Source files to be copied."),
                Target = b.ByIndex(2, "target", "Destination path.", Environment.CurrentDirectory),
                Recursive = b.Flag("s", "Recursive copy of subfoldes."),
                CopyEmptyFolders = b.Flag("e", "With 's' option, copies empty subfolders."),
                VerifyEachFileSize = b.Flag("v", "Verifies file size after each copy."),
                CopyAfter = b.ByName("d", "Copy files modified during or after specified date.", DateTime.MinValue),
                Overwrite = b.Flag("r", "Overwrites existing files."),
                Retries = b.ByName("retries", "Number of times to retry the copy operation", 3),
            };
        }

        [TestMethod]
        public void Console_xcopy_test1()
        {
            IList<ArgumentException> errors = new List<ArgumentException>();
            IList<ArgumentInfo> appOptions = new List<ArgumentInfo>();
            string appHelp = string.Empty;

            var appArgs = new ConsoleArgsService()
                .As<IConsoleArgsService>()
                .SetCaseSensitive(false)
                .SetDescription("Copy directory trees and files.")
                .SetHelp("?", "Shows this help.")
                .SetPrefix("/")
                .AddErrorHandler(errors.Add)
                .AddUsageHandler(description => appHelp = description)
                .AddUsageOptionHandler(appOptions.Add)
                .SetShowHelpOnError(true)
                ;

            var result = appArgs.Parse(Enumerable.Empty<string>(), _builder);
            Assert.IsNull(result);
        }
    }
}
