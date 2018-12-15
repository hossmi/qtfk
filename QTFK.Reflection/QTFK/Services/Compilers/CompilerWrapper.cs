using System;
using System.Linq;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.CSharp;
using QTFK.QTFK.Services;

namespace QTFK.Services.Compilers
{
    public class CompilerWrapper : ICompilerWarpper
    {
        private readonly IList<string> sources;
        private readonly ISet<string> referencedAssemblies;

        private CompilerWrapper()
        {
            this.sources = new List<string>();
            this.referencedAssemblies = new HashSet<string>();
        }

        public static ICompilerWarpper build()
        {
            return new CompilerWrapper();
        }

        public ICompilerWarpper addSource(string source)
        {
            Asserts.stringIsNotEmpty(source);
            this.sources.Add(source);

            return this;
        }

        public ICompilerWarpper addReferencedAssembly(string assembly)
        {
            Asserts.stringIsNotEmpty(assembly);
            this.referencedAssemblies.Add(assembly);

            return this;
        }

        public Assembly compile()
        {
            return prv_buildInMemoryAssembly(this.sources, this.referencedAssemblies);
        }

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

        private static Assembly prv_buildInMemoryAssembly(IEnumerable<string> sources, IEnumerable<string> referencedAssemblies)
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

                compilerResults = provider.CompileAssemblyFromSource(parameters, sources.ToArray());

                if (compilerResults.Errors.HasErrors)
                    throw prv_newCompilerException(compilerResults.Errors);

                return compilerResults.CompiledAssembly;
            }
        }
    }
}