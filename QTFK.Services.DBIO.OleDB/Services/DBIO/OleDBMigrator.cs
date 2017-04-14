using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QTFK.Models;
using QTFK.Extensions.DBIO;
using QTFK.Extensions.DBIOMigrationExtensions;
using QTFK.Extensions.Mapping.AutoMapping;

namespace QTFK.Services.DBIO
{
    public class OleDBMigrator : IDBMigrator
    {
        private readonly IDBIO _db;
        private readonly IEnumerable<IMigrationStep> _migrationSteps;

        public OleDBMigrator(IDBIO db, IEnumerable<IMigrationStep> migrationSteps)
        {
            _db = db;
            _migrationSteps = migrationSteps;
        }

        public IQueryable<MigrationInfo> GetMigrations()
        {
            var result = new Result<IEnumerable<MigrationInfo>>(() =>
                _db.Get("SELECT * FROM [__version] ", r => r.AutoMap<MigrationInfo>()));

            if (result.Ok)
                return result.Value.AsQueryable();
            else
                return new MigrationInfo[] { new MigrationInfo() }.AsQueryable();
        }

        public IEnumerable<MigrationInfo> Upgrade()
        {
            return _Upgrade().ToList();
        }

        private IEnumerable<MigrationInfo> _Upgrade()
        {
            while (true)
            {
                var currentVersion = this.GetCurrentVersion();

                var step = _migrationSteps.SingleOrDefault(m => m.ForVersion == currentVersion);

                if (step != null)
                    break;

                var result = new Result<int>(() => step.Upgrade(_db));
                if (result.Ok)
                {
                    var info = new MigrationInfo
                    {
                        Version = result.Value,
                        Description = step.Description,
                        MigrationDate = DateTime.Now
                    };
                    //add migration to data base
                    yield return info;
                    continue;
                }

                step.Downgrade(_db);
                break;
            }
        }
    }
}
