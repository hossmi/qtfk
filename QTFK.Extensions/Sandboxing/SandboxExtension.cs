using QTFK.Models;
using QTFK.Services;
using QTFK.Services.Sandboxes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTFK.Extensions.Sandboxing
{
    public static class SandboxExtension
    {
        public static T Build<T>(this ISandboxFactory factory) where T : MarshalByRefObject, new()
        {
            return factory.Build<T>(c => { });
        }

        public static Sandbox BuildSandbox(this ISandboxFactory factory, Action<SandboxConfig> configure = null)
        {
            return factory.Build<Sandbox>(configure ?? (c => { }));
        }
    }
}
