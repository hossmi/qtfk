using QTFK.Models;
using QTFK.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTFK.Extensions.DBIO.CRUDFactory
{
    public static class CRUDFactoryExtension
    {
        public static IEnumerable<T> Select<T>(this ICRUDQueryFactory factory, Action<IDBQuerySelect> queryBuild) where T : new()
        {
            var query = factory.NewSelect();
            queryBuild(query);
            return factory.DB.Get<T>(query);
        }

        public static int Insert(this ICRUDQueryFactory factory, Action<IDBQueryInsert> queryBuild)
        {
            var query = factory.NewInsert();
            queryBuild(query);
            return factory.DB.Set(query);
        }

        public static int Update(this ICRUDQueryFactory factory, Action<IDBQueryUpdate> queryBuild)
        {
            var query = factory.NewUpdate();
            queryBuild(query);
            return factory.DB.Set(query);
        }

        public static int Delete(this ICRUDQueryFactory factory, Action<IDBQueryDelete> queryBuild)
        {
            var query = factory.NewDelete();
            queryBuild(query);
            return factory.DB.Set(query);
        }
    }
}
