using System;
using QTFK;
using QTFK.Models;
using System.Security;
using System.Security.Permissions;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.IO;
using System.Runtime.Remoting;
using System.Collections.Generic;

namespace QTFK.Services.Sandboxes
{
    public class DefaultSandboxFactory : ISandboxFactory
    {
        public ISandboxEnvironment<T> build<T>(Action<SandboxConfig> configure) where T : MarshalByRefObject, new()
        {
            SandboxConfig config;
            string pathToUntrusted;
            IEnumerable<IPermission> permissions;
            PermissionSet permSet;
            IEnumerable<StrongName> fullTrustAssemblies;
            AppDomainSetup adSetup;
            AppDomain newDomain;
            ObjectHandle handle;
            ISandboxEnvironment<T> sandboxEnvironment;

            Asserts.isNotNull(configure);

            config = new SandboxConfig
            {
                Permissions = new List<IPermission>(),
                TrustedAssemblies = new List<Assembly>(),
            };
            config.Permissions.Add(new SecurityPermission(SecurityPermissionFlag.Execution | SecurityPermissionFlag.UnmanagedCode));
            config.Permissions.Add(new ReflectionPermission(ReflectionPermissionFlag.MemberAccess));
            //config.TrustedAssemblies.Add(typeof(T).Assembly);

            configure(config);

            pathToUntrusted = config.PathToUntrusted ?? Environment.CurrentDirectory;

            permissions = (config.Permissions ?? Enumerable.Empty<IPermission>())
                .Where(p => p != null)
                ;

            permSet = new PermissionSet(PermissionState.None);
            foreach (var permission in permissions)
                permSet.AddPermission(permission);

            fullTrustAssemblies = (config.TrustedAssemblies ?? Enumerable.Empty<Assembly>())
                .Select(a => a.Evidence.GetHostEvidence<StrongName>())
                .Where(sn => sn != null)
                ;

            adSetup = new AppDomainSetup()
            {
                ApplicationBase = Path.GetFullPath(pathToUntrusted)
            };
            config.DomainName = config.DomainName ?? "QTFK-Sandbox";

            newDomain = AppDomain.CreateDomain(config.DomainName, null, adSetup, permSet, fullTrustAssemblies.ToArray());
            handle = Activator.CreateInstanceFrom(newDomain, typeof(T).Assembly.ManifestModule.FullyQualifiedName, typeof(T).FullName);
            sandboxEnvironment = new SandboxEnvironment<T>(newDomain, handle, (T)handle.Unwrap());

            return sandboxEnvironment;
        }
    }
}