using System;
using System.Runtime.Serialization;

namespace QTFK.Services.Compilers
{
    [Serializable]
    public class CompilerException : Exception
    {
        protected CompilerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public CompilerException(CompilerFail[] errors) : base("Error compiling code. See Errors property for details.")
        {
            this.Errors = errors;
        }

        public CompilerFail[] Errors { get; }
    }
}