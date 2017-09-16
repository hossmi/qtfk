using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;

namespace QTFK.Services
{
    public interface ICompilerWrapper
    {
        event Action<CompilerResults> CompilationResult;

        Assembly Build(string code, IEnumerable<string> referencedAssemblies, Action<CompilerParameters> settings);
    }
}