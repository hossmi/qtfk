using QTFK.Models;
using QTFK.Services;
using QTFK.Services.Sandboxes;
using System;

namespace QTFK.Extensions.Sandboxing
{
    public static class SandboxExtension
    {
        public static ISandboxEnvironment<T> build<T>(this ISandboxFactory factory) where T : MarshalByRefObject, new()
        {
            return factory.build<T>(c => { });
        }

        public static ISandboxEnvironment<Sandbox> buildSandbox(this ISandboxFactory factory, Action<SandboxConfig> configure = null)
        {
            return factory.build<Sandbox>(configure ?? (c => { }));
        }
    }
}
