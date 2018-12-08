using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTFK.Extensions.Exceptions
{
    public static class ExceptionExtension
    {
        public static T wrap<T>(this Exception exception, Func<Exception, T> wrapperDelegate) where T : Exception
        {
            T wrappedException;

            Asserts.isNotNull(wrapperDelegate);

            wrappedException = wrapperDelegate(exception);
            Asserts.isNotNull(wrappedException);

            return wrappedException;
        }
    }
}
