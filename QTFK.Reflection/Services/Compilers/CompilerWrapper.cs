using System;
using System.Linq;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.CSharp;

namespace QTFK.Services.Compilers
{
    public class CompilerWrapper 
    {
        private static Exception prv_newCompilerException(CompilerErrorCollection errors)
        {
            CompilerException exception;
            CompilerFail[] errorArray;

            errorArray = new CompilerFail[errors.Count];

            for (int i = 0, n = errors.Count; i < n; i++)
            {
                CompilerFail fail;
                CompilerError error;

                error = errors[i];
                fail = new CompilerFail(error.FileName, error.Line, error.Column, error.ErrorNumber, error.ErrorText, error.IsWarning);
                errorArray[i] = fail;
            }

            exception = new CompilerException(errorArray);

            return exception;
        }

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
                    throw prv_newCompilerException(compilerResults.Errors);

                return compilerResults.CompiledAssembly;
            }
        }

    }
}