using System;
using System.Reflection;
using System.Runtime.Remoting;
using System.Security;

namespace QTFK.Services
{
    public interface ISandboxBuilder<T> where T : MarshalByRefObject, new()
    {
        ISandboxBuilder<T> setPathToUntrusted(string path);
        ISandboxBuilder<T> addPermission(IPermission permission);
        ISandboxBuilder<T> setTrustedAssembly(Assembly assembly);
        ISandboxBuilder<T> setDomainName(string name);
        void create(Action<T> instance);
        void create(Action<AppDomain, ObjectHandle, T> instance);
    }
}
