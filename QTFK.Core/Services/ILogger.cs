using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTFK.Services
{
    public delegate bool LoggerFilterDelegate<T>(T level);

    public interface ILogger<T>
    {
        LoggerFilterDelegate<T> Filter { get; set; }

        void Log(T level, string message);
    }
}
