using System;
using QTFK.Models;
using QTFK.Models.DBIO;

namespace QTFK.Services.DBIO
{
    public class SQLServerQueryFactory : IQueryFactory, ISQLServer
    {
        public SQLServerQueryFactory(IDBIO db)
        {
            DB = db;
        }

        public IDBIO DB { get; }

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