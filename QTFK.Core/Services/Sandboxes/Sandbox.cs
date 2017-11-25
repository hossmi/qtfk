using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace QTFK.Services.Sandboxes
{
    public class Sandbox : MarshalByRefObject
    {
        public T run<T>(Func<T> method)
        {
            T result;

            result = method();

            return result;
        }

        public void run(Action method)
        {
            method();
        }

        public object run(Type typename, string methodName, params object[] parameters)
        {
            return prv_run(typename.Assembly.FullName, typename.FullName, methodName, parameters);
        }

        public object run(string assemblyName, string typeName, string methodName, params object[] parameters)
        {
            return prv_run(assemblyName, typeName, methodName, parameters);
        }

        private object prv_run(string assemblyName, string typeName, string methodName, object[] parameters)
        {
            MethodInfo target;
            object result;

            target = Assembly
                .Load(assemblyName)
                .GetType(typeName)
                .GetMethod(methodName)
                ;

            result = target.Invoke(null, parameters);

            return result;
        }
    }
}