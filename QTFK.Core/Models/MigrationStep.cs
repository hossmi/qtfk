using System;
using QTFK.Models;
using QTFK.Services;

namespace QTFK.Models
{
    public class MigrationStep : IMigrationStep
    {
        public int ForVersion { get; set; }
        public string Description { get; set; }

        public Action<IDBIO> Downgrade { get; set; }
        public Func<IDBIO, int> Upgrade { get; set; }

        int IMigrationStep.Upgrade(IDBIO db)
        {
            return Upgrade(db);
        }

        void IMigrationStep.Downgrade(IDBIO db)
        {
            Downgrade?.Invoke(db);
        }
    }
}