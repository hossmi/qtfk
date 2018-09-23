using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.CSharp;

namespace QTFK.Services.Compilers
{
    public class CompilerWrapper 
    {
        public static Assembly buildInMemoryAssembly(string code, IEnumerable<string> referencedAssemblies)
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

                compilerResults = provider.CompileAssemblyFromSource(parameters, code);

                if (compilerResults.Errors.HasErrors)
                    throw new CompilerException(compilerResults.Errors);

                return compilerResults.CompiledAssembly;
            }
        }
    }
}