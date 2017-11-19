using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace QTFK.Extensions.Assemblies
{
    public static class AssemblyExtensions
    {
        public static T CreateInstance<T>(this Assembly assembly, string implementationType) where T : class
        {
            Type t = assembly.GetType(implementationType, true, true);
            return assembly.CreateInstance(t.FullName) as T;
        }

        public static T CreateAssignableInstance<T>(this Assembly assembly) where T : class
        {
            return createAssignableInstances(assembly, typeof(T), new object[] { })
                .Cast<T>()
                .Single()
                ;
        }

        public static T CreateAssignableInstance<T>(this Assembly assembly, object[] constructorParameters) where T : class
        {
            return createAssignableInstances(assembly, typeof(T), constructorParameters)
                .Cast<T>()
                .Single()
                ;
        }

        public static object CreateAssignableInstance(this Assembly assembly, Type baseType)
        {
            return createAssignableInstances(assembly, baseType, new object[] { })
                .Single()
                ;
        }

        public static object CreateAssignableInstance(this Assembly assembly, Type baseType, object[] constructorParameters)
        {
            return createAssignableInstances(assembly, baseType, constructorParameters)
                .Single()
                ;
        }

        public static IEnumerable<T> CreateAssignableInstances<T>(this Assembly assembly) where T : class
        {
            return createAssignableInstances(assembly, typeof(T), new object[] { })
                .Cast<T>()
                ;
        }

        public static IEnumerable<T> CreateAssignableInstances<T>(this Assembly assembly, object[] constructorParameters) where T : class
        {
            return createAssignableInstances(assembly, typeof(T), constructorParameters)
                .Cast<T>()
                ;
        }

        private static IEnumerable<object> createAssignableInstances(Assembly assembly, Type baseType, object[] constructorParameters)
        {
            IEnumerable<object> instances;

            instances = assembly
                .ExportedTypes
                .Where(t => baseType.IsAssignableFrom(t))
                .Select(t => Activator.CreateInstance(t, constructorParameters))
                ;

            return instances;
        }
    }
}
