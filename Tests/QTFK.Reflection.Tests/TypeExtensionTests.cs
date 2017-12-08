using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SampleLibrary;
using QTFK.Extensions.Types;

namespace QTFK.Reflection.Tests
{
    [TestClass]
    [TestCategory("Reflection")]
    public class TypeExtensionTests
    {
        [TestMethod]
        public void Type_extension_tests()
        {
            Type concreteType;
            Type[] implementedInterfaces;

            concreteType = typeof(ConcreteExtendedSampleService);
            implementedInterfaces = concreteType.GetInterfaces();

            //Assert.IsTrue(concreteType.implementsInterface(typeof(IExtendedSampleService).FullName));
            Assert.IsTrue(concreteType.implementsInterface(typeof(IExtendedSampleService)));
            Assert.IsTrue(concreteType.implementsInterface<IExtendedSampleService>());

            //Assert.IsFalse(concreteType.implementsInterface(typeof(IOtherSampleService).FullName));
            Assert.IsFalse(concreteType.implementsInterface(typeof(IOtherSampleService)));
            Assert.IsFalse(concreteType.implementsInterface<IOtherSampleService>());

            //Assert.IsTrue(concreteType.implementsInterface(typeof(ISampleService).FullName));
            Assert.IsTrue(concreteType.implementsInterface(typeof(ISampleService)));
            Assert.IsTrue(concreteType.implementsInterface<ISampleService>());
        }
    }
}
