using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace QTFK.Services.Sandboxes
{
    public class Sandbox : MarshalByRefObject
    {
        public T Run<T>(Func<T> method)
        {
            return method();
        }

        public void Run(Action method)
        {
            method();
        }

        public object Run(Type typename, string methodName, params object[] parameters)
        {
            return Run(typename.Assembly.FullName, typename.FullName, methodName, parameters);
        }

        public object Run(string assemblyName, string typeName, string methodName, params object[] parameters)
        {
            MethodInfo target = Assembly
                .Load(assemblyName)
                .GetType(typeName)
                .GetMethod(methodName)
                ;

            return target.Invoke(null, parameters);
        }
    }
}