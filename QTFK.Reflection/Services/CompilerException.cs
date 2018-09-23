using System;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;

namespace QTFK.Services
{
    [Serializable]
    public class CompilerException : Exception
    {
        protected CompilerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public CompilerException(CompilerErrorCollection errors) : base("Error compiling code. See Errors property for details.")
        {
            this.Errors = errors;
        }

        public CompilerErrorCollection Errors { get; }
    }
}