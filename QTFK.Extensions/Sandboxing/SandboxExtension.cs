using QTFK.Models;
using QTFK.Services;
using QTFK.Services.Sandboxes;
using System;

namespace QTFK.Extensions.Sandboxing
{
    public static class SandboxExtension
    {
        public static ISandboxEnvironment<T> Build<T>(this ISandboxFactory factory) where T : MarshalByRefObject, new()
        {
            return factory.Build<T>(c => { });
        }

        public static ISandboxEnvironment<Sandbox> BuildSandbox(this ISandboxFactory factory, Action<SandboxConfig> configure = null)
        {
            return factory.Build<Sandbox>(configure ?? (c => { }));
        }
    }
}
