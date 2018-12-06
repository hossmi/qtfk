using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QTFK.Services;

namespace QTFK.Models.DBIO
{
    public class NullQueryFilter : IQueryFilter
    {
        private static NullQueryFilter _instance;

        public static IQueryFilter Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new NullQueryFilter();
                return _instance;
            }
        }

        protected NullQueryFilter()
        {
        }

        public FilterCompilation Compile(IParameterBuilder parameterBuilder)
        {
            return new FilterCompilation(
                string.Empty, 
                Enumerable.Empty<KeyValuePair<string,object>>());
        }
    }
}
