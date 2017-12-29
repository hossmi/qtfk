using System;
using System.Linq.Expressions;

namespace QTFK.Models.QueryFilters
{
    public interface IExpressionQueryFilter : IQueryFilter
    {
        Expression<Func<bool>> Condition { get; set; }
    }
}