using System;
using System.Linq;

namespace QTFK.Extensions.Types
{
    public static class TypeExtensions
    {
        public static bool implementsInterface(this Type type, Type interfaceType)
        {
            return prv_implementsInterface(type, interfaceType);
        }

        public static bool implementsInterface<T>(this Type type) where T : class
        {
            return prv_implementsInterface(type, typeof(T));
        }

        public static bool extends(this Type type, Type predecesorType)
        {
            return prv_extends(type, predecesorType);
        }

        public static bool extends<T>(this Type type) where T : class
        {
            return prv_extends(type, typeof(T));
        }

        private static bool prv_implementsInterface(Type type, Type interfaceType)
        {
            Asserts.isNotNull(type);
            Asserts.isNotNull(interfaceType);
            Asserts.isFalse(type.IsInterface);
            Asserts.isTrue(interfaceType.IsInterface);

            return type
                .GetInterfaces()
                .Any(t => t.Equals(interfaceType))
                ;
        }

        private static bool prv_extends(Type type, Type predecesorType)
        {
            bool result, bothAreInterfaces, bothAreClasses;

            Asserts.isNotNull(type);
            Asserts.isNotNull(predecesorType);
            //Asserts.check(type.IsClass, $"Parameter '{nameof(type)}' must be class type.");
            //Asserts.check(predecesorType.IsClass, $"Parameter '{nameof(predecesorType)}' must be class type.");
            Asserts.isFalse(type.Equals(predecesorType));

            bothAreInterfaces = type.IsInterface && predecesorType.IsInterface;
            bothAreClasses = type.IsClass && predecesorType.IsClass;

            Asserts.isTrue(bothAreInterfaces || bothAreClasses);

            if (bothAreInterfaces)
                result = type
                    .GetInterfaces()
                    .Any(t => t.Equals(predecesorType));
            else
                result = prv_extendsRecursive(type, predecesorType, 1);

            return result;
        }

        private static bool prv_extendsRecursive(Type type, Type predecesorType, int recursion)
        {
            bool result;
            Type baseType;

            Asserts.isTrue(recursion <= 1000);

            baseType = type.BaseType;

            if (baseType == null)
                result = false;
            else if (baseType.Equals(predecesorType))
                result = true;
            else
                result = prv_extendsRecursive(baseType, predecesorType, recursion + 1);

            return result;
        }
    }
}
