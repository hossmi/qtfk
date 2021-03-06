﻿using System.Reflection;

namespace QTFK.Services
{
    public interface ICompilerWarpper
    {
        ICompilerWarpper addSource(string source);
        ICompilerWarpper addReferencedAssembly(string assembly);
        Assembly compile();
    }
}