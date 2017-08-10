using System.Collections.Generic;
using System.Reflection;
using System.Security;

namespace QTFK.Models
{
    public class SandboxConfig
    {
        public string PathToUntrusted { get; set; }
        public ICollection<IPermission> Permissions { get; set; }
        public ICollection<Assembly> TrustedAssemblies { get; set; }
        public string DomainName { get; set; }
    }
}