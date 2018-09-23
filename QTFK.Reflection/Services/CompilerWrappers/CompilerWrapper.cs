using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.CSharp;

namespace QTFK.Services.CompilerWrappers
{
    public class CompilerWrapper : ICompilerWrapper
    {
        public Assembly build(string code, IEnumerable<string> referencedAssemblies, Action<CompilerParameters> settings)
        {
            using (var provider = new CSharpCodeProvider())
            {
                CompilerParameters parameters;
                CompilerResults compilerResults;

                parameters = new CompilerParameters();

                foreach (var referencedAssembly in referencedAssemblies)
                    parameters.ReferencedAssemblies.Add(referencedAssembly);

                parameters.GenerateInMemory = true;
                parameters.GenerateExecutable = false;

                settings(parameters);

                compilerResults = provider.CompileAssemblyFromSource(parameters, code);

                if (compilerResults.Errors.HasErrors)
                    throw new CompilerException(compilerResults.Errors);

                return compilerResults.CompiledAssembly;
            }
        }
    }
}