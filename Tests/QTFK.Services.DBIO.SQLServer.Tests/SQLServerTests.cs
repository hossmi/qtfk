using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QTFK.Extensions.DBIO;
using System.Collections.Generic;
using QTFK.Extensions.DBCommand;
using QTFK.Extensions.DataReader;
using QTFK.Services.DBIO.SQLServer.Tests.Models;
using System.Data;
using QTFK.Extensions.DataSets;
using QTFK.Models;
using QTFK.Extensions.Collections.Dictionaries;

namespace QTFK.Services.DBIO.SQLServer.Tests
{
    [TestClass]
    public class SQLServerTests
    {
        private readonly string connectionString;
        private readonly CreateDrop createDrop;
        private readonly IDBIO db;

        public SQLServerTests()
        {
            this.connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["tests"]?.ConnectionString;
            if (string.IsNullOrWhiteSpace(this.connectionString))
                throw new ArgumentException($"Empty or invalid 'tests' connection string in app.config", "tests");

            this.db = new SQLServerDBIO(this.connectionString);
            this.createDrop = new CreateDrop(this.connectionString, this.db);
        }

        [TestInitialize()]
        public void Create()
        {
            this.createDrop.SQL_Drop_tables();
            this.createDrop.SQL_Create_tables();
        }

        [TestCleanup()]
        public void Drop()
        {
            this.createDrop.SQL_Drop_tables();
        }

        [TestMethod]
        [TestCategory("DB SQL Server")]
        public void SQLServer_Set_test()
        {
            var testPerson = new Person
            {
                Name = "pepe",
                LastName = "De la rosa Castaños",
            };

            this.db.Set(cmd =>
            {
                cmd.CommandText = $@"
                    USE qtfk
                    INSERT INTO persona (nombre, apellidos)
                    VALUES (@nombre,@apellidos)
                    ;";
                cmd.addParameters(new Dictionary<string, object>
                {
                    { "@nombre", testPerson.Name },
                    { "@apellidos", testPerson.LastName },
                });

                cmd.ExecuteNonQuery();

                int id = Convert.ToInt32(this.db.GetLastID(cmd));
                Assert.IsTrue(id > 0);

                cmd
                    .setCommandText($@" SELECT * FROM persona WHERE id = @id;")
                    .clearParameters()
                    .addParameter("@id", id)
                    ;

                var personsDB = cmd
                    .ExecuteReader()
                    .GetRecords()
                    .Select(r => new Person
                    {
                        Name = r.Get<string>("nombre"),
                        LastName = r.Get<string>("apellidos"),
                    })
                    .ToList()
                    ;

                Assert.AreEqual(1, personsDB.Count());
                var personDB = personsDB.First();
                Assert.AreEqual(testPerson.Name, personDB.Name);
                Assert.AreEqual(testPerson.LastName, personDB.LastName);
            });

            //TODO more SQL tests!!!
        }

        [TestMethod]
        [TestCategory("DB SQL Server")]
        public void SQLServer_Set_error_test()
        {
            try
            {
                this.db.Set(cmd =>
                {
                    throw new StackOverflowException("Booooom");
                });
                Assert.Fail("It was not expected to achieve this line!");
            }
            catch (DBIOException) { }
        }

        [TestMethod]
        [TestCategory("DB SQL Server")]
        public void SQLServer_Get_T_test()
        {
            var testPerson = new Person
            {
                Name = "pepe",
                LastName = "De la rosa Castaños",
            };

            this.db.Set($@"
                USE qtfk
                INSERT INTO persona (nombre, apellidos)
                VALUES (@nombre,@apellidos)
                ;", Parameters
                    .push("@nombre", testPerson.Name)
                    .push("@apellidos", testPerson.LastName)
                );

            this.db.Set($@"
                USE qtfk
                INSERT INTO persona (nombre, apellidos)
                VALUES (@nombre,@apellidos)
                ;", new Dictionary<string, object>
                {
                    { "@nombre", "Tronco" },
                    { "@apellidos", "Sanchez López" },
                });

            var personDB = this.db.Get($@" SELECT * FROM persona WHERE nombre = @nombre;",
                    new Dictionary<string, object> { { "@nombre", testPerson.Name } },
                    r => new Person
                    {
                        Name = r.Get<string>("nombre"),
                        LastName = r.Get<string>("apellidos"),
                    })
                    .FirstOrDefault()
                    ;

            Assert.IsNotNull(personDB);
            Assert.AreEqual(testPerson.Name, personDB.Name);
            Assert.AreEqual(testPerson.LastName, personDB.LastName);
        }

        [TestMethod]
        [TestCategory("DB SQL Server")]
        public void SQLServer_Get_T_error_test()
        {
            this.db.Set($@"
                USE qtfk
                INSERT INTO persona (nombre, apellidos)
                VALUES (@nombre,@apellidos)
                ;", new Dictionary<string, object>
                {
                    { "@nombre", "Tronco" },
                    { "@apellidos", "Sanchez López" },
                });

            try
            {
                var items = this.db.Get("This SQl is a bullshit", builder => builder.Get<string>("none"));
                Assert.Fail("It was not expected to achieve this line!");
            }
            catch (DBIOException) { }

            try
            {
                var items = this.db.Get("SELECT * FROM persona", builder => builder.Get<string>("notExistentColumn"));
                Assert.Fail("It was not expected to achieve this line!");
            }
            catch (DBIOException) { }
        }

        [TestMethod]
        [TestCategory("DB SQL Server")]
        public void SQLServer_Get_DataSet_test()
        {
            var testPerson = new Person
            {
                Name = "pepe",
                LastName = "De la rosa Castaños",
            };

            this.db.Set($@"
                USE qtfk
                INSERT INTO persona (nombre, apellidos)
                VALUES (@nombre,@apellidos)
                ;", new Dictionary<string, object>
                {
                    { "@nombre", testPerson.Name },
                    { "@apellidos", testPerson.LastName },
                });

            this.db.Set($@"
                USE qtfk
                INSERT INTO persona (nombre, apellidos)
                VALUES (@nombre,@apellidos)
                ;", new Dictionary<string, object>
                {
                    { "@nombre", "Tronco" },
                    { "@apellidos", "Sanchez López" },
                });

            var data = this.db.Get($@" SELECT * FROM persona");
            Assert.AreEqual(1, data.Tables.Count);
            Assert.AreEqual(2, data.Tables[0].Rows.Count);
            Assert.IsNotNull(data.AsTable());

            var row = data
                .AsTable()
                .AsEnumerable()
                .First(r => r.Get<string>("nombre") == testPerson.Name)
                ;

            Assert.AreEqual(testPerson.Name, row.Get<string>("nombre"));
            Assert.AreEqual(testPerson.LastName, row.Get<string>(2));
        }

        [TestMethod]
        [TestCategory("DB SQL Server")]
        public void SQLServer_Get_DataSet_error_test()
        {
            try
            {
                this.db.Get("Wrooon SQL statment");
                Assert.Fail("It was not expected to achieve this line!");
            }
            catch (DBIOException) { }
        }
    }
}
