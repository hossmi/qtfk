using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using SampleLibrary;
using QTFK.Extensions.Assemblies;
using System.Linq;
using System.Collections.Generic;
using QTFK.Services.Compilers;

namespace QTFK.Core.Tests
{
    [TestClass]
    public class CodeCompilingTests
    {
        [TestMethod]
        public void compiling_simple_class()
        {
            string code;
            Assembly assembly;
            Type pepeType;
            object pepeInstance1, pepeInstance2;
            MethodInfo suma;
            int sumaResult;

            code = @"
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
            assembly = CompilerWrapper.buildInMemoryAssembly(code, new string[] { });

            pepeType = assembly.GetType("First.Pepe");

            pepeInstance1 = assembly.CreateInstance("First.Pepe");
            pepeInstance2 = Activator.CreateInstance(pepeType);

            Assert.IsInstanceOfType(pepeInstance1, pepeType);
            Assert.IsInstanceOfType(pepeInstance2, pepeType);
            Assert.AreNotSame(pepeInstance1, pepeInstance2);

            suma = pepeType.GetMethod("Suma");
            sumaResult = (int)suma.Invoke(pepeInstance1, new object[] { 2, 3 });
            Assert.AreEqual(5, sumaResult);
        }

        [TestMethod]
        public void compiling_simple_class_with_errors()
        {
            string code;
            Assembly assembly;

            code = @"
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
            try
            {
                assembly = CompilerWrapper.buildInMemoryAssembly(code, new string[] { });
                Assert.Fail("Expected compilation error");
            }
            catch (CompilerException)
            {
            }
        }

        [TestMethod]
        public void compiling_implementation_class_with_external_reference()
        {
            string code;
            Assembly assembly;
            ISampleService pepeService, samePepeService;

            code = @"
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
            assembly = CompilerWrapper.buildInMemoryAssembly(code, new string[] { "SampleLibrary.dll" });
            pepeService = assembly.createAssignableInstance<ISampleService>();
            samePepeService = (ISampleService)assembly.createAssignableInstance(typeof(ISampleService));

            Assert.AreEqual(13m, pepeService.SomeMethod(10m, 16m));
            Assert.AreEqual(13m, samePepeService.SomeMethod(10m, 16m));
        }

        [TestMethod]
        public void compiling_class_with_parametrized_constructor()
        {
            string code;
            ISampleService pepeService, samePepeService;
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
            assembly = CompilerWrapper.buildInMemoryAssembly(code, new string[] { "SampleLibrary.dll" });
            constructorParams = new object[] { 2m };
            pepeService = assembly.createAssignableInstance<ISampleService>(constructorParams);
            samePepeService = (ISampleService)assembly.createAssignableInstance(typeof(ISampleService), constructorParams);

            Assert.AreEqual(13m, pepeService.SomeMethod(10m, 16m));
            Assert.AreEqual(13m, samePepeService.SomeMethod(10m, 16m));
        }

        [TestMethod]
        public void compiling_two_classes_with_external_reference()
        {
            string code;
            Assembly assembly;
            ISampleService pepeService, troncoService;
            IEnumerable<ISampleService> instances;

            code = @"
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
            assembly = CompilerWrapper.buildInMemoryAssembly(code, new string[] { "SampleLibrary.dll" });
            
            pepeService = assembly.createInstance<ISampleService>("First.PepeSampleService");
            troncoService = assembly.createInstance<ISampleService>("First.TroncoSampleService");

            Assert.AreEqual(6, pepeService.SomeMethod(4, 8));
            Assert.AreEqual(12, troncoService.SomeMethod(4, 8));

            try
            {
                ISampleService service;

                service = assembly.createAssignableInstance<ISampleService>();
                Assert.Fail("Expected exception");
            }
            catch 
            {
            }

            instances = assembly.createAssignableInstances<ISampleService>();
            Assert.AreEqual(2, instances.Count());
        }
    }
}
