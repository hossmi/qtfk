using System;
using System.CodeDom.Compiler;
using System.Linq;
using System.Runtime.Serialization;

namespace QTFK.Services.Compilers
{
    [Serializable]
    public class CompilerException : Exception
    {
        protected CompilerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public CompilerException(CompilerErrorCollection errors) : base("Error compiling code. See Errors property for details.")
        {
            this.Errors = errors
                .Cast<CompilerError>()
                .Select(error => new CompilerFail(
                    error.FileName,
                    error.Line,
                    error.Column,
                    error.ErrorNumber,
                    error.ErrorText,
                    error.IsWarning))
                .ToArray();
        }

        public CompilerFail[] Errors { get; }
    }
}