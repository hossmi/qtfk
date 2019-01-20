using QTFK.Services;
using System.Collections.Generic;

namespace QTFK.Models
{
    public interface IQueryFilter
    {
        FilterCompilation Compile(IParameterBuilder parameterBuilder);
        //IEnumerable<QueryParameter> getParameters();
    }
}