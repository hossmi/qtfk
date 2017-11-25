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
        public void sandBox_core_test()
        {
            ISandboxFactory factory;

            factory = new DefaultSandboxFactory();

            using (var sandboxEnv = factory.build<Sandbox>(c => { }))
                try
                {
                    Sandbox sandbox;
                    int result;

                    sandbox = sandboxEnv.Instance;
                    result = sandbox.run(() =>
                    {
                        SuspiciousTestClass x;
                        int someMethodResult;

                        x = new SuspiciousTestClass();
                        someMethodResult = x.SomeMethod(13);

                        return someMethodResult;
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
