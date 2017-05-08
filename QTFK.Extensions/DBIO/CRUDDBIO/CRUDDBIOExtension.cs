using QTFK.Models;
using QTFK.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTFK.Extensions.DBIO.CRUDDBIO
{
    public static class CRUDDBIOExtension
    {
        public static IEnumerable<T> Select<T>(this ICRUDDBIO db, Action<IDBQuerySelect> queryBuild) where T : new()
        {
            var query = db.NewSelect();
            queryBuild(query);
            return db.Get<T>(query);
        }

        public static int Insert(this ICRUDDBIO db, Action<IDBQueryInsert> queryBuild)
        {
            var query = db.NewInsert();
            queryBuild(query);
            return db.Set(query);
        }

        public static int Update(this ICRUDDBIO db, Action<IDBQueryUpdate> queryBuild)
        {
            var query = db.NewUpdate();
            queryBuild(query);
            return db.Set(query);
        }

        public static int Delete(this ICRUDDBIO db, Action<IDBQueryDelete> queryBuild)
        {
            var query = db.NewDelete();
            queryBuild(query);
            return db.Set(query);
        }
    }
}
