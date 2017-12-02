using QTFK.Models;
using System.Collections.Generic;

namespace QTFK.Services
{
    public interface IDBMigrator
    {
        IEnumerable<MigrationInfo> GetMigrations();
        IEnumerable<MigrationInfo> Upgrade();
        void UnInstall();
    }
}