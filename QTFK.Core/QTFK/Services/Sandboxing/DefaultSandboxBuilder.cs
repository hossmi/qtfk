using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using QTFK.Services;

namespace QTFK.QTFK.Services.Sandboxing
{
    public static class DefaultSandboxBuilder
    {
        public static ISandboxBuilder<T> buildFor<T>() where T : MarshalByRefObject, new()
        {
            return new PrvSandboxBuilder<T>();
        }

        private class PrvSandboxBuilder<T> : ISandboxBuilder<T> where T : MarshalByRefObject, new()
        {
            private const string DEFAULT_DOMAIN_NAME = "QTFK-Sandbox";
            private readonly IList<IPermission> permissions;
            private readonly IList<Assembly> trustedAssemblies;
            private string DomainName;
            private string PathToUntrusted;

            public PrvSandboxBuilder()
            {
                this.permissions = new List<IPermission>();
                this.trustedAssemblies = new List<Assembly>();

                this.DomainName = DEFAULT_DOMAIN_NAME;
                this.PathToUntrusted = Environment.CurrentDirectory;
                this.permissions.Add(new SecurityPermission(SecurityPermissionFlag.Execution | SecurityPermissionFlag.UnmanagedCode));
                this.permissions.Add(new ReflectionPermission(ReflectionPermissionFlag.MemberAccess));
                //this.trustedAssemblies.Add(typeof(T).Assembly);
            }

            public ISandboxBuilder<T> setDomainName(string name)
            {
                Asserts.stringIsNotEmpty(name);
                this.DomainName = name;
                return this;
            }

            public ISandboxBuilder<T> setPathToUntrusted(string path)
            {
                Asserts.stringIsNotEmpty(path);
                this.PathToUntrusted = path;
                return this;
            }

            public ISandboxBuilder<T> addPermission(IPermission permission)
            {
                Asserts.isNotNull(permission);
                this.permissions.Add(permission);
                return this;
            }

            public ISandboxBuilder<T> setTrustedAssembly(Assembly assembly)
            {
                Asserts.isNotNull(assembly);
                this.trustedAssemblies.Add(assembly);
                return this;
            }

            public void create(Action<T> instance)
            {
                throw new NotImplementedException();
            }

            public void create(Action<AppDomain, ObjectHandle, T> instance)
            {
                Asserts.isNotNull(instance);

                PermissionSet permSet = new PermissionSet(PermissionState.None);

                foreach (var permission in this.permissions)
                    permSet.AddPermission(permission);

                StrongName[] fullTrustAssemblies = this.trustedAssemblies
                    .Select(a => a.Evidence.GetHostEvidence<StrongName>())
                    .Where(sn => sn != null)
                    .ToArray();

                AppDomainSetup adSetup = new AppDomainSetup()
                {
                    ApplicationBase = Path.GetFullPath(this.PathToUntrusted)
                };

                AppDomain newDomain = AppDomain.CreateDomain(this.DomainName, null, adSetup, permSet, fullTrustAssemblies);
                ObjectHandle handle = Activator.CreateInstanceFrom(newDomain, typeof(T).Assembly.ManifestModule.FullyQualifiedName, typeof(T).FullName);
                T newInstance = (T)handle.Unwrap();
                instance(newDomain, handle, newInstance);
                AppDomain.Unload(newDomain);
            }
        }
    }
}
