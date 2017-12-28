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
using QTFK.Models.DBIO;
using QTFK.Extensions.DBIO.DBQueries;
using QTFK.Extensions.DBIO.QueryFactory;
using QTFK.Extensions.Mapping.AutoMapping;
using QTFK.Services.QueryExecutors;

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

        [TestMethod]
        [TestCategory("DB OleDB")]
        public void QueryBuilder_SQL_tests_1()
        {
            IDBQuery insert;

            insert = new SqlInsertQuery()
                .setPrefix("qtfk.dbo.")
                .set("persona", c => c
                    .setColumn("nombre")
                    .setColumn("apellidos")
                );

            this.db.Set(insert, this.db.Params()
                .Set("@nombre", "Pepe")
                .Set("@apellidos", "De la rosa Castaños")
                );

            this.db.Set(insert, this.db.Params()
                .Set("@nombre", "Tronco")
                .Set("@apellidos", "Sanchez López")
                );

            this.db.Set(insert, this.db.Params()
                .Set("@nombre", "Louis")
                .Set("@apellidos", "Norton Smith")
                );

            //filter
            var filter = new SqlByParamEqualsFilter()
            {
                Field = "nombre",
                Parameter = "@nombre"
            };

            //selects
            var select = new SqlSelectQuery()
                    .setPrefix("qtfk.dbo.")
                    .setTable("persona")
                    .addColumn("*")
                    //.SetWhere("nombre = @nombre")
                    ;

            var data = this.db
                .Get<DLPerson>(select)
                .ToList()
                ;

            Assert.AreEqual(3, data.Count());
            var testItem = data.Single(i => i.Nombre == "Tronco");
            Assert.AreEqual("Sanchez López", testItem.Apellidos);

            select.setFilter(filter);

            data = this.db
                .Get<DLPerson>(select, this.db.Params().Set("@nombre", "Pepe"))
                .ToList()
                ;

            Assert.AreEqual(1, data.Count());
            testItem = data.Single(i => i.Nombre == "Pepe");
            Assert.AreEqual("De la rosa Castaños", testItem.Apellidos);

            //IDBQuery updates
            var update = new SqlUpdateQuery()
                .setPrefix("qtfk.dbo.")
                .set("persona", c => c
                    .setColumn("apellidos"))
                .setFilter(filter)
                ;

            this.db.Set(update, this.db.Params()
                .Set("@apellidos", "Ramírez de Villalobos")
                .Set("@nombre", "Pepe")
                );

            data = this.db
                .Get<DLPerson>(select, this.db.Params()
                .Set("@nombre", "Pepe"))
                .ToList()
                ;

            Assert.AreEqual(1, data.Count());
            testItem = data.Single(i => i.Nombre == "Pepe");
            Assert.AreEqual("Ramírez de Villalobos", testItem.Apellidos);

            var delete = new SqlDeleteQuery()
                .setPrefix("qtfk.dbo.")
                .setTable("persona")
                .setFilter(filter)
                ;

            this.db.Set(delete, this.db.Params().Set("@nombre", "Pepe"));

            data = this.db
                .Get<DLPerson>(select, this.db.Params().Set("@nombre", "Pepe"))
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
                .setPrefix("qtfk.dbo.")
                .set("persona", c => c
                    .setColumn("nombre")
                    .setColumn("apellidos")
                    );

            this.db.Set(insert, this.db.Params().Set("@nombre", "Pepe").Set("@apellidos", "De la rosa Castaños"));
            this.db.Set(insert, this.db.Params().Set("@nombre", "Tronco").Set("@apellidos", "Sanchez López"));
            this.db.Set(insert, this.db.Params().Set("@nombre", "Louis").Set("@apellidos", "Norton Smith"));

            insert = new SqlInsertQuery()
                .setPrefix("qtfk.dbo.")
                .set("etiqueta", c => c
                    .setColumn("nombre")
                );

            this.db.Set(insert, this.db.Params().Set("@nombre", "Ciencia"));
            this.db.Set(insert, this.db.Params().Set("@nombre", "Humor"));
            this.db.Set(insert, this.db.Params().Set("@nombre", "Youtube"));
            this.db.Set(insert, this.db.Params().Set("@nombre", "Crash"));

            var persons = this.db
                .Get<DLPerson>(new SqlSelectQuery()
                    .setPrefix("qtfk.dbo.")
                    .set("persona", c => c
                        .addColumn("*")
                    ))
                .ToList()
                ;

            var tags = this.db
                .Get<DLTag>(new SqlSelectQuery()
                    .setPrefix("qtfk.dbo.")
                    .setTable("etiqueta")
                    .addColumn("*"))
                .ToList()
                ;

            var pairs = tags
                .SelectMany(t => persons.Select(p => new { person_ID = p.Id, tag_ID = t.Id }))
                .ToList()
                ;

            insert = new SqlInsertQuery()
                .setPrefix("qtfk.dbo.")
                .set("etiquetas_personas", c => c
                    .setColumn("persona_id")
                    .setColumn("etiqueta_id")
                );

            foreach (var pair in pairs)
                this.db.Set(insert, this.db.Params().Set("@persona_id", pair.person_ID).Set("@etiqueta_id", pair.tag_ID));

            var select = new SqlSelectQuery()
                .setPrefix("qtfk.dbo.")
                .set("etiquetas_personas", c => c.addColumn("*"))
                .addJoin(JoinKind.Left, "etiqueta", m => m.addJoin("etiqueta_id", "id"), c => c
                    .addColumn("*")
                    .addColumn("nombre", "etiqueta_nombre")
                    )
                .addJoin(JoinKind.Left, "persona", "persona_id", "id", c => c
                    .addColumn("*")
                    .addColumn("nombre", "persona_nombre")
                    )
                ;

            string sql = select.Compile();

            var data = this.db
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
            var filter = new SqlByParamEqualsFilter()
            {
                Field = "nombre",
                Parameter = "@nombre"
            };

            //IDBQuery updates
            var update = new SqlUpdateQuery()
                .setPrefix("qtfk.dbo.")
                .set("persona", c => c
                    .setColumn("apellidos"))
                .setFilter(filter)
                ;

            this.db.Set(update, this.db.Params()
                .Set("@apellidos", "Ramírez de Villalobos")
                .Set("@nombre", "Pepe")
                );

            var selectPersons = new SqlSelectQuery()
                .setPrefix("qtfk.dbo.")
                .set("persona", c => c.addColumn("*"))
                .setFilter(filter)
                .setParam("@nombre", "Pepe")
                ;

            var person = this.db
                .Get<DLPerson>(selectPersons)
                .Single()
                ;

            Assert.AreEqual("Ramírez de Villalobos", person.Apellidos);

            var delete = new SqlDeleteQuery()
                .setPrefix("qtfk.dbo.")
                .setTable("persona")
                .setFilter(filter)
                .setParam("@nombre", "Pepe")
                ;

            this.db.Set(delete);

            data = this.db
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
        public void QueryFactory_tests_1()
        {
            IQueryFactory factory;
            IQueryExecutor executor;
            IDBQueryInsert insert;
            IDBQuerySelect select;
            IList<DLPerson> persons;
            IList<DLTag> tags;
            IByParamEqualsFilter filter;
            string sql;

            factory = SQLServerQueryFactory.buildDefault();
            factory.Prefix = "qtfk.dbo.";

            executor = new DefaultQueryExecutor(this.db, factory);

            insert = factory.newInsert()
                .set("persona", c => c
                    .setColumn("nombre")
                    .setColumn("apellidos")
                    );

            this.db.Set(insert, this.db.Params().Set("@nombre", "Pepe").Set("@apellidos", "De la rosa Castaños"));
            this.db.Set(insert, this.db.Params().Set("@nombre", "Tronco").Set("@apellidos", "Sanchez López"));
            this.db.Set(insert, this.db.Params().Set("@nombre", "Louis").Set("@apellidos", "Norton Smith"));

            insert = factory.newInsert()
                .set("etiqueta", c => c
                    .setColumn("nombre")
                );

            this.db.Set(insert, this.db.Params().Set("@nombre", "Ciencia"));
            this.db.Set(insert, this.db.Params().Set("@nombre", "Humor"));
            this.db.Set(insert, this.db.Params().Set("@nombre", "Youtube"));
            this.db.Set(insert, this.db.Params().Set("@nombre", "Crash"));

            persons = executor
                .select<DLPerson>(q => q
                    .set("persona", c => c
                        .addColumn("*")
                    ))
                .ToList()
                ;

            tags = executor
                .select<DLTag>(q => q
                    .setTable("etiqueta")
                    .addColumn("*"))
                .ToList()
                ;

            var pairs = tags
                .SelectMany(t => persons.Select(p => new { person_ID = p.Id, tag_ID = t.Id }))
                .ToList()
                ;

            insert = factory.newInsert()
                .set("etiquetas_personas", c => c
                    .setColumn("persona_id")
                    .setColumn("etiqueta_id")
                );

            foreach (var pair in pairs)
                this.db.Set(insert, this.db.Params().Set("@persona_id", pair.person_ID).Set("@etiqueta_id", pair.tag_ID));

            select = factory.newSelect()
                .set("etiquetas_personas", c => c.addColumn("*"))
                .addJoin(JoinKind.Left, "etiqueta", m => m.addJoin("etiqueta_id", "id"), c => c
                    .addColumn("*")
                    .addColumn("nombre", "etiqueta_nombre")
                    )
                .addJoin(JoinKind.Left, "persona", "persona_id", "id", c => c
                    .addColumn("*")
                    .addColumn("nombre", "persona_nombre")
                    )
                ;

            sql = select.Compile();

            var data = this.db
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
            filter = factory.buildFilter<IByParamEqualsFilter>();
            filter.Field = "nombre";
            filter.Parameter = "@nombre";

            //IDBQuery updates
            executor.update(q => q
                .set("persona", c => c
                    .setColumn("apellidos", "Ramírez de Villalobos"))
                .setFilter(filter)
                .setParam("@nombre", "Pepe")
                );

            var person = executor.select<DLPerson>(q => q
                .set("persona", c => c.addColumn("*"))
                .setFilter(filter)
                .setParam("@nombre", "Pepe")
                )
                .Single()
                ;

            Assert.AreEqual("Ramírez de Villalobos", person.Apellidos);

            executor.delete(q => q
                .setTable("persona")
                .setFilter(filter)
                .setParam("@nombre", "Pepe")
                );

            data = this.db
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
