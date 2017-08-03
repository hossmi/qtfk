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
        public string Prefix { get; set; }

        public IDBQueryDelete NewDelete()
        {
            return new SqlDeleteQuery() { Prefix = this.Prefix };
        }

        public IDBQueryInsert NewInsert()
        {
            return new SqlInsertQuery() { Prefix = this.Prefix };
        }

        public IDBQuerySelect NewSelect()
        {
            return new SqlSelectQuery() { Prefix = this.Prefix };
        }

        public IDBQueryUpdate NewUpdate()
        {
            return new SqlUpdateQuery() { Prefix = this.Prefix };
        }
    }
}