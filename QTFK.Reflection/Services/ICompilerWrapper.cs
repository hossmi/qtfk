using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;

namespace QTFK.Services
{
    public interface ICompilerWrapper
    {
        Assembly build(string code, IEnumerable<string> referencedAssemblies, Action<CompilerParameters> settings);
    }
}