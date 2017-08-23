using QTFK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace QTFK.Services
{
    public interface ISandboxFactory
    {
        SandboxEnvironment<T> Build<T>(Action<SandboxConfig> configure) where T : MarshalByRefObject, new();
    }
}
