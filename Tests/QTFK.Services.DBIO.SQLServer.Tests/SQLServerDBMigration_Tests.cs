using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QTFK.Models;
using QTFK.Extensions.DBIO.DBIOMigrationExtensions;
using System.Collections.Generic;
using System.Linq;
using QTFK.Extensions.DBIO;
using QTFK.Extensions.Collections.Dictionaries;

namespace QTFK.Services.DBIO.SQLServer.Tests
{
    [TestClass]
    public class SQLServerDBMigration_Tests
    {
        private readonly string _connectionString;
        private readonly IDBIO _db;
        private readonly CreateDrop _createDrop;

        public SQLServerDBMigration_Tests()
        {
            this._connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["tests"]?.ConnectionString;
            if (string.IsNullOrWhiteSpace(this._connectionString))
                throw new ArgumentException($"Empty or invalid 'tests' connection string in app.config", "tests");
            this._db = new SQLServerDBIO(this._connectionString);
            this._createDrop = new CreateDrop(this._connectionString, this._db);
        }

        [TestInitialize]
        [TestCleanup]
        public void Drop()
        {
            this._createDrop.SQL_Drop_tables();
            IDBMigrator migrator = new SQLServerDBMigrator(this._db, new DefaultDBMigrationStepProvider(Enumerable.Empty<IDBMigrationStep>()));
            new Result(migrator.UnInstall);
            new Result(() => DropTableCliente(this._db));
        }

        [TestMethod]
        [TestCategory("DB SQL Server")]
        [TestCategory("DB Migrations")]
        public void SQLServerDBMigrator_tests()
        {
            IDBMigrator migrator = new SQLServerDBMigrator(this._db, new DefaultDBMigrationStepProvider(GetMigrations()));

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
            migrator = new SQLServerDBMigrator(this._db, new BadMigrationProvider());

            steps = migrator.Upgrade();

            Assert.IsFalse(steps.Any());
            version = migrator.GetCurrentVersion();
            migrationStep = migrator.GetLastMigration();
            Assert.AreEqual(3, version);
            Assert.AreEqual(3, migrationStep.Version);
            Assert.AreEqual(_kMigration3, migrationStep.Description);
        }

        const string _kMigration3 = "Insercion de clientes por defecto";

        private IEnumerable<IDBMigrationStep> GetMigrations()
        {
            yield return new DBMigrationStep
            {
                ForVersion = 0,
                Description = "Creación de tabla cliente",

                Upgrade = db =>
                {
                    db.Set($@"
                        CREATE TABLE cliente (
                            id int NOT NULL
                            , nombre nvarchar(2048) NULL
                            , id_tipo_cliente int NULL
                            , CONSTRAINT pk_cliente PRIMARY KEY (id)
                        );");
                    return 1;
                },
                Downgrade = DropTableCliente,
            };

            yield return new DBMigrationStep
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
                    , Parameters.start()
                        .push("@id", 1000001)
                        .push("@nombre", "Pepe")
                        .push("@tipo_cliente", 1)
                    );

                    db.Set($@"
                        INSERT INTO cliente(
                            id, nombre, id_tipo_cliente
                        )
                        VALUES(
                            @id, @nombre, @tipo_cliente
                        ); "
                    , Parameters.start()
                        .push("@id", 1000002)
                        .push("@nombre", "Tronco")
                        .push("@tipo_cliente", 2)
                    );
                    return 3;
                },
                Downgrade = db =>
                {
                    db.Set("DELETE FROM cliente WHERE nombre = @nombre;", Parameters.push("@nombre", "Pepe"));
                    db.Set("DELETE FROM cliente WHERE nombre = @nombre;", Parameters.push("@nombre", "Tronco"));
                },
            };

        }

        private void DropTableCliente(IDBIO db)
        {
            db.Set($@"DROP TABLE cliente");
        }

        class BadMigrationProvider : DBMigrationStepProviderBase
        {
            public override IEnumerable<IDBMigrationStep> GetSteps()
            {
                yield return new DBMigrationStep
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
}
