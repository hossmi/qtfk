using Microsoft.VisualStudio.TestTools.UnitTesting;
using QTFK.Core.Tests.Models;
using QTFK.Services.Sandboxing;
using System.Security;

namespace QTFK.Core.Tests
{
    [TestClass]
    public class SandBoxTests
    {
        [TestMethod]
        public void when_run_suspicious_code_in_sanbox_throws_expected_SecurityExpection()
        {
            try
            {
                DefaultSandboxBuilder
                    .buildFor<MethodInvoker>()
                    .create(instance =>
                    {
                        int result = instance.run(prv_suspiciousCode);
                        Assert.Fail($"Expected {nameof(SecurityException)}");
                    });
            }
            catch (SecurityException)
            {
            }
        }

        [TestMethod]
        public void when_run_suspicious_class_in_sandbox_throws_expected_SecurityException()
        {
            try
            {
                DefaultSandboxBuilder
                    .buildFor<MaliciousTestClass>()
                    .create(instance =>
                    {
                        int result = instance.someMethod(34);
                        Assert.Fail($"Expected {nameof(SecurityException)}");
                    });
            }
            catch (SecurityException)
            {
            }
        }

        private static int prv_suspiciousCode()
        {
            SuspiciousTestClass x;
            int someMethodResult;

            x = new SuspiciousTestClass();
            someMethodResult = x.someMethod(13);

            return someMethodResult;
        }

    }
}
