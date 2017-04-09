using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using QTFK.Extensions.DataSets;
using QTFK.Extensions.Tests.Models;
using System.Linq;
using QTFK.Extensions.Mapping.AutoMapping;
using System.Collections.Generic;

namespace QTFK.Extensions.Tests
{
    [TestClass]
    public class AutoMapExtensionTests
    {
        const string kNombre = "nombre";
        const string kApellidos = "apellidos";
        const string kFecha_nacimiento = "fecha_nacimiento";
        const string kHora_nacimiento = "hora_nacimiento";
        const string kNumero = "numero";
        const string kDouble = "numero_real";
        const string kDecimal = "numero_decimal";

        [TestMethod]
        [TestCategory("Mapping")]
        public void AutoMap_tests()
        {
            var ds = new DataSet();
            var table = ds.Tables.Add();
            table.Columns.Add(nameof(SimpleTestClass.Name), typeof(string));
            table.Columns.Add(nameof(SimpleTestClass.LastName), typeof(string));
            table.Columns.Add(nameof(SimpleTestClass.BirthDate), typeof(DateTime));
            table.Columns.Add(nameof(SimpleTestClass.DecimalNumber), typeof(decimal));
            table.Columns.Add(kDouble, typeof(double));
            table.Columns.Add(kNumero, typeof(int));

            var row = table.NewRow();
            row[nameof(SimpleTestClass.Name)] = "Pepe";
            row[nameof(SimpleTestClass.LastName)] = "De la Rosa Martínez";
            row[nameof(SimpleTestClass.BirthDate)] = new DateTime(1980, 12, 31);
            row[nameof(SimpleTestClass.DecimalNumber)] = decimal.MaxValue;
            row[kNumero] = 666;
            row[kDouble] = Math.PI;
            table.Rows.Add(row);

            row = table.NewRow();
            row[nameof(SimpleTestClass.Name)] = "Tronco";
            row[nameof(SimpleTestClass.LastName)] = "Ramírez Villalobos";
            row[nameof(SimpleTestClass.BirthDate)] = new DateTime(1955, 10, 12);
            row[nameof(SimpleTestClass.DecimalNumber)] = 666m;
            row[kNumero] = 512;
            row[kDouble] = Math.E;
            table.Rows.Add(row);

            var items = ds
                .AsTable()
                .AsEnumerable()
                .AutoMap<SimpleTestClass>()
                .ToList()
                ;

            Assert.AreEqual(2, items.Count());

            var itemPepe = items.Single(r => r.Name == "Pepe");
            Assert.AreEqual("De la Rosa Martínez", itemPepe.LastName);
            Assert.AreEqual(new DateTime(1980, 12, 31), itemPepe.BirthDate);
            Assert.AreEqual(decimal.MaxValue, itemPepe.DecimalNumber);
            Assert.AreEqual(0.0, itemPepe.DoubleNumber);
            Assert.AreEqual(0, itemPepe.Number);

            var itemTronco = items.Single(r => r.Name == "Tronco");
            Assert.AreEqual("Ramírez Villalobos", itemTronco.LastName);
            Assert.AreEqual(new DateTime(1955, 10, 12), itemTronco.BirthDate);
            Assert.AreEqual(666m, itemTronco.DecimalNumber);
            Assert.AreEqual(0.0, itemTronco.DoubleNumber);
            Assert.AreEqual(0, itemTronco.Number);

            items = ds
                .AsTable()
                .AsEnumerable()
                .AutoMap<SimpleTestClass>((r,i) =>
                {
                    i.DoubleNumber = r.Get<double>(kDouble);
                    i.Number = r.Get<int>(kNumero);
                })
                .ToList()
                ;

            Assert.AreEqual(2, items.Count());

            itemPepe = items.Single(r => r.Name == "Pepe");
            Assert.AreEqual("De la Rosa Martínez", itemPepe.LastName);
            Assert.AreEqual(new DateTime(1980, 12, 31), itemPepe.BirthDate);
            Assert.AreEqual(decimal.MaxValue, itemPepe.DecimalNumber);
            Assert.AreEqual(666, itemPepe.Number);
            Assert.AreEqual(Math.PI, itemPepe.DoubleNumber);

            itemTronco = items.Single(r => r.Name == "Tronco");
            Assert.AreEqual("Ramírez Villalobos", itemTronco.LastName);
            Assert.AreEqual(new DateTime(1955, 10, 12), itemTronco.BirthDate);
            Assert.AreEqual(666m, itemTronco.DecimalNumber);
            Assert.AreEqual(Math.E, itemTronco.DoubleNumber);
            Assert.AreEqual(512, itemTronco.Number);

        }
    }
}
