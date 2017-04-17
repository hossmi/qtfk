using QTFK.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTFK.Services.DBIO
{
    public class DefaultDBMigrationStepProvider : DBMigrationStepProviderBase
    {
        private readonly IEnumerable<IDBMigrationStep> _steps;

        public DefaultDBMigrationStepProvider(
            IEnumerable<IDBMigrationStep> steps
            ) : this("", steps)
        {
        }

        public DefaultDBMigrationStepProvider(
            string tablePrefix
            , IEnumerable<IDBMigrationStep> steps
            ) : base(tablePrefix)
        {
            _steps = steps;
        }

        public override IEnumerable<IDBMigrationStep> GetSteps()
        {
            return _steps;
        }
    }
}
