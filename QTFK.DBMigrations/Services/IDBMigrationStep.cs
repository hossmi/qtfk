using QTFK.Models;

namespace QTFK.Services
{
    public interface IDBMigrationStep
    {
        int ForVersion { get; }
        string Description { get; }

        int Upgrade(IDBIO db);
        void Downgrade(IDBIO db);
    }
}