using System;
using System.IO;

namespace QTFK
{
    public static class Asserts
    {
        public static void IsFilled(string someString, string exceptionMessageFormat, params object[] messageParams)
        {
            assertIsFilled(someString, exceptionMessageFormat, messageParams);
        }

        public static void IsInstance<T>(T item, string exceptionMessageFormat, params object[] messageParams) where T : class
        {
            assert(item != null, exceptionMessageFormat, messageParams);
        }

        public static void IsValidEnum<T>(T enumValue, string exceptionMessageFormat, params object[] messageParams) where T : struct
        {
            assert(Enum.IsDefined(typeof(T), enumValue), exceptionMessageFormat, messageParams);
        }

        public static void IsValidEnum<T>(string enumValue, string exceptionMessageFormat, params object[] messageParams) where T : struct
        {
            assertIsFilled(enumValue, exceptionMessageFormat, messageParams);
            assert(Enum.IsDefined(typeof(T), enumValue), exceptionMessageFormat, messageParams);
        }

        public static void IsGreaterThanZero(int value, string exceptionMessageFormat, params object[] messageParams)
        {
            assert(value > 0, exceptionMessageFormat, messageParams);
        }

        public static void IsGreaterThanZero(long value, string exceptionMessageFormat, params object[] messageParams)
        {
            assert(value > 0, exceptionMessageFormat, messageParams);
        }

        public static void IsGreaterThanZero(double value, string exceptionMessageFormat, params object[] messageParams)
        {
            assert(value > 0.0, exceptionMessageFormat, messageParams);
        }

        public static void IsGreaterThanZero(decimal value, string exceptionMessageFormat, params object[] messageParams)
        {
            assert(value > 0m, exceptionMessageFormat, messageParams);
        }

        public static void FileExists(string filePath, string exceptionMessageFormat, params object[] messageParams)
        {
            assertIsFilled(filePath, exceptionMessageFormat, messageParams);
            assert(File.Exists(filePath), exceptionMessageFormat, messageParams);
        }

        public static void IsTrue(bool condition, string exceptionMessageFormat, params object[] messageParams)
        {
            assert(condition, exceptionMessageFormat, messageParams);
        }

        private static void assertIsFilled(string someString, string exceptionMessageFormat, params object[] messageParams)
        {
            assert(string.IsNullOrWhiteSpace(someString) == false, exceptionMessageFormat, messageParams);
        }

        private static void assert(bool condition, string exceptionMessageFormat, params object[] messageParams)
        {
            if (condition == false)
                throw new ArgumentException(string.Format(exceptionMessageFormat ?? "", messageParams));
        }

    }
}