using Microsoft.VisualStudio.TestTools.UnitTesting;
using QTFK.Extensions;
using System.Linq;

namespace QTFK.Cmd.Tests
{
    [TestClass]
    public class CollectionsExtensionTest
    {
        [TestMethod]
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
                .Case(i => $"{i.Name} is not an adult.", i => i.Age < 18) //1
                .Case(i => $"{i.Name} is employed as {i.CurrentJob}.", i => i.Age >= 18 && !string.IsNullOrWhiteSpace(i.CurrentJob)) //2
                .Case(i => $"{i.Name} is unemployed.", i => i.Age >= 18 && string.IsNullOrWhiteSpace(i.CurrentJob)) //1
                .Case(i => $"{i.Name} is retired.", i => i.Age >= 65 && string.IsNullOrWhiteSpace(i.CurrentJob)) //0
                .Case(i => $"{i.Name} is in age of retirement.", i => i.Age >= 65) //1
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
                .Case(p => new
                {
                    FullName = $"{p.LastName}, {p.Name}",
                    Status = "Employed",
                }, p => p.Age >= 18 && !string.IsNullOrWhiteSpace(p.CurrentJob))
                .Case(p => new
                {
                    FullName = $"{p.LastName}, {p.Name}",
                    Status = "Developer",
                }, p => p.CurrentJob != null && p.CurrentJob == "Developer")
                .CaseElse(int.MaxValue)
                .ToList()
                ;

            Assert.AreEqual(3, filteredPersons.Count());
            Assert.AreEqual(2, filteredPersons.Count(s => s.Status == "Employed"));
            Assert.AreEqual(1, filteredPersons.Count(s => s.Status == "Developer"));
            Assert.AreEqual(2, filteredPersons.Count(s => s.FullName == "Del Bosque Huertas, John"));

            var emptyPersons = persons
                .Case(p => $"Employed {p.Name}", p => !string.IsNullOrWhiteSpace(p.CurrentJob))
                .Case(p => $"Children {p.Name}", p => p.Age < 18)
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
