using System;
using System.Linq;

namespace QTFK.Models
{
    public class Result
    {
        protected Exception _exception;

        public Exception Exception { get { return _exception; } }
        public bool Ok { get { return Exception == null; } }
        public string Message { get { return _exception?.Message; }  }

        public Result() { }

        public Result(Action body)
        {
            try
            {
                body();
            }
            catch (Exception ex)
            {
                _exception = ex;
            }
        }

        public Result(Exception ex)
        {
            if (ex == null)
                throw new ArgumentNullException(nameof(ex),"Result(Exception) constructor needs a not null exception object.");
            _exception = ex;
        }

        public void Wrap(Func<Exception, EntryPointNotFoundException> exceptionWrapperDelegate)
        {
            _exception = exceptionWrapperDelegate(_exception);
        }
    }

    public class Result<T> : Result
    {
        public Result(T value) 
        {
            Value = value;
        }

        public Result(Exception ex) : base(ex) { }

        public Result(Func<T> body)
        {
            try
            {
                Value = body();
            }
            catch (Exception ex)
            {
                _exception = ex;
            }
        }

        public T Value { get; }
    }
}