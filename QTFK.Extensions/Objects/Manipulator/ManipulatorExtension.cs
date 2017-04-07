using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace QTFK.Extensions.Objects.Manipulator
{
    public static class ManipulatorExtension
    {
        public static PropertyCollection Manipulate(this object o)
        {
            var rProps = o
                .GetType()
                .GetProperties()
                .Where(prop => prop.CanRead)
                ;

            var wProps = o
                .GetType()
                .GetProperties()
                .Where(prop => prop.CanWrite)
                ;

            return new PropertyCollection(o, rProps, wProps);
        }

        public static void Manipulate(this object o, Action<PropertyCollection> manipulator)
        {
            var rProps = o
                .GetType()
                .GetProperties()
                .Where(prop => prop.CanRead)
                ;

            var wProps = o
                .GetType()
                .GetProperties()
                .Where(prop => prop.CanWrite)
                ;

            manipulator(new PropertyCollection(o, rProps, wProps));
        }

        public class PropertyCollection
        {
            private object _object;
            private readonly IDictionary<string, PropertyInfo> _readableProps;
            private readonly IDictionary<string, PropertyInfo> _writableProps;

            internal PropertyCollection(object o
                , IEnumerable<PropertyInfo> readableProps
                , IEnumerable<PropertyInfo> writableProps)
            {
                _object = o;
                _readableProps = readableProps.ToDictionary(p => p.Name);
                _writableProps = writableProps.ToDictionary(p => p.Name);
            }

            public PropertyCollection Set<T>(string propertyName, T value)
            {
                PropertyInfo prop;
                if (_writableProps.TryGetValue(propertyName, out prop))
                    prop.SetValue(_object, value);

                return this;
            }
            public PropertyCollection Get<T>(string propertyName, out T output)
            {
                PropertyInfo prop;
                output = default(T);
                if (_readableProps.TryGetValue(propertyName, out prop))
                {
                    if (output.GetType().IsAssignableFrom(prop.PropertyType))
                        output = (T)prop.GetValue(_object);
                }

                return this;
            }

            public T Get<T>(string propertyName)
            {
                T output;
                Get<T>(propertyName, out output);
                return output;
            }
        }
    }
}
