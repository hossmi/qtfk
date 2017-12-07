using QTFK.Models;
using QTFK.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTFK.Extensions.DBIO.QueryFactory
{
    public static class QueryFactoryExtension
    {
        public static T buildFilter<T>(this IQueryFactory queryFactory) where T : IQueryFilter
        {
            return (T)queryFactory.buildFilter(typeof(T));
        }
    }
}
