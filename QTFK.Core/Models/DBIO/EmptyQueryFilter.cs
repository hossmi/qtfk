using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTFK.Models.DBIO
{
    public class EmptyQueryFilter : IQueryFilter
    {
        private static EmptyQueryFilter _instance;

        public static IQueryFilter Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new EmptyQueryFilter();
                return _instance;
            }
        }

        protected EmptyQueryFilter()
        {

        }

        public string Compile()
        {
            return string.Empty;
        }


    }
}
