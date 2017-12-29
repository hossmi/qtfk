using System;
using QTFK.Models;
using QTFK.Models.DBIO;
using System.Collections.Generic;
using QTFK.Attributes;

namespace QTFK.Services.DBIO
{
    [SqlServer]
    public class SQLServerQueryFactory : AbstractQueryFactory
    {
        public static SQLServerQueryFactory buildDefault()
        {
            return new SQLServerQueryFactory(new Type[]
            {
                //typeof(SqlByParamEqualsFilter),
            }, ParameterBuilder.TSQL);
        }

        public SQLServerQueryFactory(IEnumerable<Type> filterTypes, IParameterBuilder parameterBuilder) 
            : base(filterTypes, parameterBuilder)
        {
        }

        protected override IDBQueryDelete prv_newDelete()
        {
            return new SqlDeleteQuery();
        }

        protected override IDBQueryInsert prv_newInsert()
        {
            return new SqlInsertQuery(this.parameterBuilder);
        }

        protected override IDBQuerySelect prv_newSelect()
        {
            return new SqlSelectQuery();
        }

        protected override IDBQueryUpdate prv_newUpdate()
        {
            return new SqlUpdateQuery(this.parameterBuilder);
        }
    }
}