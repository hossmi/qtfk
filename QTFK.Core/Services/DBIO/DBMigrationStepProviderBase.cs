using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QTFK.Services;

namespace QTFK.Services.DBIO
{
    public abstract class DBMigrationStepProviderBase : IDBMigrationStepProvider
    {
        public DBMigrationStepProviderBase() : this("") { }

        public DBMigrationStepProviderBase(string tablePrefix)
        {
            TablePrefix = tablePrefix;
        }

        public string TablePrefix { get; }

        public abstract IEnumerable<IDBMigrationStep> GetSteps();
    }
}
