using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QTFK.Models;
using QTFK.Extensions.DBIOMigrationExtensions;
using System.Collections.Generic;
using System.Linq;

namespace QTFK.Services.DBIO.OleDB.Tests
{
    [TestClass]
    public class OleDBMigration_Tests
    {
        private readonly string _connectionString;
        private readonly IDBIO _db;
        private readonly CreateDrop _createDrop;

        public OleDBMigration_Tests()
        {
            _connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["tests"]?.ConnectionString;
            if (string.IsNullOrWhiteSpace(_connectionString))
                throw new ArgumentException($"Empty or invalid 'tests' connection string in app.config", "tests");
            _db = new OleDBIO(_connectionString);
            _createDrop = new CreateDrop(_connectionString, _db);
        }

        [TestInitialize]
        [TestCleanup]
        public void Drop()
        {
            _createDrop.OleDB_Drop_tables();
        }

        [TestMethod]
        public void OleDBMigration_test_1()
        {
            IDBMigrator migrator = new OleDBMigrator(_db);

            int version = migrator.GetCurrentVersion();
            var migrationStep = migrator.GetLastMigration();
            Assert.AreEqual(0, version);
            Assert.AreEqual(new DateTime(0), migrationStep.MigrationDate);
            Assert.AreEqual(0, migrationStep.Version);

            IEnumerable<MigrationInfo> steps = migrator.Upgrade()
        }

    }
}
