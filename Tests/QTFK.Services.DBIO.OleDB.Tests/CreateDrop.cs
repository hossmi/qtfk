using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QTFK.Extensions.DBIO;
using QTFK.Extensions.Collections.Filters;
using System.Configuration;
using QTFK.FileSystem;

namespace QTFK.Services.DBIO.OleDB.Tests
{
    [TestClass]
    public class CreateDrop
    {
        private readonly string _connectionString;
        private readonly IDBIO _db;

        public CreateDrop() : this(null, null) { }

        public CreateDrop(string connectionString = null, IDBIO db = null)
        {
            _connectionString = connectionString ?? ConfigurationManager.ConnectionStrings["tests"]?.ConnectionString;
            if (string.IsNullOrWhiteSpace(_connectionString))
                throw new ArgumentException($"Empty or invalid 'tests' connection string in app.config", "tests");

            _db = db ?? new OleDBIO(_connectionString);
        }

        [TestMethod]
        [TestCategory("DB")]
        public void OleDB_Create_tables()
        {
            _db.Set(FileExtension.readBlocks("create.sql", "go").NotEmpty(), true);
        }

        [TestMethod]
        [TestCategory("DB")]
        public void OleDB_Drop_tables()
        {
            _db.Set(FileExtension.readBlocks("drop.sql", "go").NotEmpty(), false);
        }
    }
}
