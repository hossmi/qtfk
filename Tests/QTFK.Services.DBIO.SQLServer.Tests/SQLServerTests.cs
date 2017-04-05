using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QTFK.Services;
using QTFK.Services.DBIO;
using QTFK.Extensions.DBIO;
using System.Collections.Generic;
using System.Text;
using QTFK.Extensions.Collections.Filters;
using QTFK.Extensions.DBCommand;

namespace QTFK.Services.DBIO.SQLServer.Tests
{
    [TestClass]
    public class SQLServerTests
    {
        private readonly string _connectionString;
        private readonly IDBIO _db;

        public SQLServerTests()
        {
            _connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["tests"]?.ConnectionString;
            if (string.IsNullOrWhiteSpace(_connectionString))
                throw new ArgumentException($"Empty or invalid 'tests' connection string in app.config", "tests");

            _db = new QTFK.Services.DBIO.SQLServerDBIO(_connectionString);
        }

        [TestInitialize()]
        public void Create()
        {
            Drop();
            _db.SetInBlock(File.ReadBlocks("create.sql", "go"));
        }


        [TestCleanup()]
        public void Drop()
        {
            _db.SetIndividually(File.ReadBlocks("drop.sql", "go"), false);
        }

        [TestMethod]
        public void TestMethod1()
        {
            _db.Set(cmd =>
            {
                cmd.CommandText = $@"
                    USE qtfk
                    INSERT INTO persona (nombre, apellidos)
                    VALUES (@nombre,@apellidos)
                    ;";
                cmd.AddParameters(new Dictionary<string, object>
                {
                    { "@nombre", "pepe" },
                    { "@apellidos", "De la rosa Castaños" },
                });

                cmd.ExecuteNonQuery();
            });

            
        }
    }
}
