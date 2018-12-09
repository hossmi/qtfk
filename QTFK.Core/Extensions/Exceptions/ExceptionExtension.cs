using System;

namespace QTFK.Extensions.Exceptions
{
    public static class ExceptionExtension
    {
        //public static T wrap<T>(this Exception exception, Func<Exception, T> wrapperDelegate) where T : Exception
        //{
        //    T wrappedException;

        //    Asserts.isNotNull(wrapperDelegate);

        //    wrappedException = wrapperDelegate(exception);
        //    Asserts.isNotNull(wrappedException);

        //    return wrappedException;
        //}

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
