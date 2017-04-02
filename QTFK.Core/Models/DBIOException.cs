using System;
using System.Runtime.Serialization;

namespace QTFK.Models
{
    [Serializable]
    public class DBIOException : Exception
    {
        public DBIOException()
        {
        }

        public DBIOException(string message) : base(message)
        {
        }

        public DBIOException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DBIOException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}