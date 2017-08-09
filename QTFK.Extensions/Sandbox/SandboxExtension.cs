using QTFK.Models;
using QTFK.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTFK.Extensions.Sandbox
{
    public static class SandboxExtension
    {
        public static void Run(this ISandbox sandbox, Action method)
        {
            sandbox.Run<int>(() =>
            {
                method();
                return 0;
            });

            ISandboxFactory f = null;
        }

        public static ISandbox Build(this ISandboxFactory factory, Action<SandboxConfig> configure)
        {
            var config = new SandboxConfig();
            configure(config);
            return factory.Build(config);
        }
    }
}
