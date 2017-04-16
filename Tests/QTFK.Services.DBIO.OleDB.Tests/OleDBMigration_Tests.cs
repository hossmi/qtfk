﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QTFK.Models;
using QTFK.Extensions.DBIOMigrationExtensions;
using System.Collections.Generic;
using System.Linq;
using QTFK.Extensions.DBIO;
using QTFK.Extensions.DBCommand;

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
        [TestCategory("DB OleDB")]
        [TestCategory("DB Migrations")]
        public void OleDBMigration_tests()
        {
            IDBMigrator migrator = new OleDBMigrator(_db, GetMigrations());

            int version = migrator.GetCurrentVersion();
            var migrationStep = migrator.GetLastMigration();
            Assert.AreEqual(0, version);
            Assert.AreEqual(new DateTime(0), migrationStep.MigrationDate);
            Assert.AreEqual(0, migrationStep.Version);

            IEnumerable<MigrationInfo> steps = migrator.Upgrade();

            version = migrator.GetCurrentVersion();
            migrationStep = migrator.GetLastMigration();
            Assert.AreEqual(3, version);
            Assert.AreEqual(3, migrationStep.Version);
            Assert.AreEqual(_kMigration3, migrationStep.Description);
            Assert.AreEqual(2, steps.Count());

            steps = migrator.Upgrade();

            Assert.IsFalse(steps.Any());
            version = migrator.GetCurrentVersion();
            migrationStep = migrator.GetLastMigration();
            Assert.AreEqual(3, version);
            Assert.AreEqual(3, migrationStep.Version);
            Assert.AreEqual(_kMigration3, migrationStep.Description);

            //simulating error on migration from 3 to 4
            migrator = new OleDBMigrator(_db, GetBadMigration());

            steps = migrator.Upgrade();

            Assert.IsFalse(steps.Any());
            version = migrator.GetCurrentVersion();
            migrationStep = migrator.GetLastMigration();
            Assert.AreEqual(3, version);
            Assert.AreEqual(3, migrationStep.Version);
            Assert.AreEqual(_kMigration3, migrationStep.Description);
        }

        const string _kMigration3 = "Insercion de clientes por defecto";

        private IEnumerable<IMigrationStep> GetMigrations()
        {
            yield return new MigrationStep
            {
                ForVersion = 0,
                Description = "Creación de tabla cliente",

                Upgrade = db =>
                {
                    db.Set($@"
                        CREATE TABLE cliente (
                            id int NULL
                            , nombre varchar(255) NULL
                            , id_tipo_cliente int NULL
                            , CONSTRAINT pk_cliente PRIMARY KEY (id)
                        );");
                    return 1;
                },
                Downgrade = db => db.Set($@"DROP TABLE cliente"),
            };

            yield return new MigrationStep
            {
                ForVersion = 1,
                Description = _kMigration3,

                Upgrade = db =>
                {
                    db.Set($@"
                        INSERT INTO cliente ( 
                            id, nombre, id_tipo_cliente
                        ) 
                        VALUES ( 
                            @id, @nombre, @tipo_cliente
                        );"
                    , db.Params()
                        .Set("@id", 1000001)
                        .Set("@nombre", "Pepe")
                        .Set("@tipo_cliente", 1)
                    );

                    db.Set($@"
                        INSERT INTO cliente(
                            id, nombre, id_tipo_cliente
                        )
                        VALUES(
                            @id, @nombre, @tipo_cliente
                        ); "
                    , db.Params()
                        .Set("@id", 1000002)
                        .Set("@nombre", "Tronco")
                        .Set("@tipo_cliente", 2)
                    );
                    return 3;
                },
                Downgrade = db =>
                {
                    db.Set("DELETE FROM cliente WHERE nombre = @nombre;", db.Param("@nombre", "Pepe"));
                    db.Set("DELETE FROM cliente WHERE nombre = @nombre;", db.Param("@nombre", "Tronco"));
                },
            };

        }

        private IEnumerable<IMigrationStep> GetBadMigration()
        {
            yield return new MigrationStep
            {
                ForVersion = 3,
                Description = "campo long",
                Upgrade = db =>
                {
                    throw new Exception("Booooooom");
                },
            };
        }

    }
}
