using Microsoft.VisualStudio.TestTools.UnitTesting;
using QTFK.Services;
using QTFK.Services.Sandboxes;
using System.Security;
using QTFK.Extensions.Tests.Models;
using QTFK.Extensions.Sandboxing;
using QTFK.Extensions.Collections;
using System.Security.Permissions;

namespace QTFK.Extensions.Tests
{
    [TestClass]
    public class SandBoxTests
    {
        [TestMethod]
        [TestCategory("Sandbox")]
        public void SandBox_Extensions_test1()
        {
            ISandboxFactory factory = new DefaultSandboxFactory();
            var sandbox = factory.Build<Sandbox>();

            try
            {
                int result = sandbox.Run(() =>
                {
                    var x = new SuspiciousTestClass();
                    return x.SomeMethod(13);
                });
                Assert.Fail($"Expected {nameof(SecurityException)}");
            }
            catch (SecurityException)
            {
                //good!
            }
        }

        [TestMethod]
        [TestCategory("Sandbox")]
        public void SandBox_Extensions_test2()
        {
            ISandboxFactory factory = new DefaultSandboxFactory();
            var sandbox = factory.BuildSandbox();

            try
            {
                int result = sandbox.Run(() =>
                {
                    var x = new SuspiciousTestClass();
                    return x.SomeMethod(13);
                });
                Assert.Fail($"Expected {nameof(SecurityException)}");
            }
            catch (SecurityException)
            {
                //good!
            }
        }
    }
}
