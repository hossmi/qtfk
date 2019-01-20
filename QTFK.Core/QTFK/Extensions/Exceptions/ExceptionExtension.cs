using System;

namespace QTFK.Extensions.Exceptions
{
    public static class ExceptionExtension
    {
        public static TExOut wrap<TExOut, TExIn>(this TExIn exception, Func<TExIn, TExOut> wrapperDelegate) 
            where TExIn : Exception
            where TExOut : Exception
        {
            TExOut wrappedException;

            Asserts.isNotNull(wrapperDelegate);

            wrappedException = wrapperDelegate(exception);
            Asserts.isNotNull(wrappedException);

            return wrappedException;
        }
    }
}
