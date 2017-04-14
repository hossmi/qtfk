using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QTFK.Models;

namespace QTFK.Services.DBIO
{
    public class OleDBMigrator : IDBMigrator
    {
        private readonly IDBIO _db;
        private readonly IMigrationStepProvider _stepProvider;

        public OleDBMigrator(IDBIO db, IMigrationStepProvider stepProvider)
        {
            _db = db;
            _stepProvider = stepProvider;
        }

        public IQueryable<MigrationInfo> GetMigrations()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<MigrationInfo> Upgrade()
        {
            throw new NotImplementedException();
        }
    }
}
