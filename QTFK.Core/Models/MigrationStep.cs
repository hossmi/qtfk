using System;
using QTFK.Models;
using QTFK.Services;

namespace QTFK.Models
{
    public class MigrationStep : IMigrationStep
    {
        public int ForVersion { get; set; }
        public string Description { get; set; }

        public Func<IDBIO, int> DowngradeFunction { get; set; }
        public Func<IDBIO, int> UpgradeFunction { get; set; }

        public int Downgrade(IDBIO _db)
        {
            return DowngradeFunction(_db);
        }

        public int Upgrade(IDBIO _db)
        {
            return UpgradeFunction(_db);
        }
    }
}