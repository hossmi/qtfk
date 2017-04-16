using QTFK.Models;
using System.Collections.Generic;
using System.Linq;

namespace QTFK.Services
{
    public interface IDBMigrator
    {
        IEnumerable<MigrationInfo> GetMigrations();
        IEnumerable<MigrationInfo> Upgrade();
    }
}