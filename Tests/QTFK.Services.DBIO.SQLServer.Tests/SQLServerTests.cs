using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QTFK.Services;
using QTFK.Services.DBIO;
using QTFK.Extensions.DBIO;
using System.Collections.Generic;
using System.Text;
using QTFK.Extensions.Collections.Filters;
using QTFK.Extensions.DBCommand;
using QTFK.Extensions.DataReader;
using QTFK.Services.DBIO.SQLServer.Tests.Models;
using System.Data;
using QTFK.Extensions.DataSets;
using QTFK.Models;

namespace QTFK.Services.DBIO.SQLServer.Tests
{
    [TestClass]
    public class SQLServerTests
    {
        private readonly string _connectionString;
        private readonly CreateDrop _createDrop;
        private readonly IDBIO _db;

        public SQLServerTests()
        {
            _connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["tests"]?.ConnectionString;
            if (string.IsNullOrWhiteSpace(_connectionString))
                throw new ArgumentException($"Empty or invalid 'tests' connection string in app.config", "tests");

            _db = new SQLServerDBIO(_connectionString);
            _createDrop = new CreateDrop(_connectionString, _db);
        }

        [TestInitialize()]
        public void Create()
        {
            _createDrop.SQL_Drop_tables();
            _createDrop.SQL_Create_tables();
        }


        [TestCleanup()]
        public void Drop()
        {
            _createDrop.SQL_Drop_tables();
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

            _db.Set(cmd =>
            {
                cmd.CommandText = $@"
                    USE qtfk
                    INSERT INTO persona (nombre, apellidos)
                    VALUES (@nombre,@apellidos)
                    ;";
                cmd.AddParameters(new Dictionary<string, object>
                {
                    { "@nombre", testPerson.Name },
                    { "@apellidos", testPerson.LastName },
                });

                cmd.ExecuteNonQuery();

                int id = Convert.ToInt32(_db.GetLastID(cmd));
                Assert.IsTrue(id > 0);

                cmd
                    .SetCommandText($@" SELECT * FROM persona WHERE id = @id;")
                    .ClearParameters()
                    .AddParameter("@id", id)
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
                _db.Set(cmd =>
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

            _db.Set($@"
                USE qtfk
                INSERT INTO persona (nombre, apellidos)
                VALUES (@nombre,@apellidos)
                ;", new Dictionary<string, object>
                {
                    { "@nombre", testPerson.Name },
                    { "@apellidos", testPerson.LastName },
                });

            _db.Set($@"
                USE qtfk
                INSERT INTO persona (nombre, apellidos)
                VALUES (@nombre,@apellidos)
                ;", new Dictionary<string, object>
                {
                    { "@nombre", "Tronco" },
                    { "@apellidos", "Sanchez López" },
                });

            var personDB = _db.Get($@" SELECT * FROM persona WHERE nombre = @nombre;", _db.Param("@nombre", testPerson.Name), r => new Person
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
            _db.Set($@"
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
                var items = _db.Get("This SQl is a bullshit", builder => builder.Get<string>("none"));
                Assert.Fail("It was not expected to achieve this line!");
            }
            catch (DBIOException) { }

            try
            {
                var items = _db.Get("SELECT * FROM persona", builder => builder.Get<string>("notExistentColumn"));
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

            _db.Set($@"
                USE qtfk
                INSERT INTO persona (nombre, apellidos)
                VALUES (@nombre,@apellidos)
                ;", new Dictionary<string, object>
                {
                    { "@nombre", testPerson.Name },
                    { "@apellidos", testPerson.LastName },
                });

            _db.Set($@"
                USE qtfk
                INSERT INTO persona (nombre, apellidos)
                VALUES (@nombre,@apellidos)
                ;", new Dictionary<string, object>
                {
                    { "@nombre", "Tronco" },
                    { "@apellidos", "Sanchez López" },
                });

            var data = _db.Get($@" SELECT * FROM persona");
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
                _db.Get("Wrooon SQL statment");
                Assert.Fail("It was not expected to achieve this line!");
            }
            catch (DBIOException) { }
        }
    }
}
