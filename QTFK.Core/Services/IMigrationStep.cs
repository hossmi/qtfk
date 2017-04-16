using QTFK.Models;

namespace QTFK.Services
{
    public interface IMigrationStep
    {
        int ForVersion { get; }
        string Description { get; }

        int Upgrade(IDBIO db);
        void Downgrade(IDBIO db);
    }
}