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
        public static Assembly build(this ICompilerWrapper compilerWrapper, string code, IEnumerable<string> referencedAssemblies)
        {
            return compilerWrapper.build(code, referencedAssemblies, p => { });
        }

        public static Assembly build(this ICompilerWrapper compilerWrapper, string code)
        {
            return compilerWrapper.build(code, Enumerable.Empty<string>(), p => { });
        }

        public static Assembly build(this ICompilerWrapper compilerWrapper, string code, Action<CompilerParameters> settings)
        {
            return compilerWrapper.build(code, Enumerable.Empty<string>(), settings);
        }
    }
}
