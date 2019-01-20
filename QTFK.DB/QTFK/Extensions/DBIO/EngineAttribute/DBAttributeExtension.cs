using QTFK.Attributes;
using QTFK.Models;
using QTFK.Services;
using System;
using System.Reflection;

namespace QTFK.Extensions.DBIO.EngineAttribute
{
    public static class DBAttributeExtension
    {
        public static string getDBEngine(this IDBIO db)
        {
            return prv_getDBEngine(db);
        }

        public static string getDBEngine(this IQueryFactory queryFactory)
        {
            return prv_getDBEngine(queryFactory);
        }

        public static string getDBEngine(this IDBQuery query)
        {
            return prv_getDBEngine(query);
        }

        private static string prv_getDBEngine(object item)
        {
            DBAttribute dbAttribute;
            string engine;
            Type itemType;

            Asserts.isNotNull(item);

            itemType = item.GetType();
            dbAttribute = itemType.GetCustomAttribute<DBAttribute>();

            Asserts.isNotNull(dbAttribute);
            engine = dbAttribute.Engine;

            return engine;
        }
    }
}
