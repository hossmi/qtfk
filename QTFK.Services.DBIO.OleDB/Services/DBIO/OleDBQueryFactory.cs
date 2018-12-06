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
            }, new TSQLParameterBuilderFactory());
        }

        public OleDBQueryFactory(IEnumerable<Type> filterTypes)
            : base(filterTypes, new TSQLParameterBuilderFactory())
        {
        }

        public OleDBQueryFactory(IEnumerable<Type> filterTypes, IParameterBuilderFactory parameterBuilderFactory)
            : base(filterTypes, parameterBuilderFactory)
        {
        }

        protected override IDBQueryDelete prv_newDelete()
        {
            return new OleDBDeleteQuery(this.parameterBuilderFactory);
        }

        protected override IDBQueryInsert prv_newInsert()
        {
            return new OleDBInsertQuery(this.parameterBuilderFactory);
        }

        protected override IDBQuerySelect prv_newSelect()
        {
            return new OleDBSelectQuery(this.parameterBuilderFactory);
        }

        protected override IDBQueryUpdate prv_newUpdate()
        {
            return new OleDBUpdateQuery(this.parameterBuilderFactory);
        }



    }
}
