using System;
using System.Linq.Expressions;

namespace QTFK.Models
{
    public interface IExpressionQueryFilter : IQueryFilter
    {
        Expression<Func<bool>> Condition { get; set; }
    }
}