using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using QTFK.Services.CompilerWrappers;
using QTFK.Extensions.Compilers;
using QTFK.Services;

namespace QTFK.Core.Tests
{
    [TestClass]
    [TestCategory("Reflection")]
    public class CodeCompilingTests
    {
        [TestMethod]
        public void CodeCompiling_Test1()
        {
            string code = @"
    using System;

    namespace First
    {
        public class Pepe
        {
            public int Suma(int a, int b)
            {
                return a + b;
            }
        }
    }
";
            ICompilerWrapper compiler = new CompilerWrapper();
            Assembly assembly = compiler.Build(code);

            Type pepeType = assembly.GetType("First.Pepe");

            var pepeInstance1 = assembly.CreateInstance("First.Pepe");
            var pepeInstance2 = Activator.CreateInstance(pepeType);

            Assert.IsInstanceOfType(pepeInstance1, pepeType);
            Assert.IsInstanceOfType(pepeInstance2, pepeType);
            Assert.AreNotSame(pepeInstance1, pepeInstance2);

            MethodInfo suma = pepeType.GetMethod("Suma");
            int x = (int)suma.Invoke(pepeInstance1, new object[] { 2,3 });
            Assert.AreEqual(5, x);
        }

        [TestMethod]
        public void CodeCompiling_Test2()
        {
            string code = @"
    using System;

    namespace First
    {
        public class Pepe
        {
            public int Suma(int a, int b)
            {
                return a + b
            }
        }
    }
";
            ICompilerWrapper compiler = new CompilerWrapper();
            compiler.CompilationResult += result =>
            {
                if (!result.Errors.HasErrors)
                    Assert.Fail("Expected compilation error");
            };
            Assembly assembly = compiler.Build(code);
        }

        [TestMethod]
        public void CodeCompiling_Test3()
        {
            string code = @"
    using System;

    namespace First
    {
        public class Pepe
        {
            public int Suma(int a, int b)
            {
                return a + b
            }
        }
    }
";
            ICompilerWrapper compiler = new CompilerWrapper();
            Assembly assembly = compiler.Build(code);
            Assert.IsNull(assembly);
        }
    }
}
