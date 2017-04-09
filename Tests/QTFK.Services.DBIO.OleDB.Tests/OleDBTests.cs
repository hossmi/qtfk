﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using QTFK.Extensions.DataReader;
using QTFK.Extensions.DataSets;
using QTFK.Extensions.DBCommand;
using QTFK.Extensions.DBIO;
using QTFK.Models;
using QTFK.Services.DBIO.OleDB.Tests.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace QTFK.Services.DBIO.OleDB.Tests
{
    [TestClass]
    public class OleDBTests
    {
        private readonly string _connectionString;
        private readonly CreateDrop _createDrop;
        private readonly IDBIO _db;

        public OleDBTests()
        {
            _connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["tests"]?.ConnectionString;
            if (string.IsNullOrWhiteSpace(_connectionString))
                throw new ArgumentException($"Empty or invalid 'tests' connection string in app.config", "tests");

            _db = new OleDBIO(_connectionString);
            _createDrop = new CreateDrop(_connectionString, _db);
        }

        [TestInitialize()]
        public void Create()
        {
            _createDrop.OleDB_Drop_tables();
            _createDrop.OleDB_Create_tables();
        }


        [TestCleanup()]
        public void Drop()
        {
            _createDrop.OleDB_Drop_tables();
        }

        [TestMethod]
        [TestCategory("DB OleDB")]
        public void OleDB_Set_test()
        {
            var testPerson = new Person
            {
                Name = "pepe",
                LastName = "De la rosa Castaños",
            };

            _db.Set(cmd =>
            {
                cmd.CommandText = $@"
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
        [TestCategory("DB OleDB")]
        public void OleDB_Set_error_test()
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
        [TestCategory("DB OleDB")]
        public void OleDB_Get_T_test()
        {
            var testPerson = new Person
            {
                Name = "pepe",
                LastName = "De la rosa Castaños",
            };

            _db.Set($@"
                INSERT INTO persona (nombre, apellidos)
                VALUES (@nombre,@apellidos)
                ;", new Dictionary<string, object>
                {
                    { "@nombre", testPerson.Name },
                    { "@apellidos", testPerson.LastName },
                });

            _db.Set($@"
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
        [TestCategory("DB OleDB")]
        public void OleDB_Get_T_error_test()
        {
            _db.Set($@"
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
        [TestCategory("DB OleDB")]
        public void OleDB_Get_DataSet_test()
        {
            var testPerson = new Person
            {
                Name = "pepe",
                LastName = "De la rosa Castaños",
            };

            _db.Set($@"
                INSERT INTO persona (nombre, apellidos)
                VALUES (@nombre,@apellidos)
                ;", new Dictionary<string, object>
                {
                    { "@nombre", testPerson.Name },
                    { "@apellidos", testPerson.LastName },
                });

            _db.Set($@"
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
        [TestCategory("DB OleDB")]
        public void OleDB_Get_DataSet_error_test()
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