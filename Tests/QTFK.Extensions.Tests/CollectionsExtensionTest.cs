using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QTFK.Extensions.Collections.SwitchCase;
using System.Collections.Generic;
using System.Linq;

namespace QTFK.Extensions.Tests
{
    [TestClass]
    public class CollectionsExtensionTest
    {
        [TestMethod]
        [TestCategory("Extensions")]
        public void SwitchCase_extension_test()
        {
            var persons = new[]
            {
                new
                {
                    Name = "pepe",
                    LastName = "Flores Silvestre",
                    Age = 15,
                    CurrentJob = (string)null
                },
                new
                {
                    Name = "Rosa",
                    LastName = "De los vientos Céspedes",
                    Age = 21,
                    CurrentJob = "Actress"
                },
                new
                {
                    Name = "Florencio",
                    LastName = "Espinosa De las rosas",
                    Age = 34,
                    CurrentJob = (string)null
                },
                new
                {
                    Name = "John",
                    LastName = "Del Bosque Huertas",
                    Age = 67,
                    CurrentJob = "Developer"
                },
            };

            var cases = persons
                .Case(i => i.Age < 18, i => $"{i.Name} is not an adult.") //1
                .Case(i => i.Age >= 18 && !string.IsNullOrWhiteSpace(i.CurrentJob), i => $"{i.Name} is employed as {i.CurrentJob}.") //2
                .Case(i => i.Age >= 18 && string.IsNullOrWhiteSpace(i.CurrentJob), i => $"{i.Name} is unemployed.") //1
                .Case(i => i.Age >= 65 && string.IsNullOrWhiteSpace(i.CurrentJob), i => $"{i.Name} is retired.") //0
                .Case(i => i.Age >= 65, i => $"{i.Name} is in age of retirement.") //1
                ;

            var jobs = cases
                .CaseElse(int.MaxValue)
                .ToList()
                ;

            Assert.AreEqual(5, jobs.Count());
            Assert.AreEqual(1, jobs.Count(s => s.Contains(" is not an adult.")));
            Assert.AreEqual(2, jobs.Count(s => s.Contains(" is employed as ")));
            Assert.AreEqual(1, jobs.Count(s => s.Contains(" is unemployed.")));
            Assert.AreEqual(0, jobs.Count(s => s.Contains(" is retired.")));
            Assert.AreEqual(1, jobs.Count(s => s.Contains(" is in age of retirement.")));

            jobs = cases
                .CaseElse()
                .ToList()
                ;

            Assert.AreEqual(4, jobs.Count());
            Assert.AreEqual(1, jobs.Count(s => s.Contains(" is not an adult.")));
            Assert.AreEqual(2, jobs.Count(s => s.Contains(" is employed as ")));
            Assert.AreEqual(1, jobs.Count(s => s.Contains(" is unemployed.")));
            Assert.AreEqual(0, jobs.Count(s => s.Contains(" is retired.")));
            Assert.AreEqual(0, jobs.Count(s => s.Contains(" is in age of retirement.")));

            // from anonymous to anonymous switch-case test
            var filteredPersons = persons
                .Case(p => p.Age >= 18 && !string.IsNullOrWhiteSpace(p.CurrentJob), p => new
                {
                    FullName = $"{p.LastName}, {p.Name}",
                    Status = "Employed",
                })
                .Case(p => p.CurrentJob != null && p.CurrentJob == "Developer", p => new
                {
                    FullName = $"{p.LastName}, {p.Name}",
                    Status = "Developer",
                })
                .CaseElse(int.MaxValue)
                .ToList()
                ;

            Assert.AreEqual(3, filteredPersons.Count());
            Assert.AreEqual(2, filteredPersons.Count(s => s.Status == "Employed"));
            Assert.AreEqual(1, filteredPersons.Count(s => s.Status == "Developer"));
            Assert.AreEqual(2, filteredPersons.Count(s => s.FullName == "Del Bosque Huertas, John"));

            var emptyPersons = persons
                .Case(p => !string.IsNullOrWhiteSpace(p.CurrentJob), p => $"Employed {p.Name}")
                .Case(p => p.Age < 18, p => $"Children {p.Name}")
                .CaseElse(p => $"Other {p.Name}")
                .ToList()
                ;

            Assert.AreEqual(4, emptyPersons.Count());
            Assert.AreEqual(2, emptyPersons.Count(s => s.StartsWith("Employed")));
            Assert.AreEqual(1, emptyPersons.Count(s => s.StartsWith("Children")));
            Assert.AreEqual(1, emptyPersons.Count(s => s.StartsWith("Other")));
        }
    }
}
