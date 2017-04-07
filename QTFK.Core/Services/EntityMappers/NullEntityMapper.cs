using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QTFK.Models;

namespace QTFK.Services.EntityMappers
{
    public class NullEntityMapper : IEntityMapper
    {
        protected static NullEntityMapper _instance;

        protected NullEntityMapper() { }

        public static IEntityMapper Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new NullEntityMapper();
                return _instance;
            }
        }

        public TTarget Map<TSource, TTarget>(TSource row)
        {
            return default(TTarget);
        }

        public void Register<TSource, TTarget>(Func<TSource, TTarget> mapper)
        {
        }
    }
}
