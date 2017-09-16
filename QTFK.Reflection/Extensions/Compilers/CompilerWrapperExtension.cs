using QTFK.Services;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace QTFK.Extensions.Compilers
{
    public static class CompilerWrapperExtension
    {
        public static Assembly Build(this ICompilerWrapper compilerWrapper, string code, IEnumerable<string> referencedAssemblies)
        {
            return compilerWrapper.Build(code, referencedAssemblies, p => { });
        }

        public static Assembly Build(this ICompilerWrapper compilerWrapper, string code)
        {
            return compilerWrapper.Build(code, Enumerable.Empty<string>(), p => { });
        }

        public static Assembly Build(this ICompilerWrapper compilerWrapper, string code, Action<CompilerParameters> settings)
        {
            return compilerWrapper.Build(code, Enumerable.Empty<string>(), settings);
        }
    }
}
