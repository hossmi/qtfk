using QTFK.Models;
using System;

namespace QTFK.Services
{
    public interface ISandboxFactory
    {
        ISandboxEnvironment<T> build<T>(Action<SandboxConfig> configure) where T : MarshalByRefObject, new();
    }
}
