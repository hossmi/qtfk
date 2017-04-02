using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QTFK.Services;
using QTFK.Services.DBIO;

namespace QTFK.Services.DBIO.SQLServer.Tests
{
    [TestClass]
    public class SQLServerTests
    {
        private readonly string _connectionString;

        public SQLServerTests()
        {
            _connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["tests"].ConnectionString;
            if (string.IsNullOrWhiteSpace(_connectionString))
                throw new ArgumentException($"Empty or invalid 'tests' connection string in app.config", "tests");


        }

        [TestMethod]
        public void TestMethod1()
        {
            IDBIO db = new SQLServerDBIO(_connectionString);
            
        }
    }
}
