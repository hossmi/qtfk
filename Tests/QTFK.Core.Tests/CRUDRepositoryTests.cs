﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QTFK.Models;
using System.Collections.Generic;
using QTFK.Core.Tests.Models;
using QTFK.Services;
using QTFK.Services.ExplicitKeyRepositories;
using System.Linq;

namespace QTFK.Core.Tests
{
    [TestClass]
    public class CRUDRepositoryTests
    {
        [TestMethod]
        [TestCategory("Repository")]
        public void CRUDRepository_test_1()
        {
            IExplicitKeyRepository<string, SimpleTestClass> repo = new InMemoryInlineRepository<string, SimpleTestClass>(
                new Dictionary<string,SimpleTestClass>()
                , item => item.LastName
                , item => !string.IsNullOrWhiteSpace(item.LastName)
                , (source, target) =>
                {
                    target.BirthDate = source.BirthDate;
                    target.DecimalNumber = source.DecimalNumber;
                    target.DoubleNumber = source.DoubleNumber;
                    target.Name = source.Name;
                    target.Number = source.Number;
                });

            Assert.IsFalse(repo.Get(i => true).AsEnumerable().Any());

            var item1 = new SimpleTestClass
            {
                BirthDate = DateTime.Today,
                Name = "Pepe",
                LastName = "Sanchez Villalobos",
                Number = 3,
                DecimalNumber = 3.1415926535m,
                DoubleNumber = Math.E,
            };

            Assert.IsFalse(repo.Set(item1));
            Assert.AreEqual("Sanchez Villalobos", repo.Add(item1));
            Assert.AreEqual(1, repo.Get(i => true).AsEnumerable().Count());

            item1.DoubleNumber = Math.PI;
            Assert.IsTrue(repo.Set(item1));
            var item2 = repo.Get(item1.LastName);
            Assert.AreEqual(Math.PI, item2.DoubleNumber);

            Assert.IsFalse(ReferenceEquals(item1, item2));

            item2.LastName = "López López";
            repo.Add(item2);

            Assert.AreEqual(2, repo.Get(i => true).AsEnumerable().Count());

            repo.Remove(item1.LastName);
            Assert.AreEqual(1, repo.Get(i => true).AsEnumerable().Count());
        }

        [TestMethod]
        [TestCategory("Repository")]
        public void CRUDRepository_test_2()
        {
            IExplicitKeyRepository<decimal, SimpleTestClass> repo = new InMemoryInlineRepository<decimal, SimpleTestClass>(
                new Dictionary<decimal, SimpleTestClass>()
                , item => item.DecimalNumber
                , item => item.DecimalNumber > 0
                , (source, target) =>
                {
                    target.BirthDate = source.BirthDate;
                    target.DoubleNumber = source.DoubleNumber;
                    target.Name = source.Name;
                    target.LastName = source.LastName;
                    target.Number = source.Number;
                    target.DecimalNumber = source.DecimalNumber;
                }
                , item => DateTime.Now.Ticks
                );

            Assert.IsFalse(repo.Get(i => true).AsEnumerable().Any());

            var item1 = new SimpleTestClass
            {
                BirthDate = DateTime.Today,
                Name = "Pepe",
                LastName = "Sanchez Villalobos",
                Number = 3,
                DecimalNumber = 3.1415926535m,
                DoubleNumber = Math.E,
            };

            Assert.IsFalse(repo.Set(item1));

            try
            {
                repo.Add(item1);
                Assert.Fail($"Expected exception adding item with setted id on repository with auto id");
            }
            catch {}

            item1.DecimalNumber = 0m;
            item1.DecimalNumber = repo.Add(item1);
            Assert.IsTrue(item1.DecimalNumber > 0m);
            Assert.AreEqual(1, repo.Get(i => true).AsEnumerable().Count());

            item1.DoubleNumber = Math.PI;
            Assert.IsTrue(repo.Set(item1));
            var item2 = repo.Get(item1.DecimalNumber);
            Assert.AreEqual(Math.PI, item2.DoubleNumber);

            Assert.IsFalse(ReferenceEquals(item1, item2));

            item2.DecimalNumber = 0m;
            var id2 = repo.Add(item2);
            Assert.AreNotEqual(id2, item1.DecimalNumber);

            Assert.AreEqual(2, repo.Get(i => true).AsEnumerable().Count());

            repo.Remove(item1.DecimalNumber);
            Assert.AreEqual(1, repo.Get(i => true).AsEnumerable().Count());
        }
    }
}
