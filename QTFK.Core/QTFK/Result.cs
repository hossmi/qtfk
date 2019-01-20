using System;

namespace QTFK
{
    public class Result : Result<bool>
    {
        public Result(Action body)
            : base(() =>
            {
                body();
                return true;
            })
        {
        }
    }

    public class Result<T>
    {
        protected Exception exception;

        public Result(Func<T> body)
        {
            Asserts.isNotNull(body);

            try
            {
                Value = body();
            }
            catch (Exception ex)
            {
                this.exception = ex;
            }
        }

        public Exception Exception => this.exception;

        public bool Ok => this.Exception == null;

        public string Message => this.exception?.Message;

        public T Value { get; }

    }
}