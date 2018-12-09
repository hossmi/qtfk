using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QTFK.Extensions.Exceptions;

namespace QTFK.Core.Tests
{
    [TestClass]
    public class ExceptionExtensionsTests
    {
        [TestMethod]
        public void when_wrap_exception_works_as_expected()
        {
            Exception innerException;
            DivideByZeroException upperExpection;

            innerException = new NotImplementedException("Booooom");
            upperExpection = innerException.wrap(ex => new DivideByZeroException("more booom!", ex));

            Assert.IsInstanceOfType(upperExpection, typeof(DivideByZeroException));
        }
    }
}
