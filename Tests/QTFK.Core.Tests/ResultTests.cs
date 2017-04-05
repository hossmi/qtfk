using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QTFK.Models;
using System.Collections.Generic;

namespace QTFK.Core.Tests
{
    [TestClass]
    public class ResultTests
    {
        [TestMethod]
        [TestCategory("Result<T>")]
        public void Result_test_1()
        {
            Result res = new Result();
            Assert.IsTrue(res.Ok);
            Assert.IsNull(res.Exception);
            Assert.IsNull(res.Message);

            res = new Result(new DivideByZeroException("boooom"));
            Assert.IsFalse(res.Ok);
            Assert.IsNotNull(res.Exception);
            Assert.AreEqual("boooom", res.Message);

            try
            {
                Exception nullEx = null;
                res = new Result(nullEx);
                Assert.Fail("Constructor Result(Exception) must fail on null exception passed");
            }
            catch (ArgumentNullException)
            {
                //ok
            }
            catch (Exception ex)
            {
                //ooops unexpected
                throw ex;
            }
        }

        [TestMethod]
        [TestCategory("Result<T>")]
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
        [TestCategory("Result<T>")]
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
        [TestCategory("Result<T>")]
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
        [TestCategory("Result<T>")]
        public void Result_wrap_Exception_test()
        {
            var res = new Result<int>(() =>
            {
                int i = 2 + 3;
                i = 3 * i;
                throw new DivideByZeroException("boooom");
                //return i;
            });
            Assert.IsInstanceOfType(res.Exception, typeof(DivideByZeroException));

            res = res.Wrap(e => new EntryPointNotFoundException("wrapping exception with this message :)",e));
            Assert.IsInstanceOfType(res.Exception, typeof(EntryPointNotFoundException));
            Assert.AreEqual("wrapping exception with this message :)", res.Exception.Message);
            Assert.IsInstanceOfType(res.Exception.InnerException, typeof(DivideByZeroException));
            Assert.AreEqual("boooom", res.Exception.InnerException.Message);
        }
    }
}
