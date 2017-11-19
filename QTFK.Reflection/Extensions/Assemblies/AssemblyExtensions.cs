﻿using System;
using System.Collections.Generic;
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

        public static T CreateInstance<T>(this Assembly assembly) where T : class
        {
            return CreateInstance(assembly, typeof(T)) as T;
        }

        public static T CreateInstance<T>(this Assembly assembly, object[] constructorParameters) where T : class
        {
            return CreateInstance(assembly, typeof(T), constructorParameters) as T;
        }

        public static T CreateInstance<T>(this Assembly assembly, string implementationType, bool throwOnError, bool ignoreCase) where T : class
        {
            Type t = assembly.GetType(implementationType, throwOnError, ignoreCase);
            return assembly.CreateInstance(t.FullName) as T;
        }

        public static object CreateInstance(this Assembly assembly, Type type)
        {
            var assignableType = getAssignableType(assembly, type);

            return assembly.CreateInstance(assignableType.FullName);
        }

        public static object CreateInstance(this Assembly assembly, Type type, object[] constructorParameters)
        {
            object instance;
            Type assignableType;

            assignableType = getAssignableType(assembly, type);
            instance = Activator.CreateInstance(assignableType, constructorParameters);

            return instance;
        }

        private static Type getAssignableType(Assembly assembly, Type type)
        {
            return assembly
                .ExportedTypes
                .Single(t => type.IsAssignableFrom(t));
        }

        public static IEnumerable<T> CreateInstances<T>(this Assembly assembly) where T : class
        {
            var superType = typeof(T);

            return assembly
                .ExportedTypes
                .Where(t => superType.IsAssignableFrom(t))
                .Select(t => assembly.CreateInstance(t.FullName) as T)
                ;
        }
    }
}
