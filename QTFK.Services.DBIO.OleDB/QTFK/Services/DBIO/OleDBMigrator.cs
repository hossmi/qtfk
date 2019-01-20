using System;
using System.Collections.Generic;
using System.Linq;
using QTFK.Models;
using QTFK.Extensions.DBIO;
using QTFK.Extensions.Mapping.AutoMapping;
using QTFK.Extensions.DataReader;
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
            this._db = db;
            this._migrationSteps = (migratorProvider.GetSteps() ?? Enumerable.Empty<IDBMigrationStep>())
                .ToDictionary(m => m.ForVersion);
            this._tableName = $"{migratorProvider.TablePrefix ?? ""}{kVersionTableName}";
        }

        string _SQL_Create_Table_Version
        {
            get
            {
                return $@"
                    CREATE TABLE [{this._tableName}] (
	                    id long IDENTITY(1,1) NOT NULL
                        , [{nameof(MigrationInfo.MigrationDate)}] datetime NULL
                        , milis long NULL
                        , [{nameof(MigrationInfo.Version)}] int NULL
                        , [{nameof(MigrationInfo.Description)}] varchar(255) NULL
                        , CONSTRAINT [PK_{this._tableName}] PRIMARY KEY (id)
                    );";
            }
        }

        string _SQL_INSERT_INTO_Version
        {
            get
            {
                return $@"
                    INSERT INTO [{this._tableName}] (
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
                return $"SELECT * FROM [{this._tableName}] ";
            }
        }

        private static IDictionary<string, object> Map(MigrationInfo data)
        {
            return new Dictionary<string, object>
            {
                { "@migrationDate", data.MigrationDate.ToString() },
                { "@secs", data.MigrationDate.Millisecond.ToString() },
                { "@version", data.Version },
                { "@description", data.Description },
            };
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
                this._db.Set(_SQL_Create_Table_Version);
                version = GetVersion();
            }

            if (version == 0)
                this._db.Set(_SQL_INSERT_INTO_Version, Map(new MigrationInfo()
                {
                    Description = $"{nameof(OleDBMigrator)} Initialization.",
                    MigrationDate = DateTime.Now,
                    Version = 0
                }));

            return _Upgrade().ToList();
        }

        public IEnumerable<MigrationInfo> GetMigrations()
        {
            var result = new Result<IEnumerable<MigrationInfo>>(() => this._db
                .Get(_SQL_SELECT_FROM_Version, Map)
                .DefaultIfEmpty(new MigrationInfo())
                );

            if (!result.Ok)
                return new MigrationInfo[] { new MigrationInfo() };

            return result.Value;
        }

        private int? GetVersion()
        {
            var result = new Result<int?>(() => this._db
                    .Get<int?>($@"
SELECT TOP 1 [{nameof(MigrationInfo.Version)}]
FROM [{this._tableName}]
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

                if (!this._migrationSteps.ContainsKey(currentVersion))
                    break;

                var step = this._migrationSteps[currentVersion];

                var result = new Result<int>(() => step.Upgrade(this._db));
                if (result.Ok)
                {
                    var info = new MigrationInfo
                    {
                        Version = result.Value,
                        Description = step.Description,
                        MigrationDate = DateTime.Now
                    };

                    this._db.Set(_SQL_INSERT_INTO_Version, Map(info));
                    yield return info;
                    continue;
                }

                new Result(() => step.Downgrade(this._db));
                break;
            }
        }

        public void UnInstall()
        {
            this._db.Set($@"DROP TABLE [{this._tableName}]");
        }
    }
}
