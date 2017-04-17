using System;

namespace QTFK.Services.DBIO.OleDB.Tests.Models
{
    public class Person
    {
        public DateTime BirdhDate { get; set; }
        public DateTime? DeathDate { get; set; }
        public string LastName { get; set; }
        public string Name { get; set; }
    }

    public class DLPerson
    {
        public string Apellidos { get; set; }
        public string Nombre { get; set; }
        public DateTime BirthDate { get; set; }
    }
}