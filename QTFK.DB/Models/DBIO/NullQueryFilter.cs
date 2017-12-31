﻿using System;
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

        public string Compile()
        {
            return string.Empty;
        }

        public IDictionary<string, object> getParameters()
        {
            return new Dictionary<string, object>();
        }
    }
}
