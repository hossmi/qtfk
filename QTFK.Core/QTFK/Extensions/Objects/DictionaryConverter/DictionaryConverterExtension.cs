using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace QTFK.Extensions.Objects.DictionaryConverter
{
    public static class DictionaryConverterExtension
    {
        public static IDictionary<string, object> toDictionary(this object model)
        {
            return prv_toDictionary(model, p => true);
        }

        public static IDictionary<string, object> toDictionary(this object model, Type propertyType)
        {
            return prv_toDictionary(model, p => Attribute.IsDefined(p, propertyType));
        }

        public static IDictionary<string, object> toDictionary(this object model, Func<PropertyInfo, bool> propertySelector)
        {
            return prv_toDictionary(model, propertySelector);
        }

        private static IDictionary<string, object> prv_toDictionary(object model, Func<PropertyInfo, bool> propertySelector)
        {
            return model
                .GetType()
                .GetProperties()
                .Where(propertySelector)
                .Select(p => new { K = p.Name, V = p.GetValue(model) })
                .ToDictionary(item => item.K, item => item.V)
                ;
        }
    }
}
