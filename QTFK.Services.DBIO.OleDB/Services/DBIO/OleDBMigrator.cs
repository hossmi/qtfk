using System;
using System.Collections.Generic;
using System.Linq;
using QTFK.Models;
using QTFK.Extensions.DBIO;
using QTFK.Extensions.Mapping.AutoMapping;
using QTFK.Extensions.DBCommand;
using QTFK.Extensions.DataReader;
using QTFK.Extensions.DBIO.OleDBIOExtensions;
using QTFK.Extensions.Mapping;
using System.Data;

namespace QTFK.Services.DBIO
{
    public class OleDBMigrator : IDBMigrator
    {
        public const string kVersionTableName = "__version";

        private readonly IDBIO _db;
        private readonly string _tableName;
        private readonly IDictionary<int, IDBMigrationStep> _migrationSteps;

        public OleDBMigrator(IDBIO db, IDBMigrationStepProvider migratorProvider)
        {
            _db = db;
            _migrationSteps = (migratorProvider.GetSteps() ?? Enumerable.Empty<IDBMigrationStep>())
                .ToDictionary(m => m.ForVersion);
            _tableName = $"{migratorProvider.TablePrefix ?? ""}{kVersionTableName}";
        }

        string _SQL_Create_Table_Version
        {
            get
            {
                return $@"
                    CREATE TABLE [{_tableName}] (
	                    id long IDENTITY(1,1) NOT NULL
                        , [{nameof(MigrationInfo.MigrationDate)}] datetime NULL
                        , milis long NULL
                        , [{nameof(MigrationInfo.Version)}] int NULL
                        , [{nameof(MigrationInfo.Description)}] varchar(255) NULL
                        , CONSTRAINT [PK_{_tableName}] PRIMARY KEY (id)
                    );";
            }
        }

        string _SQL_INSERT_INTO_Version
        {
            get
            {
                return $@"
                    INSERT INTO [{_tableName}] (
                          [{nameof(MigrationInfo.MigrationDate)}]
                        , milis
                        , [{nameof(MigrationInfo.Version)}]
                        , [{nameof(MigrationInfo.Description)}]
                    )
                    VALUES (
                          @migrationDate
                        , @secs
                        , @version
                        , @description
                    );";
            }
        }

        string _SQL_SELECT_FROM_Version
        {
            get
            {
                return $"SELECT * FROM [{_tableName}] ";
            }
        }

        private static IDictionary<string, object> Map(MigrationInfo data)
        {
            return DBIOExtension.Params()
                .Set("@migrationDate", data.MigrationDate.ToString())
                .Set("@secs", data.MigrationDate.Millisecond.ToString())
                .Set("@version", data.Version)
                .Set("@description", data.Description)
                ;
        }

        private static MigrationInfo Map(IDataRecord record)
        {
            var item = record.AutoMap<MigrationInfo>();
            item.MigrationDate += new TimeSpan(0, 0, 0, 0, record.Get<int>("milis"));
            return item;
        }

        public IEnumerable<MigrationInfo> Upgrade()
        {
            var version = GetVersion();

            if (!version.HasValue)
            {
                _db.Set(_SQL_Create_Table_Version);
                version = GetVersion();
            }

            if (version == 0)
                _db.Set(_SQL_INSERT_INTO_Version, Map(new MigrationInfo()
                {
                    Description = $"{nameof(OleDBMigrator)} Initialization.",
                    MigrationDate = DateTime.Now,
                    Version = 0
                }));

            return _Upgrade().ToList();
        }

        public IEnumerable<MigrationInfo> GetMigrations()
        {
            var result = new Result<IEnumerable<MigrationInfo>>(() => _db
                .Get(_SQL_SELECT_FROM_Version, Map)
                .DefaultIfEmpty(new MigrationInfo())
                );

            if (!result.Ok)
                return new MigrationInfo[] { new MigrationInfo() };

            return result.Value;
        }

        private int? GetVersion()
        {
            var result = new Result<int?>(() => _db
                    .Get<int?>($@"
SELECT TOP 1 [{nameof(MigrationInfo.Version)}]
FROM [{_tableName}]
ORDER BY id DESC",
                        r => r.Get<int?>("version"))
                    .FirstOrDefault()
                );

            if (!result.Ok)
                return null;

            if (!result.Value.HasValue)
                return 0;

            return result.Value.Value;
        }

        private IEnumerable<MigrationInfo> _Upgrade()
        {
            while (true)
            {
                int currentVersion = GetVersion().Value;

                if (!_migrationSteps.ContainsKey(currentVersion))
                    break;

                var step = _migrationSteps[currentVersion];

                var result = new Result<int>(() => step.Upgrade(_db));
                if (result.Ok)
                {
                    var info = new MigrationInfo
                    {
                        Version = result.Value,
                        Description = step.Description,
                        MigrationDate = DateTime.Now
                    };

                    _db.Set(_SQL_INSERT_INTO_Version, Map(info));
                    yield return info;
                    continue;
                }

                new Result(() => step.Downgrade(_db));
                break;
            }
        }

    }
}
