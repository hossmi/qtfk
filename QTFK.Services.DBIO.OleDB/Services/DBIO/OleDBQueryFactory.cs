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
        public string Prefix { get; set; }

        public IDBQueryDelete NewDelete()
        {
            return new OleDBDeleteQuery() { Prefix = this.Prefix };
        }

        public IDBQueryInsert NewInsert()
        {
            return new OleDBInsertQuery() { Prefix = this.Prefix };
        }

        public IDBQuerySelect NewSelect()
        {
            return new OleDBSelectQuery() { Prefix = this.Prefix };
        }

        public IDBQueryUpdate NewUpdate()
        {
            return new OleDBUpdateQuery() { Prefix = this.Prefix };
        }
    }
}
