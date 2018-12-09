using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace QTFK.Core.Tests
{
    [TestClass]
    public class ResultTests
    {
        [TestMethod]
        public void Result_test_2()
        {
            Result res = new Result(() =>
            {
                int i = 2 + 3;
                i = 3 * i;
            });
            Assert.IsTrue(res.Ok);
            Assert.IsNull(res.Exception);

            res = new Result(() =>
            {
                int i = 2 + 3;
                i = 3 * i;
                throw new DivideByZeroException("boooom");
            });
            Assert.IsFalse(res.Ok);
            Assert.IsNotNull(res.Exception);
        }

        [TestMethod]
        public void Result_T_test()
        {
            var res = new Result<int>(() =>
            {
                int i = 2 + 3;
                i = 3 * i;
                return i;
            });
            Assert.IsTrue(res.Ok);
            Assert.IsNull(res.Exception);
            Assert.AreEqual(15, res.Value);

            res = new Result<int>(() =>
            {
                int i = 2 + 3;
                i = 3 * i;
                throw new DivideByZeroException("boooom");
            });
            Assert.IsFalse(res.Ok);
            Assert.IsNotNull(res.Exception);
            Assert.AreEqual(0, res.Value);
        }

        [TestMethod]
        public void Result_T_test_2()
        {
            var res = new Result<int?>(() =>
            {
                int i = 2 + 3;
                i = 3 * i;
                return i;
            });
            Assert.IsTrue(res.Ok);
            Assert.IsNull(res.Exception);
            Assert.AreEqual(15, res.Value);

            res = new Result<int?>(() =>
            {
                int i = 2 + 3;
                i = 3 * i;
                throw new DivideByZeroException("boooom");
            });
            Assert.IsFalse(res.Ok);
            Assert.IsNotNull(res.Exception);
            Assert.IsNull(res.Value);
        }

        [TestMethod]
        public void when_using_Result_T_with_exception_OK_returns_false_and_Exception_is_of_expected_type()
        {
            var res = new Result<int>(() =>
            {
                int i = 2 + 3;
                i = 3 * i;
                throw new DivideByZeroException("boooom");
            });
            Assert.IsInstanceOfType(res.Exception, typeof(DivideByZeroException));
            Assert.AreEqual("boooom", res.Exception.Message);
            Assert.AreEqual("boooom", res.Message);
            Assert.IsFalse(res.Ok);
        }

        [TestMethod]
        public void when_using_Result_with_exception_OK_returns_false_and_Exception_is_of_expected_type()
        {
            var res = new Result(() =>
            {
                int i = 2 + 3;
                i = 3 * i;
                throw new DivideByZeroException("boooom");
            });
            Assert.IsInstanceOfType(res.Exception, typeof(DivideByZeroException));
            Assert.AreEqual("boooom", res.Exception.Message);
            Assert.AreEqual("boooom", res.Message);
            Assert.IsFalse(res.Ok);
        }

        [TestMethod]
        public void when_using_Result_with_good_body_OK_returns_true_and_Exception_is_null()
        {
            var res = new Result(() =>
            {
                int i = 2 + 3;
                i = 3 * i;
            });
            Assert.IsNull(res.Exception);
            Assert.IsNull(res.Message);
            Assert.IsTrue(res.Ok);
        }

        [TestMethod]
        public void when_using_Result_T_with_good_body_OK_returns_true_Exception_is_null_and_value_has_its_expected_value()
        {
            var res = new Result<int>(() =>
            {
                int i = 2 + 3;
                i = 3 * i;

                return i;
            });
            Assert.IsNull(res.Exception);
            Assert.IsNull(res.Message);
            Assert.IsTrue(res.Ok);
            Assert.AreEqual(15, res.Value);
        }
    }
}
