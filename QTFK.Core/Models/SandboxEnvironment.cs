using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;

namespace QTFK.Models
{
    public class SandboxEnvironment<T> : IDisposable
        where T : MarshalByRefObject, new()
    {
        private bool _disposed;

        public AppDomain Domain { get; set; }
        public ObjectHandle Handle { get; set; }
        public T Instance { get; set; }

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
