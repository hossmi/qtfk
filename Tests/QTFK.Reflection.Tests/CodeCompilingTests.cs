﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using QTFK.Services.CompilerWrappers;
using QTFK.Extensions.Compilers;
using QTFK.Services;
using SampleLibrary;
using QTFK.Extensions.Assemblies;
using System.Linq;

namespace QTFK.Core.Tests
{
    [TestClass]
    [TestCategory("Reflection")]
    public class CodeCompilingTests
    {
        [TestMethod]
        public void Compiling_simple_class()
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
            int x = (int)suma.Invoke(pepeInstance1, new object[] { 2, 3 });
            Assert.AreEqual(5, x);
        }

        [TestMethod]
        public void Compiling_simple_class_with_errors()
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
        public void Compiling_simple_class_with_errors_checking_null_assembly()
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

        [TestMethod]
        public void Compiling_implementation_class_with_external_reference()
        {
            string code = @"
    using System;
    using SampleLibrary;

    namespace First
    {
        public class PepeSampleService : ISampleService
        {
            public int SomeMethod(decimal a, decimal b)
            {
                return (int)((a + b) / 2m);
            }
        }
    }
";
            ICompilerWrapper compiler = new CompilerWrapper();
            Assembly assembly = compiler.Build(code, new string[] { "SampleLibrary.dll" });
            ISampleService pepeService = assembly.CreateAssignableInstance<ISampleService>();
            ISampleService samePepeService = (ISampleService)assembly.CreateAssignableInstance(typeof(ISampleService));
            Assert.AreEqual(13m, pepeService.SomeMethod(10m, 16m));
            Assert.AreEqual(13m, samePepeService.SomeMethod(10m, 16m));
        }

        [TestMethod]
        public void Compiling_class_with_parametrized_constructor()
        {
            string code;
            ISampleService pepeService, samePepeService;
            ICompilerWrapper compiler;
            Assembly assembly;
            object[] constructorParams;

            code = @"
    using System;
    using SampleLibrary;

    namespace First
    {
        public class PepeSampleService : ISampleService
        {
            private decimal divisor;

            public PepeSampleService(decimal divisor)
            {
                if(divisor == 0m)
                    throw new ArgumentException(""'divisor' cannot be zero"");
                
                this.divisor = divisor;
            }

            public int SomeMethod(decimal a, decimal b)
            {
                return (int)((a + b) / this.divisor);
            }
        }
    }
";
            compiler = new CompilerWrapper();
            assembly = compiler.Build(code, new string[] { "SampleLibrary.dll" });
            constructorParams = new object[] { 2m };
            pepeService = assembly.CreateAssignableInstance<ISampleService>(constructorParams);
            samePepeService = (ISampleService)assembly.CreateAssignableInstance(typeof(ISampleService), constructorParams);

            Assert.AreEqual(13m, pepeService.SomeMethod(10m, 16m));
            Assert.AreEqual(13m, samePepeService.SomeMethod(10m, 16m));
        }

        [TestMethod]
        public void Compiling_two_classes_with_external_reference()
        {
            string code = @"
    using System;
    using SampleLibrary;

    namespace First
    {
        public class PepeSampleService : ISampleService
        {
            public int SomeMethod(decimal a, decimal b)
            {
                return (int)((a + b) / 2m);
            }
        }

        public class TroncoSampleService : ISampleService
        {
            public int SomeMethod(decimal a, decimal b)
            {
                return (int)(a + b);
            }
        }
    }
";
            ICompilerWrapper compiler = new CompilerWrapper();
            Assembly assembly = compiler.Build(code, new string[] { "SampleLibrary.dll" });
            
            ISampleService pepeService = assembly.CreateInstance<ISampleService>("First.PepeSampleService");
            ISampleService troncoService = assembly.CreateInstance<ISampleService>("First.TroncoSampleService");

            Assert.AreEqual(6, pepeService.SomeMethod(4, 8));
            Assert.AreEqual(12, troncoService.SomeMethod(4, 8));

            try
            {
                ISampleService service = assembly.CreateAssignableInstance<ISampleService>();
                Assert.Fail("Expected exception");
            }
            catch 
            {
            }

            var instances = assembly.CreateAssignableInstances<ISampleService>();
            Assert.AreEqual(2, instances.Count());

            var noInstances = assembly.CreateAssignableInstances<ICompilerWrapper>();
            Assert.AreEqual(0, noInstances.Count());
        }
    }
}