using System;
using System.Reflection;

namespace QTFK.Services.Sandboxing
{
    public class MethodInvoker : MarshalByRefObject
    {
        public T run<T>(Func<T> method)
        {
            return method();
        }

        public void run(Action method)
        {
            method();
        }

        public object run(Type typename, string methodName, params object[] parameters)
        {
            return typename
                .GetMethod(methodName)
                .Invoke(null, parameters);
        }

        public object run(string assemblyName, string typeName, string methodName, params object[] parameters)
        {
            return Assembly
                .Load(assemblyName)
                .GetType(typeName)
                .GetMethod(methodName)
                .Invoke(null, parameters);
        }
    }
}