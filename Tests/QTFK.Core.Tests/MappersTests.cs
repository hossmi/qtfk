using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using QTFK.Services.EntityMappers;
using QTFK.Core.Tests.Models;
using System.Data;
using QTFK.Extensions.DataSets;
using QTFK.Extensions.Mapping;

namespace QTFK.Core.Tests
{
    [TestClass]
    public class MappersTests
    {
        const string kNombre = "nombre";
        const string kApellidos = "apellidos";
        const string kFecha_nacimiento = "fecha_nacimiento";
        const string kHora_nacimiento = "hora_nacimiento";
        const string kNumero = "numero";
        const string kDouble = "numero_real";
        const string kDecimal = "numero_decimal";

        [TestMethod]
        public void EntityMapper_tests()
        {
            var mapper = new DefaultEntityMapper();
            mapper.Register<DataRow, SimpleTestClass>(r => new SimpleTestClass
            {
                Name = (string)r[kNombre],
                LastName = (string)r[kApellidos],
                BirthDate = ((DateTime)r[kFecha_nacimiento]).Date + (TimeSpan)r[kHora_nacimiento],
                Number = (int)r[kNumero],
                DoubleNumber = (double)r[kDouble],
                DecimalNumber = (decimal)r[kDecimal],
            });

            mapper.Register<SimpleTestClass, IDictionary<string, object>>(item => new Dictionary<string, object>
            {
                { kNombre, item.Name },
                { kApellidos, item.LastName },
                { kFecha_nacimiento, item.BirthDate.Date },
                { kHora_nacimiento, item.BirthDate.TimeOfDay },
                { kNumero, item.Number },
                { kDouble, item.DoubleNumber },
                { kDecimal, item.DecimalNumber },
            });

            var ds = new DataSet();
            var table = ds.Tables.Add();
            table.Columns.Add(kNombre, typeof(string));
            table.Columns.Add(kApellidos, typeof(string));
            table.Columns.Add(kFecha_nacimiento, typeof(DateTime));
            table.Columns.Add(kHora_nacimiento, typeof(TimeSpan));
            table.Columns.Add(kNumero, typeof(int));
            table.Columns.Add(kDouble, typeof(double));
            table.Columns.Add(kDecimal, typeof(decimal));

            var row = table.NewRow();
            row[kNombre] = "Pepe";
            row[kApellidos] = "De la Rosa Martínez";
            row[kFecha_nacimiento] = new DateTime(1980, 12, 31);
            row[kHora_nacimiento] = new TimeSpan(23, 59, 59);
            row[kNumero] = int.MaxValue;
            row[kDouble] = Math.PI;
            row[kDecimal] = decimal.MaxValue;

            table.Rows.Add(row);

            var items = table
                .AsEnumerable()
                .Select(mapper.Map<DataRow, SimpleTestClass>)
                .ToList()
                ;

            SimpleTestClass itemDB = items.First();

            Assert.AreEqual("Pepe", itemDB.Name);
            Assert.AreEqual(new DateTime(1980, 12, 31, 23, 59, 59), itemDB.BirthDate);
            Assert.AreEqual(decimal.MaxValue, itemDB.DecimalNumber);
            Assert.AreEqual(Math.PI, itemDB.DoubleNumber);

            var dictionaries1 = items
                .Select(mapper.Map<SimpleTestClass, IDictionary<string, object>>)
                .ToList()
                ;

            var dicDB = dictionaries1.First();

            Assert.AreEqual("Pepe", dicDB[kNombre]);
            Assert.AreEqual(new DateTime(1980, 12, 31), dicDB[kFecha_nacimiento]);
            Assert.AreEqual(new TimeSpan(23, 59, 59), dicDB[kHora_nacimiento]);
            Assert.AreEqual(decimal.MaxValue, dicDB[kDecimal]);
            Assert.AreEqual(Math.PI, dicDB[kDouble]);
        }

        [TestMethod]
        public void EntityMapperExtension_tests()
        {
            var mapper = new DefaultEntityMapper();
            EntityMapperExtension.Mapper = mapper;
            EntityMapperExtension.Mapper.Register<DataRow, SimpleTestClass>(r => new SimpleTestClass
            {
                Name = r.Get<string>(kNombre),
                LastName = r.Get<string>(kApellidos),
                BirthDate = r.Get<DateTime>(kFecha_nacimiento).Date + r.Get<TimeSpan>(kHora_nacimiento),
                Number = r.Get<int>(kNumero),
                DoubleNumber = r.Get<double>(kDouble),
                DecimalNumber = r.Get<decimal>(kDecimal),
            });

            EntityMapperExtension.Mapper.Register<SimpleTestClass, IDictionary<string, object>>(item => new Dictionary<string, object>
            {
                { kNombre, item.Name },
                { kApellidos, item.LastName },
                { kFecha_nacimiento, item.BirthDate.Date },
                { kHora_nacimiento, item.BirthDate.TimeOfDay },
                { kNumero, item.Number },
                { kDouble, item.DoubleNumber },
                { kDecimal, item.DecimalNumber },
            });

            var ds = new DataSet();
            var table = ds.Tables.Add();
            table.Columns.Add(kNombre, typeof(string));
            table.Columns.Add(kApellidos, typeof(string));
            table.Columns.Add(kFecha_nacimiento, typeof(DateTime));
            table.Columns.Add(kHora_nacimiento, typeof(TimeSpan));
            table.Columns.Add(kNumero, typeof(int));
            table.Columns.Add(kDouble, typeof(double));
            table.Columns.Add(kDecimal, typeof(decimal));

            var row = table.NewRow();
            row[kNombre] = "Pepe";
            row[kApellidos] = "De la Rosa Martínez";
            row[kFecha_nacimiento] = new DateTime(1980, 12, 31);
            row[kHora_nacimiento] = new TimeSpan(23, 59, 59);
            row[kNumero] = int.MaxValue;
            row[kDouble] = Math.PI;
            row[kDecimal] = decimal.MaxValue;

            table.Rows.Add(row);

            var items = ds
                .AsTable()
                .AsEnumerable()
                .Map<DataRow, SimpleTestClass>()
                .ToList()
                ;

            SimpleTestClass itemDB = items.First();

            Assert.AreEqual("Pepe", itemDB.Name);
            Assert.AreEqual(new DateTime(1980, 12, 31, 23, 59, 59), itemDB.BirthDate);
            Assert.AreEqual(decimal.MaxValue, itemDB.DecimalNumber);
            Assert.AreEqual(Math.PI, itemDB.DoubleNumber);

            var items2 = ds
                .AsTable()
                .AsEnumerable()
                .Select(mapper.Map<DataRow, SimpleTestClass>)
                .ToList()
                ;

            itemDB = items2.First();

            Assert.AreEqual("Pepe", itemDB.Name);
            Assert.AreEqual(new DateTime(1980, 12, 31, 23, 59, 59), itemDB.BirthDate);
            Assert.AreEqual(decimal.MaxValue, itemDB.DecimalNumber);
            Assert.AreEqual(Math.PI, itemDB.DoubleNumber);

            var dictionaries1 = items2
                .Map<SimpleTestClass, IDictionary<string, object>>()
                .ToList()
                ;

            var dicDB = dictionaries1.First();

            Assert.AreEqual("Pepe", dicDB[kNombre]);
            Assert.AreEqual(new DateTime(1980, 12, 31), dicDB[kFecha_nacimiento]);
            Assert.AreEqual(new TimeSpan(23, 59, 59), dicDB[kHora_nacimiento]);
            Assert.AreEqual(decimal.MaxValue, dicDB[kDecimal]);
            Assert.AreEqual(Math.PI, dicDB[kDouble]);

            var dictionaries2 = items2
                .Select(mapper.Map<SimpleTestClass, IDictionary<string, object>>)
                .ToList()
                ;

            dicDB = dictionaries2.First();

            Assert.AreEqual("Pepe", dicDB[kNombre]);
            Assert.AreEqual(new DateTime(1980, 12, 31), dicDB[kFecha_nacimiento]);
            Assert.AreEqual(new TimeSpan(23, 59, 59), dicDB[kHora_nacimiento]);
            Assert.AreEqual(decimal.MaxValue, dicDB[kDecimal]);
            Assert.AreEqual(Math.PI, dicDB[kDouble]);
        }
    }
}
