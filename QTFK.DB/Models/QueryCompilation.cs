using System.Collections.Generic;
using System.Linq;

namespace QTFK.Models
{
    public class QueryCompilation
    {
        public QueryCompilation(string query, IDictionary<string, object> parameters)
        {
            Asserts.isFilled(query, $"Contructor parameter '{nameof(query)}' cannot be empty.");
            Asserts.isSomething(parameters, $"Contructor parameter '{nameof(parameters)}' cannot be null.");

            this.Query = query;
            this.Parameters = parameters;
        }

        public IDictionary<string, object> Parameters { get; }
        public string Query { get; }
    }
}