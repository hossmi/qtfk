using System.Collections.Generic;

namespace QTFK.Models
{
    public class FilterCompilation
    {
        public FilterCompilation(string query, IEnumerable<KeyValuePair<string, object>> parameters)
        {
            Asserts.isFilled(query, $"Contructor parameter '{nameof(query)}' cannot be empty.");
            Asserts.isSomething(parameters, $"Contructor parameter '{nameof(parameters)}' cannot be null.");

            this.FilterSegment = query;
            this.Parameters = parameters;
        }

        public IEnumerable<KeyValuePair<string, object>> Parameters { get; }
        public string FilterSegment { get; }
    }
}