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
            }, new TSQLParameterBuilderFactory());
        }

        public SQLServerQueryFactory(IEnumerable<Type> filterTypes)
            : this(filterTypes, new TSQLParameterBuilderFactory())
        {
        }

        public SQLServerQueryFactory(IEnumerable<Type> filterTypes, IParameterBuilderFactory parameterBuilderFactory) 
            : base(filterTypes, parameterBuilderFactory)
        {
        }

        protected override IDBQueryDelete prv_newDelete()
        {
            return new SqlDeleteQuery(this.parameterBuilderFactory);
        }

        protected override IDBQueryInsert prv_newInsert()
        {
            return new SqlInsertQuery(this.parameterBuilderFactory);
        }

        protected override IDBQuerySelect prv_newSelect()
        {
            return new SqlSelectQuery(this.parameterBuilderFactory);
        }

        protected override IDBQueryUpdate prv_newUpdate()
        {
            return new SqlUpdateQuery(this.parameterBuilderFactory);
        }
    }
}