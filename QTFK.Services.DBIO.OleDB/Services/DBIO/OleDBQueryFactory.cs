using System;
using QTFK.Models;
using QTFK.Models.DBIO;
using QTFK.Models.DBIO.Filters;
using System.Collections.Generic;
using System.Linq;
using QTFK.Attributes;

namespace QTFK.Services.DBIO
{
    [OleDB]
    public class OleDBQueryFactory : AbstractQueryFactory 
    {
        public static OleDBQueryFactory buildDefault()
        {
            return new OleDBQueryFactory(new Type[]
            {
                typeof(OleDBByParamEqualsFilter),
            });
        }

        public OleDBQueryFactory(IEnumerable<Type> filterTypes) 
            : base(filterTypes)
        {
        }

        protected override IDBQueryDelete prv_newDelete()
        {
            return new OleDBDeleteQuery();
        }

        protected override IDBQueryInsert prv_newInsert()
        {
            return new OleDBInsertQuery();
        }

        protected override IDBQuerySelect prv_newSelect()
        {
            return new OleDBSelectQuery();
        }

        protected override IDBQueryUpdate prv_newUpdate()
        {
            return new OleDBUpdateQuery();
        }
    }
}
