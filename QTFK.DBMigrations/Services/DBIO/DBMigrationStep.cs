using System;

namespace QTFK.Services.DBIO
{
    public class DBMigrationStep : IDBMigrationStep
    {
        public int ForVersion { get; set; }
        public string Description { get; set; }

        public Action<IDBIO> Downgrade { get; set; }
        public Func<IDBIO, int> Upgrade { get; set; }

        int IDBMigrationStep.Upgrade(IDBIO db)
        {
            return Upgrade(db);
        }

        void IDBMigrationStep.Downgrade(IDBIO db)
        {
            Downgrade?.Invoke(db);
        }
    }
}