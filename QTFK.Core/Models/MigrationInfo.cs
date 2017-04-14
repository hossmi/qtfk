using System;

namespace QTFK.Models
{
    public class MigrationInfo
    {
        public int Version { get; set; }
        public DateTime MigrationDate { get; set; }
        public string Description { get; set; }
    }
}