using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace QTFK
{
    public static class Asserts
    {
        public static void isValidEnum<T>(object enumValue,
            [CallerLineNumber] int sourceLineNumber = 0,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "") where T : struct
        {
            prv_assert(Enum.IsDefined(typeof(T), enumValue), memberName, sourceFilePath, sourceLineNumber);
        }

        public static void fileExists(string filePath,
            [CallerLineNumber] int sourceLineNumber = 0,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "")
        {
            prv_stringIsNotEmpty(filePath, memberName, sourceFilePath, sourceLineNumber);
            prv_assert(File.Exists(filePath), memberName, sourceFilePath, sourceLineNumber);
        }

        public static void stringIsNotEmpty(string someString,
            [CallerLineNumber] int sourceLineNumber = 0,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "")
        {
            prv_stringIsNotEmpty(someString, memberName, sourceFilePath, sourceLineNumber);
        }

        public static void isTrue(bool condition,
            [CallerLineNumber] int sourceLineNumber = 0,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "")
        {
            prv_assert(condition, memberName, sourceFilePath, sourceLineNumber);
        }

        public static void isFalse(bool condition,
            [CallerLineNumber] int sourceLineNumber = 0,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "")
        {
            prv_assert(condition == false, memberName, sourceFilePath, sourceLineNumber);
        }

        public static void fail(
            [CallerLineNumber] int sourceLineNumber = 0,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "")
        {
            throw new PrvAssertException(memberName, sourceFilePath, sourceLineNumber);
        }

        public static void isNotNull<T>(T item,
            [CallerLineNumber] int sourceLineNumber = 0,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "") where T : class
        {
            prv_assert(item != null, memberName, sourceFilePath, sourceLineNumber);
        }

        public static void isNull<T>(T item,
            [CallerLineNumber] int sourceLineNumber = 0,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "") where T : class
        {
            prv_assert(item == null, memberName, sourceFilePath, sourceLineNumber);
        }

        public static void isInstanceOf<T>(object item,
            [CallerLineNumber] int sourceLineNumber = 0,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "")
        {
            prv_assert(item is T, memberName, sourceFilePath, sourceLineNumber);
        }

        private static void prv_stringIsNotEmpty(string someString, string memberName, string sourceFilePath, int sourceLineNumber)
        {
            prv_assert(string.IsNullOrWhiteSpace(someString) == false, memberName, sourceFilePath, sourceLineNumber);
        }

        private static void prv_assert(bool condition, string memberName, string sourceFilePath, int sourceLineNumber)
        {
            if (condition == false)
                throw new PrvAssertException(memberName, sourceFilePath, sourceLineNumber);
        }

        private class PrvAssertException : Exception
        {
            public PrvAssertException(string memberName, string sourceFilePath, int sourceLineNumber)
            {
                this.Message = string.Format(@"False assertion at:
  member: {0}
  file: {1}
  line: {2}", memberName, sourceFilePath, sourceLineNumber);
            }

            public override string Message { get; }
        }
    }
}