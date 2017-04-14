using QTFK.Models;
using System.Collections.Generic;
using System.Linq;

namespace QTFK.Services
{
    public interface IDBMigrator
    {
        IQueryable<MigrationInfo> GetMigrations();
        IEnumerable<MigrationInfo> Upgrade();
    }
}