using System.Collections.Generic;

namespace QTFK.Models
{
    public class FilterCompilation
    {
        public FilterCompilation(string query, IEnumerable<KeyValuePair<string, object>> parameters)
        {
            Asserts.stringIsNotEmpty(query);
            Asserts.isNotNull(parameters);

            this.FilterSegment = query;
            this.Parameters = parameters;
        }

        public IEnumerable<KeyValuePair<string, object>> Parameters { get; }
        public string FilterSegment { get; }
    }
}