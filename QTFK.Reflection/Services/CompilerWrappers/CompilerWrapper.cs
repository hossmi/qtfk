using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.CSharp;

namespace QTFK.Services.CompilerWrappers
{
    public class CompilerWrapper : ICompilerWrapper
    {
        public event Action<CompilerResults> CompilationResult;

        public Assembly Build(string code, IEnumerable<string> referencedAssemblies, Action<CompilerParameters> settings)
        {
            using (var provider = new CSharpCodeProvider())
            {
                CompilerParameters parameters = new CompilerParameters();

                foreach (var referencedAssembly in referencedAssemblies)
                    parameters.ReferencedAssemblies.Add(referencedAssembly);

                parameters.GenerateInMemory = true;
                parameters.GenerateExecutable = false;

                settings(parameters);

                var result = provider.CompileAssemblyFromSource(parameters, code);
                CompilationResult?.Invoke(result);

                return result.Errors.HasErrors
                    ? null
                    : result.CompiledAssembly
                    ;
            }
        }
    }
}