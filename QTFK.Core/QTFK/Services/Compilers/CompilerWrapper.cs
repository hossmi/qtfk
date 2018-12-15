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
            Asserts.isTrue(this.sources.Count > 0);

            using (var provider = new CSharpCodeProvider())
            {
                CompilerParameters parameters;
                CompilerResults compilerResults;

                parameters = new CompilerParameters();

                foreach (var referencedAssembly in this.referencedAssemblies)
                    parameters.ReferencedAssemblies.Add(referencedAssembly);

                parameters.GenerateInMemory = true;
                parameters.GenerateExecutable = false;

                compilerResults = provider.CompileAssemblyFromSource(parameters, this.sources.ToArray());

                if (compilerResults.Errors.HasErrors)
                    throw new CompilerException(compilerResults.Errors);

                return compilerResults.CompiledAssembly;
            }
        }
    }
}