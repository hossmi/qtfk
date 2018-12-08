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
            this._connectionString = connectionString ?? ConfigurationManager.ConnectionStrings["tests"]?.ConnectionString;
            if (string.IsNullOrWhiteSpace(this._connectionString))
                throw new ArgumentException($"Empty or invalid 'tests' connection string in app.config", "tests");

            this._db = db ?? new OleDBIO(this._connectionString);
        }

        [TestMethod]
        public void OleDB_Create_tables()
        {
            this._db.Set(FileExtension.readBlocks("create.sql", "go").NotEmpty(), true);
        }

        [TestMethod]
        public void OleDB_Drop_tables()
        {
            this._db.Set(FileExtension.readBlocks("drop.sql", "go").NotEmpty(), false);
        }
    }
}
