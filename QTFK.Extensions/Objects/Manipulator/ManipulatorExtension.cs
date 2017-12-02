using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace QTFK.Extensions.Objects.Manipulator
{
    public static class ManipulatorExtension
    {
        public static PropertyCollection manipulate(this object o)
        {
            return prv_manipulate(o);
        }

        public static void manipulate(this object o, Action<PropertyCollection> manipulator)
        {
            PropertyCollection propertyCollection;

            propertyCollection = prv_manipulate(o);
            manipulator(propertyCollection);
        }

        private static PropertyCollection prv_manipulate(object o)
        {
            PropertyCollection propertyCollection;
            IEnumerable<PropertyInfo> rProps, wProps;

            rProps = o
                .GetType()
                .GetProperties()
                .Where(prop => prop.CanRead)
                ;

            wProps = o
                .GetType()
                .GetProperties()
                .Where(prop => prop.CanWrite)
                ;

            propertyCollection = new PropertyCollection(o, rProps, wProps);

            return propertyCollection;
        }

        public class PropertyCollection
        {
            private object instance;
            private readonly IDictionary<string, PropertyInfo> readableProps;
            private readonly IDictionary<string, PropertyInfo> writableProps;

            internal PropertyCollection(object o
                , IEnumerable<PropertyInfo> readableProps
                , IEnumerable<PropertyInfo> writableProps)
            {
                this.instance = o;
                this.readableProps = readableProps.ToDictionary(p => p.Name);
                this.writableProps = writableProps.ToDictionary(p => p.Name);
            }

            public PropertyCollection Set<T>(string propertyName, T value)
            {
                PropertyInfo prop;
                if (this.writableProps.TryGetValue(propertyName, out prop))
                    prop.SetValue(this.instance, value);

                return this;
            }

            public T Get<T>(string propertyName)
            {
                T output;

                prv_get<T>(propertyName, out output);

                return output;
            }

            public PropertyCollection Get<T>(string propertyName, out T output)
            {
                return prv_get<T>(propertyName, out output);
            }

            private PropertyCollection prv_get<T>(string propertyName, out T output)
            {
                PropertyInfo prop;

                output = default(T);
                if (this.readableProps.TryGetValue(propertyName, out prop))
                {
                    if (output.GetType().IsAssignableFrom(prop.PropertyType))
                        output = (T)prop.GetValue(this.instance);
                }

                return this;
            }

        }
    }
}
