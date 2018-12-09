using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SampleLibrary;
using QTFK.Extensions.Types;

namespace QTFK.Reflection.Tests
{
    [TestClass]
    public class TypeExtensionTests
    {
        [TestMethod]
        public void Type_extension_tests()
        {
            Type concreteType, concreteFromAbstract;

            concreteType = typeof(ConcreteExtendedSampleService);
            concreteFromAbstract = typeof(ConcreteForAbstractExtendedSampleService);

            Assert.IsTrue(concreteType.implementsInterface(typeof(IExtendedSampleService)));
            Assert.IsTrue(concreteType.implementsInterface<IExtendedSampleService>());

            Assert.IsFalse(concreteType.implementsInterface(typeof(IOtherSampleService)));
            Assert.IsFalse(concreteType.implementsInterface<IOtherSampleService>());

            Assert.IsTrue(concreteType.implementsInterface(typeof(ISampleService)));
            Assert.IsTrue(concreteType.implementsInterface<ISampleService>());

            Assert.IsFalse(concreteType.extends(typeof(AbstractSampleService)));
            Assert.IsFalse(concreteType.extends<AbstractSampleService>());

            Assert.IsTrue(concreteFromAbstract.extends(typeof(AbstractSampleService)));
            Assert.IsTrue(concreteFromAbstract.extends<AbstractSampleService>());

            Assert.IsTrue(typeof(IOtherSampleService).extends<ISampleService>());
            Assert.IsFalse(typeof(ISampleService).extends<IOtherSampleService>());

            try { Assert.IsTrue(typeof(IOtherSampleService).extends<AbstractSampleService>()); Assert.Fail(); } catch { }
            try { Assert.IsTrue(typeof(ConcreteExtendedSampleService).extends<ISampleService>()); Assert.Fail(); } catch { }
        }
    }
}
