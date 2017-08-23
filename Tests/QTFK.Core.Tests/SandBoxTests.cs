using Microsoft.VisualStudio.TestTools.UnitTesting;
using QTFK.Services;
using QTFK.Services.Sandboxes;
using System.Security;
using QTFK.Core.Tests.Models;

namespace QTFK.Core.Tests
{
    [TestClass]
    public class SandBoxTests
    {
        [TestMethod]
        [TestCategory("Sandbox")]
        public void SandBox_core_test()
        {
            ISandboxFactory factory = new DefaultSandboxFactory();
            using (var sandboxEnv = factory.Build<Sandbox>(c => { }))
                try
                {
                    var sandbox = sandboxEnv.Instance;
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
