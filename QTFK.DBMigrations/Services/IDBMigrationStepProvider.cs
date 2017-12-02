using System.Collections.Generic;

namespace QTFK.Services
{
    public interface IDBMigrationStepProvider
    {
        IEnumerable<IDBMigrationStep> GetSteps();
        string TablePrefix { get; }
    }
}