using System.Collections.Generic;

namespace QTFK.Models
{
    public class QueryCompilation
    {
        public QueryCompilation(string query, IEnumerable<KeyValuePair<string, object>> parameters)
        {
            Asserts.stringIsNotEmpty(query);
            Asserts.isNotNull(parameters);

            this.Query = query;
            this.Parameters = parameters;
        }

        public IEnumerable<KeyValuePair<string, object>> Parameters { get; }
        public string Query { get; }
    }
}