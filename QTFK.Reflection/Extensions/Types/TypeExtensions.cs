using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
            Asserts.isSomething(type, "Parameter 'type' cannot be null.");
            Asserts.isSomething(interfaceType, "Parameter 'interfaceType' cannot be null.");
            Asserts.check(type.IsInterface == false, "Parameter 'type' cannot be an interface.");
            Asserts.check(interfaceType.IsInterface == true, "Parameter 'interfaceType' must be an interface.");

            return type
                .GetInterfaces()
                .Any(t => t.Equals(interfaceType))
                ;
        }

        private static bool prv_extends(Type type, Type predecesorType)
        {
            bool result, bothAreInterfaces, bothAreClasses;

            Asserts.isSomething(type, $"Parameter '{nameof(type)}' cannot be null.");
            Asserts.isSomething(predecesorType, $"Parameter '{nameof(predecesorType)}' cannot be null.");
            //Asserts.check(type.IsClass, $"Parameter '{nameof(type)}' must be class type.");
            //Asserts.check(predecesorType.IsClass, $"Parameter '{nameof(predecesorType)}' must be class type.");
            Asserts.check(type.Equals(predecesorType) == false, $"Parameter '{nameof(type)}' and '{nameof(predecesorType)}' are the same.");

            bothAreInterfaces = type.IsInterface && predecesorType.IsInterface;
            bothAreClasses = type.IsClass && predecesorType.IsClass;

            Asserts.check(bothAreInterfaces || bothAreClasses, $"Both types must be interfaces or (concrete\abstract).");

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

            Asserts.check(recursion <= 1000, $"'Function {nameof(prv_extendsRecursive)}' has reached maximum recursion level");

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
