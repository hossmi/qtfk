using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QTFK.Services;
using QTFK.Services.Sandboxes;
using QTFK.Models;

namespace QTFK.Core.Tests
{
    [TestClass]
    public class SandBoxTests
    {
        public int SomeMethod(int x)
        {
            return 2 * x;
        }

        [TestMethod]
        public void TestMethod1()
        {
            ISandboxFactory factory = new SandboxFactory();
            ISandbox sandbox = factory.Build(new SandboxConfig());

            sandbox.Run(() =>
            {
                return SomeMethod(13);
            });
        }
    }
}
