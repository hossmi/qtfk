using System.Collections.Generic;

namespace QTFK.Models
{
    public class QueryCompilation
    {
        public QueryCompilation(string query, IEnumerable<KeyValuePair<string, object>> parameters)
        {
            Asserts.isFilled(query, $"Contructor parameter '{nameof(query)}' cannot be empty.");
            Asserts.isSomething(parameters, $"Contructor parameter '{nameof(parameters)}' cannot be null.");

            this.Query = query;
            this.Parameters = parameters;
        }

        public IEnumerable<KeyValuePair<string, object>> Parameters { get; }
        public string Query { get; }
    }
}