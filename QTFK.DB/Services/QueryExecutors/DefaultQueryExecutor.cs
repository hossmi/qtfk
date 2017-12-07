using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QTFK.Models;
using QTFK.Extensions.DBIO;

namespace QTFK.Services.QueryExecutors
{
    public class DefaultQueryExecutor : IQueryExecutor
    {
        private readonly IDBIO db;
        private readonly IQueryFactory queryFactory;

        public DefaultQueryExecutor(IDBIO db, IQueryFactory queryFactory)
        {
            Asserts.isSomething(db, "Parameter 'db' cannot be null.");
            Asserts.isSomething(queryFactory, "Parameter 'queryFactory' cannot be null.");

            this.db = db;
            this.queryFactory = queryFactory;
        }

        public int delete(Action<IDBQueryDelete> queryBuild)
        {
            IDBQueryDelete query;
            int result;

            query = this.queryFactory.newDelete();
            queryBuild(query);
            result = this.db.Set(query);

            return result;
        }

        public int insert(Action<IDBQueryInsert> queryBuild)
        {
            IDBQueryInsert query;
            int result;

            query = this.queryFactory.newInsert();
            queryBuild(query);
            result = this.db.Set(query);

            return result;
        }

        public IEnumerable<T> select<T>(Action<IDBQuerySelect> queryBuild) where T : new()
        {
            IDBQuerySelect query;
            IEnumerable<T> items;

            Asserts.isSomething(queryBuild, "Parameter 'queryBuild' cannot be null.");

            query = this.queryFactory.newSelect();
            queryBuild(query);
            items = this.db.Get<T>(query);

            return items;
        }

        public int update(Action<IDBQueryUpdate> queryBuild)
        {
            IDBQueryUpdate query;
            int result;

            Asserts.isSomething(queryBuild, "Parameter 'queryBuild' cannot be null.");

            query = this.queryFactory.newUpdate();
            queryBuild(query);
            result = this.db.Set(query);

            return result;
        }
    }
}
