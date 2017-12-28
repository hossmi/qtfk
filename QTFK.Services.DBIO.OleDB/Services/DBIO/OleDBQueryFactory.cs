using System;
using QTFK.Models;
using QTFK.Models.DBIO;
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
                //typeof(OleDBByParamEqualsFilter),
            }, new TransactSQLParameterBuilder());
        }

        public OleDBQueryFactory(IEnumerable<Type> filterTypes, IParameterBuilder parameterBuilder) 
            : base(filterTypes, parameterBuilder)
        {
        }

        protected override IDBQueryDelete prv_newDelete()
        {
            return new OleDBDeleteQuery();
        }

        protected override IDBQueryInsert prv_newInsert()
        {
            return new OleDBInsertQuery(this.parameterBuilder);
        }

        protected override IDBQuerySelect prv_newSelect()
        {
            return new OleDBSelectQuery();
        }

        protected override IDBQueryUpdate prv_newUpdate()
        {
            return new OleDBUpdateQuery(this.parameterBuilder);
        }



    }
}
