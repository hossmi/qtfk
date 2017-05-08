using QTFK.Models;
using QTFK.Models.DBIO;

namespace QTFK.Services.DBIO
{
    public class OleDBCrudDBIO : OleDBIO, ICRUDDBIO
    {
        public OleDBCrudDBIO(string connectionString, ILogger<LogLevel> log = null) : base(connectionString, log)
        {
        }

        public IDBQueryDelete NewDelete()
        {
            return new OleDBDeleteQuery();
        }

        public IDBQueryInsert NewInsert()
        {
            return new OleDBInsertQuery();
        }

        public IDBQuerySelect NewSelect()
        {
            return new OleDBSelectQuery();
        }

        public IDBQueryUpdate NewUpdate()
        {
            return new OleDBUpdateQuery();
        }
    }
}
