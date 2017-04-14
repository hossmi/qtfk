
using QTFK.Models;
using QTFK.Services;
using System.Linq;

namespace QTFK.Extensions.DBIOMigrationExtensions
{

    public static class DBMigrationExtension
    {
        public static int GetCurrentVersion(this IDBMigrator migrator)
        {
            return GetLastMigration(migrator).Version;
        }

        public static MigrationInfo GetLastMigration(this IDBMigrator migrator)
        {
            return migrator.GetMigrations()
                .OrderByDescending(m => m.MigrationDate)
                .DefaultIfEmpty(new MigrationInfo
                {
                    Description = string.Empty,
                    Version = 0,
                    MigrationDate = new System.DateTime(0)
                })
                .FirstOrDefault()
                ;
        }
    }
}