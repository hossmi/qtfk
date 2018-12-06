using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;

namespace QTFK.Models
{
    public class SandboxEnvironment<T> : ISandboxEnvironment<T>
        where T : MarshalByRefObject, new()
    {
        private bool _disposed;

        public AppDomain Domain { get; private set; }
        public ObjectHandle Handle { get; private set; }
        public T Instance { get; private set; }

        public SandboxEnvironment(AppDomain domain, ObjectHandle handle, T instance)
        {
            Domain = domain;
            Handle = handle;
            Instance = instance;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                AppDomain.Unload(Domain);
                Domain = null;
                Handle = null;
                Instance = null;
                _disposed = true;
            }
        }
    }
}
