using System;
using System.Runtime.Remoting;

namespace QTFK.Models
{
    public interface ISandboxEnvironment<T> : IDisposable
        where T : MarshalByRefObject, new()
    {
        AppDomain Domain { get; }
        ObjectHandle Handle { get; }
        T Instance { get; }
    }
}
