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

namespace QTFK
{
    public class Sandbox : MarshalByRefObject
    {
        public static Sandbox Build(string pathToUntrusted = null
            , IEnumerable<IPermission> permissions = null
            , IEnumerable<Assembly> trustedAssemblies = null
            )
        {
            pathToUntrusted = pathToUntrusted ?? Environment.CurrentDirectory;

            PermissionSet permSet = new PermissionSet(PermissionState.None);
            permissions = (permissions ?? Enumerable.Empty<IPermission>())
                .Concat(new IPermission[]
                {
                    //new SecurityPermission(PermissionState.Unrestricted),
                    new SecurityPermission(SecurityPermissionFlag.Execution
                        //| SecurityPermissionFlag.Assertion 
                        //| SecurityPermissionFlag.RemotingConfiguration  
                        //| SecurityPermissionFlag.SerializationFormatter 
                        | SecurityPermissionFlag.UnmanagedCode
                    ),
                    //new ReflectionPermission(PermissionState.Unrestricted),
                    new ReflectionPermission(ReflectionPermissionFlag.MemberAccess 
                        //| ReflectionPermissionFlag.RestrictedMemberAccess
                        //| ReflectionPermissionFlag.TypeInformation
                        //| ReflectionPermissionFlag.ReflectionEmit
                        ),
                });
            foreach (var permission in permissions)
                permSet.AddPermission(permission);
            //permSet.PermitOnly();

            trustedAssemblies = trustedAssemblies ?? Enumerable.Empty<Assembly>();
            var fullTrustAssemblies = trustedAssemblies
                .Select(a => a.Evidence.GetHostEvidence<StrongName>())
                .Concat(new StrongName[] { typeof(Sandbox).Assembly.Evidence.GetHostEvidence<StrongName>() })
                .Where(sn => sn != null)
                .ToArray()
                ;

            AppDomainSetup adSetup = new AppDomainSetup()
            {
                ApplicationBase = Path.GetFullPath(pathToUntrusted)
            };

            AppDomain newDomain = AppDomain.CreateDomain("Sandbox", null, adSetup, permSet, fullTrustAssemblies);

            ObjectHandle handle = Activator.CreateInstanceFrom(
                newDomain, typeof(Sandbox).Assembly.ManifestModule.FullyQualifiedName,
                typeof(Sandbox).FullName
                );

            Sandbox newDomainInstance = (Sandbox)handle.Unwrap();

            return newDomainInstance;
        }

        public object Run(Type typename, string methodName, params object[] parameters)
        {
            return Run(typename.Assembly.FullName, typename.FullName, methodName, parameters);
        }

        public object Run(string assemblyName, string typeName, string methodName, params object[] parameters)
        {
            MethodInfo target = Assembly
                .Load(assemblyName)
                .GetType(typeName)
                .GetMethod(methodName)
                ;

            return target.Invoke(null, parameters);
        }

        public T Run<T>(Func<T> method)
        {
            return method.Invoke();
        }

        public void Run(Action method)
        {
            method.Invoke();
        }
    }
}
