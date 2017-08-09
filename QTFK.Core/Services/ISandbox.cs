using System;

namespace QTFK.Services
{
    public interface ISandbox
    {
        T Run<T>(Func<T> method);
    }
}