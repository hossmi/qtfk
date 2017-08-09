using QTFK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTFK.Services
{
    public interface ISandboxFactory
    {
        ISandbox Build(SandboxConfig config);
    }
}
