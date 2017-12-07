using System;
using QTFK.Models;
using QTFK.Models.DBIO;
using QTFK.Models.DBIO.Filters;
using System.Collections.Generic;
using System.Linq;

namespace QTFK.Services.DBIO
{
    public class SQLServerQueryFactory : AbstractQueryFactory, ISQLServer
    {
        public static SQLServerQueryFactory buildDefault()
        {
            return new SQLServerQueryFactory(new Type[]
            {
                typeof(SqlByParamEqualsFilter),
            });
        }

        public SQLServerQueryFactory(IEnumerable<Type> filterTypes) 
            : base(filterTypes)
        {
        }

        protected override IDBQueryDelete prv_newDelete()
        {
            return new SqlDeleteQuery();
        }

        protected override IDBQueryInsert prv_newInsert()
        {
            return new SqlInsertQuery();
        }

        protected override IDBQuerySelect prv_newSelect()
        {
            return new SqlSelectQuery();
        }

        protected override IDBQueryUpdate prv_newUpdate()
        {
            return new SqlUpdateQuery();
        }
    }
}