using QTFK.Extensions.Exceptions;
using System;
using System.Linq;

namespace QTFK.Models
{
    public class Result
    {
        protected Exception exception;

        public Exception Exception { get { return this.exception; } }
        public bool Ok { get { return Exception == null; } }
        public string Message { get { return this.exception?.Message; } }

        protected Result()
        {

        }

        public Result(Action body)
        {
            Asserts.isSomething(body, "Delegate 'body' cannot be null.");

            try
            {
                body();
            }
            catch (Exception ex)
            {
                this.exception = ex;
            }
        }

        public Result Wrap<TExcep>(Func<Exception, TExcep> exceptionWrapperDelegate) where TExcep : Exception
        {
            this.exception = this.exception.wrap<TExcep>(exceptionWrapperDelegate);
            return this;
        }
    }

    public class Result<T> : Result
    {
        public T Value { get; }

        public Result(Func<T> body)
        {
            Asserts.isSomething(body, "Delegate 'body' cannot be null.");

            try
            {
                Value = body();
            }
            catch (Exception ex)
            {
                this.exception = ex;
            }
        }

        public new Result<T> Wrap<TExcep>(Func<Exception, TExcep> exceptionWrapperDelegate) where TExcep : Exception
        {
            this.exception = this.exception.wrap<TExcep>(exceptionWrapperDelegate);
            return this;
        }

    }
}