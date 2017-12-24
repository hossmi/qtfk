using System.Collections.Generic;

namespace QTFK.Models
{
    public interface IQueryFilter
    {
        IDictionary<string, object> Parameters { get; }
        string Compile();
    }
}