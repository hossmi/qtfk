using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace QTFK.Extensions.Objects.Factory
{
    public static class GenericFactory
    {
        /// <summary>
        /// Creates a new instance of <typeparamref name="T"/> that receives all public instance properties which can be read and written, from <paramref name="source"/>
        /// </summary>
        /// <typeparam name="T"><paramref name="source"/> and returned type</typeparam>
        /// <param name="source">Instance that contains the original properties that will be carried over the new intance</param>
        /// <returns>The created instance</returns>
        /// <remarks>Intended to be used over POCO's and objects that represent simple models.
        /// Properties that implement special logic inside their getter/setter's may present undesired results.
        /// Members are NOT cloned, only properties, which may indirectly affect members.</remarks>
        public static T ShallowClone<T>(this T source) where T : class, new()
        {
            return source.ShallowClone(
                (property) => property.CanRead && property.CanWrite
                , BindingFlags.Public | BindingFlags.Instance);            
        }

        /// <summary>
        /// Creates a new instance of <typeparam name="T"/> that receives some selected properties from <paramref name="source"/>
        /// </summary>
        /// <typeparam name="T"><paramref name="source"/> and returned type</typeparam>
        /// <param name="source">Instance that contaitypeparamrefns the original properties that will be carried over the new intance</param>
        /// <param name="where">Filters which properties will carry over the new instance</param>
        /// <param name="bindingAttr">Filters <typeparamref name="T"/> properties by these <seealso cref="BindingFlags"/></param>
        /// <returns>The created instance</returns>
        public static T ShallowClone<T>(
            this T source
            , Func<PropertyInfo, bool> where
            , BindingFlags bindingAttr)
            where T : class, new()
        {
            if (source == null)
                return null;

            var result = new T();

            foreach (var property in typeof(T)
                .GetProperties(bindingAttr)
                .Where(where))
                property.SetValue(result, property.GetValue(source));

            return result;
        }
    }
}
