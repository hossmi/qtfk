using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QTFK.Extensions.DBIO;
using QTFK.Extensions.Collections.Filters;
using QTFK.FileSystem;

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
            this._connectionString = connectionString ?? System.Configuration.ConfigurationManager.ConnectionStrings["tests"]?.ConnectionString;
            if (string.IsNullOrWhiteSpace(this._connectionString))
                throw new ArgumentException($"Empty or invalid 'tests' connection string in app.config", "tests");

            this._db = db ?? new SQLServerDBIO(this._connectionString);
        }

        [TestMethod]
        [TestCategory("Requires SQL Server database connection")]
        public void SQL_Create_tables()
        {
            this._db.Set(FileExtension.readBlocks("create.sql", "go").NotEmpty(), true);
        }

        [TestMethod]
        [TestCategory("Requires SQL Server database connection")]
        public void SQL_Drop_tables()
        {
            this._db.Set(FileExtension.readBlocks("drop.sql", "go").NotEmpty(), false);
        }
    }
}
