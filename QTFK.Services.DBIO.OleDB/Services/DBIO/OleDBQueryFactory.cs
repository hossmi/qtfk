using System;
using QTFK.Models;
using QTFK.Models.DBIO;

namespace QTFK.Services.DBIO
{
    public class OleDBQueryFactory : IQueryFactory, IOleDB
    {
        public OleDBQueryFactory(IDBIO db)
        {
            DB = db;
        }

        public IDBIO DB { get; }

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
