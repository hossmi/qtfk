﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        public void sandBox_Extensions_test1()
        {
            ISandboxFactory factory;

            factory = new DefaultSandboxFactory();
            using (var sandboxEnv = factory.build<Sandbox>())
                try
                {
                    Sandbox sandbox;
                    int result;

                    sandbox = sandboxEnv.Instance;
                    result = sandbox.run(prv_suspiciousCode);
                    Assert.Fail($"Expected {nameof(SecurityException)}");
                }
                catch (SecurityException)
                {
                    //good!
                }
        }

        [TestMethod]
        [TestCategory("Sandbox")]
        public void sandBox_Extensions_test2()
        {
            ISandboxFactory factory;

            factory = new DefaultSandboxFactory();
            using (var sandboxEnv = factory.buildSandbox())
                try
                {
                    var sandbox = sandboxEnv.Instance;
                    int result = sandbox.run(prv_suspiciousCode);
                    Assert.Fail($"Expected {nameof(SecurityException)}");
                }
                catch (SecurityException)
                {
                    //good!
                }
        }

        [TestMethod]
        [TestCategory("Sandbox")]
        public void sandBox_Extensions_test3()
        {
            ISandboxFactory factory;

            factory = new DefaultSandboxFactory();
            using (var sandboxEnv = factory.build<MaliciousTestClass>())
                try
                {
                    var sandbox = sandboxEnv.Instance;
                    int result = sandbox.someMethod(13);
                    Assert.Fail($"Expected {nameof(SecurityException)}");
                }
                catch (SecurityException)
                {
                    //good!
                }
        }

        private static int prv_suspiciousCode()
        {
            SuspiciousTestClass x;
            int someMethodResult;

            x = new SuspiciousTestClass();
            someMethodResult = x.SomeMethod(13);

            return someMethodResult;
        }

    }
}
