using QTFK.Services;
using System.Collections.Generic;

namespace QTFK.Models
{
    public interface IQueryFilter
    {
        string Compile();
        IDictionary<string, object> getParameters();
        void setParameterBuilder(IParameterBuilder parameterBuilder);
    }
}