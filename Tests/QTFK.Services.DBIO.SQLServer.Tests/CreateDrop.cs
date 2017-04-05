using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QTFK.Extensions.DBIO;

namespace QTFK.Services.DBIO.SQLServer.Tests
{
    [TestClass]
    public class CreateDrop
    {
        private readonly string _connectionString;
        private readonly IDBIO _db;

        public CreateDrop() : this(null, null) { }

        public CreateDrop(string connectionString = null, IDBIO db = null)
        {
            _connectionString = connectionString ?? System.Configuration.ConfigurationManager.ConnectionStrings["tests"]?.ConnectionString;
            if (string.IsNullOrWhiteSpace(_connectionString))
                throw new ArgumentException($"Empty or invalid 'tests' connection string in app.config", "tests");

            _db = db ?? new SQLServerDBIO(_connectionString);
        }

        [TestMethod]
        [TestCategory("DB")]
        public void SQL_Create_tables()
        {
            _db.SetInBlock(File.ReadBlocks("create.sql", "go"));
        }

        [TestMethod]
        [TestCategory("DB")]
        public void SQL_Drop_tables()
        {
            _db.SetIndividually(File.ReadBlocks("drop.sql", "go"), false);
        }
    }
}
