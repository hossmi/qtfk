using System;
using System.Linq;
using System.Reflection;

namespace QTFK.Extensions.Assemblies
{
    public static class AssemblyExtensions
    {
        public static T CreateInstance<T>(this Assembly assembly, string implementationType) where T : class
        {
            return CreateInstance<T>(assembly, implementationType, true, true);
        }

        public static T CreateInstance<T>(this Assembly assembly, string implementationType, bool throwOnError) where T : class
        {
            return CreateInstance<T>(assembly, implementationType, throwOnError, true);
        }

        public static T CreateInstance<T>(this Assembly assembly, string implementationType, bool throwOnError, bool ignoreCase) where T : class
        {
            Type t = assembly.GetType(implementationType, throwOnError, ignoreCase);
            return assembly.CreateInstance(t.FullName) as T;
        }

        public static T CreateInstance<T>(this Assembly assembly) where T : class
        {
            var superType = typeof(T);

            var type = assembly
                .ExportedTypes
                .Single(t => superType.IsAssignableFrom(t))
                ;

            return assembly.CreateInstance(type.FullName) as T;
        }
    }
}
