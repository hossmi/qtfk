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
using QTFK.Extensions.Collections.Dictionaries;
using QTFK.Models.DBIO;
using QTFK.Extensions.DBIO.DBQueries;
using QTFK.Extensions.Mapping.AutoMapping;
using QTFK.Extensions.DBIO.QueryFactory;
using QTFK.Models.DBIO.Filters;

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

            var personDB = _db.Get($@" SELECT * FROM persona WHERE nombre = @nombre;",
                    _db.Params().Set("@nombre", testPerson.Name),
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

        [TestMethod]
        [TestCategory("DB OleDB")]
        public void QueryBuilder_SQL_tests_1()
        {
            IDBQuery insert;

            insert = new SqlInsertQuery()
                .SetPrefix("qtfk.dbo.")
                .Set("persona", c => c
                    .Column("nombre")
                    .Column("apellidos")
                );

            _db.Set(insert, _db.Params()
                .Set("@nombre", "Pepe")
                .Set("@apellidos", "De la rosa Castaños")
                );

            _db.Set(insert, _db.Params()
                .Set("@nombre", "Tronco")
                .Set("@apellidos", "Sanchez López")
                );

            _db.Set(insert, _db.Params()
                .Set("@nombre", "Louis")
                .Set("@apellidos", "Norton Smith")
                );

            //filter
            var filter = new SqlEqualsToParamFilter("nombre", "@nombre");

            //selects
            var select = new SqlSelectQuery()
                    .SetPrefix("qtfk.dbo.")
                    .SetTable("persona")
                    .AddColumn("*")
                    //.SetWhere("nombre = @nombre")
                    ;

            var data = _db
                .Get<DLPerson>(select)
                .ToList()
                ;

            Assert.AreEqual(3, data.Count());
            var testItem = data.Single(i => i.Nombre == "Tronco");
            Assert.AreEqual("Sanchez López", testItem.Apellidos);

            select.SetFilter(filter);

            data = _db
                .Get<DLPerson>(select, _db.Params().Set("@nombre", "Pepe"))
                .ToList()
                ;

            Assert.AreEqual(1, data.Count());
            testItem = data.Single(i => i.Nombre == "Pepe");
            Assert.AreEqual("De la rosa Castaños", testItem.Apellidos);

            //IDBQuery updates
            var update = new SqlUpdateQuery()
                .SetPrefix("qtfk.dbo.")
                .Set("persona", c => c
                    .Column("apellidos"))
                .SetFilter(filter)
                ;

            _db.Set(update, _db.Params()
                .Set("@apellidos", "Ramírez de Villalobos")
                .Set("@nombre", "Pepe")
                );

            data = _db
                .Get<DLPerson>(select, _db.Params()
                .Set("@nombre", "Pepe"))
                .ToList()
                ;

            Assert.AreEqual(1, data.Count());
            testItem = data.Single(i => i.Nombre == "Pepe");
            Assert.AreEqual("Ramírez de Villalobos", testItem.Apellidos);

            var delete = new SqlDeleteQuery()
                .SetPrefix("qtfk.dbo.")
                .SetTable("persona")
                .SetFilter(filter)
                ;

            _db.Set(delete, _db.Params().Set("@nombre", "Pepe"));

            data = _db
                .Get<DLPerson>(select, _db.Params().Set("@nombre", "Pepe"))
                .ToList()
                ;

            Assert.AreEqual(0, data.Count());
        }

        [TestMethod]
        [TestCategory("DB OleDB")]
        public void QueryBuilder_SQL_tests_2()
        {
            IDBQuery insert;

            insert = new SqlInsertQuery()
                .SetPrefix("qtfk.dbo.")
                .Set("persona", c => c
                    .Column("nombre")
                    .Column("apellidos")
                    );

            _db.Set(insert, _db.Params().Set("@nombre", "Pepe").Set("@apellidos", "De la rosa Castaños"));
            _db.Set(insert, _db.Params().Set("@nombre", "Tronco").Set("@apellidos", "Sanchez López"));
            _db.Set(insert, _db.Params().Set("@nombre", "Louis").Set("@apellidos", "Norton Smith"));

            insert = new SqlInsertQuery()
                .SetPrefix("qtfk.dbo.")
                .Set("etiqueta", c => c
                    .Column("nombre")
                );

            _db.Set(insert, _db.Params().Set("@nombre", "Ciencia"));
            _db.Set(insert, _db.Params().Set("@nombre", "Humor"));
            _db.Set(insert, _db.Params().Set("@nombre", "Youtube"));
            _db.Set(insert, _db.Params().Set("@nombre", "Crash"));

            var persons = _db
                .Get<DLPerson>(new SqlSelectQuery()
                    .SetPrefix("qtfk.dbo.")
                    .Select("persona", c => c
                        .Column("*")
                    ))
                .ToList()
                ;

            var tags = _db
                .Get<DLTag>(new SqlSelectQuery()
                    .SetPrefix("qtfk.dbo.")
                    .SetTable("etiqueta")
                    .AddColumn("*"))
                .ToList()
                ;

            var pairs = tags
                .SelectMany(t => persons.Select(p => new { person_ID = p.Id, tag_ID = t.Id }))
                .ToList()
                ;

            insert = new SqlInsertQuery()
                .SetPrefix("qtfk.dbo.")
                .Set("etiquetas_personas", c => c
                    .Column("persona_id")
                    .Column("etiqueta_id")
                );

            foreach (var pair in pairs)
                _db.Set(insert, _db.Params().Set("@persona_id", pair.person_ID).Set("@etiqueta_id", pair.tag_ID));

            var select = new SqlSelectQuery()
                .SetPrefix("qtfk.dbo.")
                .Select("etiquetas_personas", c => c.Column("*"))
                .AddJoin(JoinKind.Left, "etiqueta", m => m.Add("etiqueta_id", "id"), c => c
                    .Column("*")
                    .Column("nombre", "etiqueta_nombre")
                    )
                .AddJoin(JoinKind.Left, "persona", "persona_id", "id", c => c
                    .Column("*")
                    .Column("nombre", "persona_nombre")
                    )
                ;

            string sql = select.Compile();

            var data = _db
                .Get(select, r => new
                {
                    Person = r.AutoMap<DLPerson>(p => p.Nombre = r.Get<string>("persona_nombre")),
                    Tag = r.AutoMap<DLTag>(t => t.Nombre = r.Get<string>("etiqueta_nombre")),
                })
                .ToList()
                ;

            Assert.AreEqual(12, data.Count());
            Assert.AreEqual(4, data.Where(r => r.Person.Nombre == "Pepe").Count());
            Assert.AreEqual(3, data.Where(r => r.Tag.Nombre == "Youtube").Count());
            var testItem = data.Single(i => i.Person.Nombre == "Pepe" && i.Tag.Nombre == "Youtube");
            Assert.AreEqual("De la rosa Castaños", testItem.Person.Apellidos);

            //filter
            var filter = new SqlEqualsToParamFilter("nombre", "@nombre");

            //IDBQuery updates
            var update = new SqlUpdateQuery()
                .SetPrefix("qtfk.dbo.")
                .Set("persona", c => c
                    .Column("apellidos"))
                .SetFilter(filter)
                ;

            _db.Set(update, _db.Params()
                .Set("@apellidos", "Ramírez de Villalobos")
                .Set("@nombre", "Pepe")
                );

            var selectPersons = new SqlSelectQuery()
                .SetPrefix("qtfk.dbo.")
                .Select("persona", c => c.Column("*"))
                .SetFilter(filter)
                .SetParam("@nombre", "Pepe")
                ;

            var person = _db
                .Get<DLPerson>(selectPersons)
                .Single()
                ;

            Assert.AreEqual("Ramírez de Villalobos", person.Apellidos);

            var delete = new SqlDeleteQuery()
                .SetPrefix("qtfk.dbo.")
                .SetTable("persona")
                .SetFilter(filter)
                .SetParam("@nombre", "Pepe")
                ;

            _db.Set(delete);

            data = _db
                .Get(select, r => new
                {
                    Person = r.AutoMap<DLPerson>(p => p.Nombre = r.Get<string>("persona_nombre")),
                    Tag = r.AutoMap<DLTag>(t => t.Nombre = r.Get<string>("etiqueta_nombre")),
                })
                .ToList()
                ;

            Assert.AreEqual(8, data.Count());
            Assert.AreEqual(0, data.Where(r => r.Person.Nombre == "Pepe").Count());
            Assert.AreEqual(2, data.Where(r => r.Tag.Nombre == "Youtube").Count());
        }

        [TestMethod]
        [TestCategory("DB OleDB")]
        public void QueryBuilder_SQL_CrudFactory_tests_1()
        {
            var factory = new SQLServerQueryFactory(_db);

            var insert = factory.NewInsert()
                .SetPrefix("qtfk.dbo.")
                .Set("persona", c => c
                    .Column("nombre")
                    .Column("apellidos")
                    );

            _db.Set(insert, _db.Params().Set("@nombre", "Pepe").Set("@apellidos", "De la rosa Castaños"));
            _db.Set(insert, _db.Params().Set("@nombre", "Tronco").Set("@apellidos", "Sanchez López"));
            _db.Set(insert, _db.Params().Set("@nombre", "Louis").Set("@apellidos", "Norton Smith"));

            insert = factory.NewInsert()
                .SetPrefix("qtfk.dbo.")
                .Set("etiqueta", c => c
                    .Column("nombre")
                );

            _db.Set(insert, _db.Params().Set("@nombre", "Ciencia"));
            _db.Set(insert, _db.Params().Set("@nombre", "Humor"));
            _db.Set(insert, _db.Params().Set("@nombre", "Youtube"));
            _db.Set(insert, _db.Params().Set("@nombre", "Crash"));

            var persons = factory
                .Select<DLPerson>(q => q
                    .SetPrefix("qtfk.dbo.")
                    .Select("persona", c => c
                        .Column("*")
                    ))
                .ToList()
                ;

            var tags = factory
                .Select<DLTag>(q => q
                    .SetPrefix("qtfk.dbo.")
                    .SetTable("etiqueta")
                    .AddColumn("*"))
                .ToList()
                ;

            var pairs = tags
                .SelectMany(t => persons.Select(p => new { person_ID = p.Id, tag_ID = t.Id }))
                .ToList()
                ;

            insert = factory.NewInsert()
                .SetPrefix("qtfk.dbo.")
                .Set("etiquetas_personas", c => c
                    .Column("persona_id")
                    .Column("etiqueta_id")
                );

            foreach (var pair in pairs)
                _db.Set(insert, _db.Params().Set("@persona_id", pair.person_ID).Set("@etiqueta_id", pair.tag_ID));

            var select = factory.NewSelect()
                .SetPrefix("qtfk.dbo.")
                .Select("etiquetas_personas", c => c.Column("*"))
                .AddJoin(JoinKind.Left, "etiqueta", m => m.Add("etiqueta_id", "id"), c => c
                    .Column("*")
                    .Column("nombre", "etiqueta_nombre")
                    )
                .AddJoin(JoinKind.Left, "persona", "persona_id", "id", c => c
                    .Column("*")
                    .Column("nombre", "persona_nombre")
                    )
                ;

            string sql = select.Compile();

            var data = _db
                .Get(select, r => new
                {
                    Person = r.AutoMap<DLPerson>(p => p.Nombre = r.Get<string>("persona_nombre")),
                    Tag = r.AutoMap<DLTag>(t => t.Nombre = r.Get<string>("etiqueta_nombre")),
                })
                .ToList()
                ;

            Assert.AreEqual(12, data.Count());
            Assert.AreEqual(4, data.Where(r => r.Person.Nombre == "Pepe").Count());
            Assert.AreEqual(3, data.Where(r => r.Tag.Nombre == "Youtube").Count());
            var testItem = data.Single(i => i.Person.Nombre == "Pepe" && i.Tag.Nombre == "Youtube");
            Assert.AreEqual("De la rosa Castaños", testItem.Person.Apellidos);

            //filter
            var filter = new SqlEqualsToParamFilter("nombre", "@nombre");

            //IDBQuery updates
            factory.Update(q => q
                .SetPrefix("qtfk.dbo.")
                .Set("persona", c => c
                    .Column("apellidos", "Ramírez de Villalobos"))
                .SetFilter(filter)
                .SetParam("@nombre", "Pepe")
                );

            var person = factory.Select<DLPerson>(q => q
                .SetPrefix("qtfk.dbo.")
                .Select("persona", c => c.Column("*"))
                .SetFilter(filter)
                .SetParam("@nombre", "Pepe")
                )
                .Single()
                ;

            Assert.AreEqual("Ramírez de Villalobos", person.Apellidos);

            factory.Delete(q => q
                .SetPrefix("qtfk.dbo.")
                .SetTable("persona")
                .SetFilter(filter)
                .SetParam("@nombre", "Pepe")
                );

            data = _db
                .Get(select, r => new
                {
                    Person = r.AutoMap<DLPerson>(p => p.Nombre = r.Get<string>("persona_nombre")),
                    Tag = r.AutoMap<DLTag>(t => t.Nombre = r.Get<string>("etiqueta_nombre")),
                })
                .ToList()
                ;

            Assert.AreEqual(8, data.Count());
            Assert.AreEqual(0, data.Where(r => r.Person.Nombre == "Pepe").Count());
            Assert.AreEqual(2, data.Where(r => r.Tag.Nombre == "Youtube").Count());
        }
    }
}
