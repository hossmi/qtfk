using System;
using QTFK.Models;
using System.Security;
using System.Security.Permissions;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.IO;
using System.Runtime.Remoting;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace QTFK.Services.Sandboxes
{
    public class DefaultSandboxFactory : ISandboxFactory
    {
        public SandboxEnvironment<T> Build<T>(Action<SandboxConfig> configure) where T : MarshalByRefObject, new()
        {
            var config = new SandboxConfig
            {
                Permissions = new List<IPermission>(),
                TrustedAssemblies = new List<Assembly>(),
            };
            config.Permissions.Add(new SecurityPermission(SecurityPermissionFlag.Execution | SecurityPermissionFlag.UnmanagedCode));
            config.Permissions.Add(new ReflectionPermission(ReflectionPermissionFlag.MemberAccess));
            //config.TrustedAssemblies.Add(typeof(T).Assembly);

            configure(config);

            string pathToUntrusted = config.PathToUntrusted ?? Environment.CurrentDirectory;

            var permissions = (config.Permissions ?? Enumerable.Empty<IPermission>())
                .Where(p => p != null)
                ;

            PermissionSet permSet = new PermissionSet(PermissionState.None);
            foreach (var permission in permissions)
                permSet.AddPermission(permission);

            var fullTrustAssemblies = (config.TrustedAssemblies ?? Enumerable.Empty<Assembly>())
                .Select(a => a.Evidence.GetHostEvidence<StrongName>())
                .Where(sn => sn != null)
                ;

            AppDomainSetup adSetup = new AppDomainSetup()
            {
                ApplicationBase = Path.GetFullPath(pathToUntrusted)
            };

            AppDomain newDomain = AppDomain.CreateDomain(config.DomainName ?? "QTFK-Sandbox", null, adSetup, permSet, fullTrustAssemblies.ToArray());

            ObjectHandle handle = Activator.CreateInstanceFrom(
                newDomain, typeof(T).Assembly.ManifestModule.FullyQualifiedName,
                typeof(T).FullName
                );

            return new SandboxEnvironment<T>
            {
                Domain = newDomain,
                Handle = handle,
                Instance = (T)handle.Unwrap(),
            };
        }
    }
}