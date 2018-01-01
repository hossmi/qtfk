using QTFK.Services;
using System.Collections.Generic;

namespace QTFK.Models
{
    public interface IQueryFilter
    {
        string Compile();
        IEnumerable<QueryParameter> getParameters();
    }
}