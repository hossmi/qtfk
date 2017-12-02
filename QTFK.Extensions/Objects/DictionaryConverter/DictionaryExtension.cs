using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace QTFK.Extensions.Objects.DictionaryConverter
{
    public static class DictionaryExtension
    {
        public static IDictionary<string, object> toDictionary(this object model)
        {
            return prv_toDictionary(model, p => true);
        }

        public static IDictionary<string, object> toDictionary(this object model, Type propertyType)
        {
            return prv_toDictionary(model, p => Attribute.IsDefined(p, propertyType));
        }

        public static IDictionary<string, object> toDictionary(this object model, Func<PropertyInfo, bool> where)
        {
            return prv_toDictionary(model, where);
        }

        private static IDictionary<string, object> prv_toDictionary(object model, Func<PropertyInfo, bool> where)
        {
            return model
                .GetType()
                .GetProperties()
                .Where(where)
                .Select(p => new { K = p.Name, V = p.GetValue(model) })
                .ToDictionary(item => item.K, item => item.V)
                ;
        }
    }
}
