using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTFK.Extensions.Collections.Casting
{
    public static class CastExtension
    {
        //TODO test
        public static T As<T>(this object source) where T : class
        {
            if (source is T)
                return source as T;
            return null;
        }
    }
}
