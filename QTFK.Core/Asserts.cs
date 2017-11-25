using System;
using System.IO;

namespace QTFK
{
    public static class Asserts
    {
        public static void isFilled(string someString, string exceptionMessageFormat, params object[] messageParams)
        {
            prv_assertIsFilled(someString, exceptionMessageFormat, messageParams);
        }

        public static void isSomething(object item, string exceptionMessageFormat, params object[] messageParams)
        {
            prv_assert(item != null, exceptionMessageFormat, messageParams);
        }

        public static void isValidEnum<T>(object enumValue, string exceptionMessageFormat, params object[] messageParams) where T : struct
        {
            prv_assert(Enum.IsDefined(typeof(T), enumValue), exceptionMessageFormat, messageParams);
        }

        public static void fileExists(string filePath, string exceptionMessageFormat, params object[] messageParams)
        {
            prv_assertIsFilled(filePath, exceptionMessageFormat, messageParams);
            prv_assert(File.Exists(filePath), exceptionMessageFormat, messageParams);
        }

        public static void check(bool condition, string exceptionMessageFormat, params object[] messageParams)
        {
            prv_assert(condition, exceptionMessageFormat, messageParams);
        }

        private static void prv_assertIsFilled(string someString, string exceptionMessageFormat, params object[] messageParams)
        {
            prv_assert(string.IsNullOrWhiteSpace(someString) == false, exceptionMessageFormat, messageParams);
        }

        private static void prv_assert(bool condition, string exceptionMessageFormat, params object[] messageParams)
        {
            if (condition == false)
                throw new ArgumentException(string.Format(exceptionMessageFormat ?? "", messageParams));
        }

    }
}