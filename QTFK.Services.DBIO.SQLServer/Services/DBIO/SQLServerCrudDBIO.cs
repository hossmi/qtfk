using QTFK.Models;
using QTFK.Models.DBIO;

namespace QTFK.Services.DBIO
{
    public class SQLServerCrudDBIO : SQLServerDBIO, ICRUDDBIO
    {
        public SQLServerCrudDBIO(string connectionString, ILogger<LogLevel> log = null) : base(connectionString, log)
        {
        }

        public IDBQueryDelete NewDelete()
        {
            return new SqlDeleteQuery();
        }

        public IDBQueryInsert NewInsert()
        {
            return new SqlInsertQuery();
        }

        public IDBQuerySelect NewSelect()
        {
            return new SqlSelectQuery();
        }

        public IDBQueryUpdate NewUpdate()
        {
            return new SqlUpdateQuery();
        }
    }
}
