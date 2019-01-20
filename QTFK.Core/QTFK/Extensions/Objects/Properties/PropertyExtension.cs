using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace QTFK.Extensions.Objects.Properties
{
    public static class PropertyExtension
    {
        public static IEnumerable<PropertyInfo> getReadWriteProperties(this Type type)
        {
            return type
                .GetProperties()
                .Where(p => p.CanWrite && p.CanRead)
                ;
        }
    }
}
