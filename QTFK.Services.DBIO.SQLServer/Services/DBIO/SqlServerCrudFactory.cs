﻿using System;
using QTFK.Models;
using QTFK.Models.DBIO;

namespace QTFK.Services.DBIO
{
    public class SqlServerCrudFactory : ICRUDQueryFactory, ISQLServer
    {
        public SqlServerCrudFactory(IDBIO db)
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