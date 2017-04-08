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
        public static IDictionary<string, object> ToDictionary(this object model)
        {
            return ToDictionary(model, p => true);
        }

        public static IDictionary<string, object> ToDictionary(this object model, Func<PropertyInfo, bool> where)
        {
            return model
                .GetType()
                .GetProperties()
                .Where(where)
                .Select(p => new { K = p.Name, V = p.GetValue(model) })
                .ToDictionary(item => item.K, item => item.V)
                ;
        }

        public static IDictionary<string, object> ToDictionary(this object model, Type propertyType)
        {
            return ToDictionary(model, p => Attribute.IsDefined(p, propertyType));
        }
    }
}
