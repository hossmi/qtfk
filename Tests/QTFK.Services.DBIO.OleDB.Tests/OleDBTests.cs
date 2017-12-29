using Microsoft.VisualStudio.TestTools.UnitTesting;
using QTFK.Extensions.Collections.Dictionaries;
using QTFK.Extensions.DataReader;
using QTFK.Extensions.DataSets;
using QTFK.Extensions.DBCommand;
using QTFK.Extensions.DBIO;
using QTFK.Extensions.DBIO.DBQueries;
using QTFK.Extensions.DBIO.OleDBIOExtensions;
using QTFK.Extensions.Mapping.AutoMapping;
using QTFK.Models;
using QTFK.Models.DBIO;
using QTFK.Services.DBIO.OleDB.Tests.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using QTFK.Extensions.DBIO.QueryFactory;
using QTFK.Models.QueryFilters;

namespace QTFK.Services.DBIO.OleDB.Tests
{
    [TestClass]
    public class OleDBTests
    {
        private readonly string connectionString;
        private readonly CreateDrop createDrop;
        private readonly IDBIO db;

        public OleDBTests()
        {
            this.connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["tests"]?.ConnectionString;
            if (string.IsNullOrWhiteSpace(this.connectionString))
                throw new ArgumentException($"Empty or invalid 'tests' connection string in app.config", "tests");

            this.db = new OleDBIO(this.connectionString);
            this.createDrop = new CreateDrop(this.connectionString, this.db);
        }

        [TestInitialize()]
        public void Create()
        {
            this.createDrop.OleDB_Drop_tables();
            this.createDrop.OleDB_Create_tables();
        }


        [TestCleanup()]
        public void Drop()
        {
            this.createDrop.OleDB_Drop_tables();
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

            this.db.Set(cmd =>
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

                int id = Convert.ToInt32(this.db.GetLastID(cmd));
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
                this.db.Set(cmd =>
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

            this.db.Set($@"
                INSERT INTO persona (nombre, apellidos)
                VALUES (@nombre,@apellidos)
                ;", new Dictionary<string, object>
                {
                    { "@nombre", testPerson.Name },
                    { "@apellidos", testPerson.LastName },
                });

            this.db.Set($@"
                INSERT INTO persona (nombre, apellidos)
                VALUES (@nombre,@apellidos)
                ;", new Dictionary<string, object>
                {
                    { "@nombre", "Tronco" },
                    { "@apellidos", "Sanchez López" },
                });

            var personDB = this.db.Get($@" SELECT * FROM persona WHERE nombre = @nombre;",
                this.db.Params().Set("@nombre", testPerson.Name),
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
        [TestCategory("DB OleDB")]
        public void OleDB_Date_Time_field_test()
        {
            var pepePerson = new Person
            {
                Name = "pepe",
                LastName = "De la rosa Castaños",
                BirdhDate = new DateTime(2015, 10, 21, 23, 59, 58),
            };

            var troncoPerson = new Person
            {
                Name = "tronco",
            };

            this.db.Set($@"
                INSERT INTO persona (nombre, apellidos, fecha_nacimiento, hora_nacimiento)
                VALUES (@nombre,@apellidos, @fecha_nacimiento, @hora_nacimiento)
                ;", this.db.Params()
                    .Set("@nombre", pepePerson.Name)
                    .Set("@apellidos", pepePerson.LastName)
                    .SetDateTime("@fecha_nacimiento", "@hora_nacimiento", pepePerson.BirdhDate)
                );

            this.db.Set($@"
                INSERT INTO persona (nombre, apellidos, fecha_nacimiento, hora_nacimiento)
                VALUES (@nombre,@apellidos, @fecha_nacimiento, @hora_nacimiento)
                ;", this.db.Params()
                    .Set("@nombre", troncoPerson.Name)
                    .Set("@apellidos", troncoPerson.LastName)
                    .SetDateTime("@fecha_nacimiento", "@hora_nacimiento", troncoPerson.DeathDate)
                );

            var personsDB = this.db.Get($@" SELECT * FROM persona;", r => new Person
            {
                Name = r.Get<string>("nombre"),
                LastName = r.Get<string>("apellidos"),
                BirdhDate = r.GetDateTime("fecha_nacimiento", "hora_nacimiento"),
                DeathDate = r.GetNullableDateTime("fecha_nacimiento", "hora_nacimiento")
            });

            var pepeDB = personsDB.Single(p => p.Name == "pepe");
            var troncoDB = personsDB.Single(p => p.Name == "tronco");

            Assert.AreEqual(pepePerson.BirdhDate, pepeDB.BirdhDate);
            Assert.AreEqual(pepePerson.BirdhDate, pepeDB.DeathDate);
            Assert.AreEqual(troncoPerson.BirdhDate, troncoDB.BirdhDate);
            Assert.AreEqual(troncoPerson.DeathDate, troncoDB.DeathDate);
            Assert.IsNull(troncoDB.DeathDate);
        }

        [TestMethod]
        [TestCategory("DB OleDB")]
        public void OleDB_Get_T_error_test()
        {
            this.db.Set($@"
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
        [TestCategory("DB OleDB")]
        public void OleDB_Get_DataSet_test()
        {
            var testPerson = new Person
            {
                Name = "pepe",
                LastName = "De la rosa Castaños",
            };

            this.db.Set($@"
                INSERT INTO persona (nombre, apellidos)
                VALUES (@nombre,@apellidos)
                ;", new Dictionary<string, object>
                {
                    { "@nombre", testPerson.Name },
                    { "@apellidos", testPerson.LastName },
                });

            this.db.Set($@"
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
        [TestCategory("DB OleDB")]
        public void OleDB_Get_DataSet_error_test()
        {
            try
            {
                this.db.Get("Wroooong SQL statment");
                Assert.Fail("It was not expected to achieve this line!");
            }
            catch (DBIOException) { }
        }

        [TestMethod]
        [TestCategory("Mapping")]
        public void AutoMap_IDataRecord_tests()
        {
            this.db.Set($@"
                INSERT INTO persona (nombre, apellidos)
                VALUES (@nombre,@apellidos)
                ;", new Dictionary<string, object>
                {
                    { "@nombre", "Pepe" },
                    { "@apellidos", "De la rosa Castaños" },
                });

            this.db.Set($@"
                INSERT INTO persona (nombre, apellidos)
                VALUES (@nombre,@apellidos)
                ;", new Dictionary<string, object>
                {
                    { "@nombre", "Tronco" },
                    { "@apellidos", "Sanchez López" },
                });

            var data = this.db
                .Get($@" SELECT * FROM persona", r => r.AutoMap<DLPerson>())
                .ToList()
                ;

            Assert.AreEqual(2, data.Count());
            var testItem = data.Single(i => i.Nombre == "Tronco");
            Assert.AreEqual("Sanchez López", testItem.Apellidos);
            Assert.AreEqual(DateTime.MinValue, testItem.BirthDate);

            this.db.Set(cmd =>
            {
                data = cmd
                    .SetCommandText($@" SELECT * FROM persona")
                    .ExecuteReader()
                    .GetRecords()
                    .AutoMap<DLPerson>()
                    .ToList()
                    ;
            });

            Assert.AreEqual(2, data.Count());
            testItem = data.Single(i => i.Nombre == "Tronco");
            Assert.AreEqual("Sanchez López", testItem.Apellidos);
            Assert.AreEqual(DateTime.MinValue, testItem.BirthDate);

            this.db.Set(cmd =>
            {
                data = cmd
                    .SetCommandText($@" SELECT * FROM persona")
                    .ExecuteReader()
                    .GetRecords()
                    .AutoMap<DLPerson>((r, i) =>
                    {
                        i.BirthDate = DateTime.Today;
                    })
                    .ToList()
                    ;
            });

            Assert.AreEqual(2, data.Count());
            testItem = data.Single(i => i.Nombre == "Tronco");
            Assert.AreEqual("Sanchez López", testItem.Apellidos);
            Assert.AreEqual(DateTime.Today, testItem.BirthDate);
        }

        [TestMethod]
        [TestCategory("Mapping")]
        public void AutoMap_and_DBIO_IDataRecord_tests()
        {
            this.db.Set($@"
                INSERT INTO persona (nombre, apellidos)
                VALUES (@nombre,@apellidos)
                ;", new Dictionary<string, object>
                {
                    { "@nombre", "Pepe" },
                    { "@apellidos", "De la rosa Castaños" },
                });

            this.db.Set($@"
                INSERT INTO persona (nombre, apellidos)
                VALUES (@nombre,@apellidos)
                ;", new Dictionary<string, object>
                {
                    { "@nombre", "Tronco" },
                    { "@apellidos", "Sanchez López" },
                });

            var data = this.db
                .Get<DLPerson>($@" SELECT * FROM persona")
                .ToList()
                ;

            Assert.AreEqual(2, data.Count());
            var testItem = data.Single(i => i.Nombre == "Tronco");
            Assert.AreEqual("Sanchez López", testItem.Apellidos);
            Assert.AreEqual(DateTime.MinValue, testItem.BirthDate);

            data = this.db
                .Get<DLPerson>($@" SELECT * FROM persona WHERE nombre = @nombre", this.db.Params().Set("@nombre", "Pepe"))
                .ToList()
                ;

            Assert.AreEqual(1, data.Count());
            testItem = data.Single(i => i.Nombre == "Pepe");
            Assert.AreEqual("De la rosa Castaños", testItem.Apellidos);
            Assert.AreEqual(DateTime.MinValue, testItem.BirthDate);
        }

        [TestMethod]
        [TestCategory("DB OleDB")]
        public void QueryBuilder_OleBDInsert_tests()
        {
            IQueryFactory factory;
            IDBQueryInsert insert;
            IDBQuerySelect select;
            IDBQueryUpdate update;
            IDBQueryDelete delete;
            IKeyFilter filter;
            DLPerson testItem;
            IEnumerable<DLPerson> data;

            factory = OleDBQueryFactory.buildDefault();

            insert = factory
                .newInsert()
                .setTable("persona")
                .column("nombre", "Pepe")
                .column("apellidos", "De la rosa Castaños")
                ;

            this.db.Set(insert);

            insert = factory
                .newInsert()
                .setTable("persona")
                .column("nombre", null)
                .column("apellidos", null)
                ;

            var insertValuesCollection = new[]
            {
                new
                {
                    Name = "Rosa",
                    LastName = "Olmos Robles",
                },
                new
                {
                    Name = "Narciso",
                    LastName = "Céspedes De la higuera",
                },
            };

            foreach (var item in insertValuesCollection)
            {
                insert
                    .column("nombre", item.Name)
                    .column("apellidos", item.LastName);
                this.db.Set(insert);
            }

            insert
                .column("nombre", "Tronco")
                .column("apellidos", "Sanchez López");
            this.db.Set(insert);

            insert
                .column("nombre", "Louis")
                .column("apellidos", "Norton Smith");
            this.db.Set(insert);



            data = this.db
                .Get<DLPerson>($@" SELECT * FROM persona")
                .ToList()
                ;

            Assert.AreEqual(3, data.Count());

            testItem = data.Single(i => i.Nombre == "Tronco");
            Assert.AreEqual("Sanchez López", testItem.Apellidos);
            Assert.AreEqual(DateTime.MinValue, testItem.BirthDate);

            data = this.db
                .Get<DLPerson>($@" SELECT * FROM persona WHERE nombre = @nombre", this.db.Params().Set("@nombre", "Pepe"))
                .ToList()
                ;

            Assert.AreEqual(1, data.Count());
            testItem = data.Single(i => i.Nombre == "Pepe");
            Assert.AreEqual("De la rosa Castaños", testItem.Apellidos);
            Assert.AreEqual(DateTime.MinValue, testItem.BirthDate);


            //IDBQuery selects
            select = factory
                .newSelect()
                .setTable("persona")
                .column("nombre")
                .column("apellidos")
                ;

            data = this.db
                .Get<DLPerson>(select)
                .ToList()
                ;

            Assert.AreEqual(3, data.Count());
            testItem = data.Single(i => i.Nombre == "Tronco");
            Assert.AreEqual("Sanchez López", testItem.Apellidos);
            Assert.AreEqual(DateTime.MinValue, testItem.BirthDate);



            filter = factory.buildFilter<IKeyFilter>();
            filter.setKey("nombre", "Pepe");
            select.setFilter(filter);

            data = this.db
                .Get<DLPerson>(select)
                .ToList()
                ;

            Assert.AreEqual(1, data.Count());
            testItem = data.Single(i => i.Nombre == "Pepe");

            Assert.AreEqual("De la rosa Castaños", testItem.Apellidos);
            Assert.AreEqual(DateTime.MinValue, testItem.BirthDate);



            update = factory
                .newUpdate()
                .setTable("persona")
                .column("apellidos", "Ramírez de Villalobos")
                .setFilter(filter);

            filter.setKey("nombre", "Pepe");
            this.db.Set(update);

            select.Filter = filter;
            data = this.db
                .Get<DLPerson>(select)
                .ToList();

            Assert.AreEqual(1, data.Count());
            testItem = data.Single(i => i.Nombre == "Pepe");

            Assert.AreEqual("Ramírez de Villalobos", testItem.Apellidos);
            Assert.AreEqual(DateTime.MinValue, testItem.BirthDate);

            delete = factory
                .newDelete()
                .setTable("persona")
                .setFilter(filter);

            this.db.Set(delete);

            data = this.db
                .Get<DLPerson>(select)
                .ToList();

            Assert.AreEqual(0, data.Count());
        }
    }
}
