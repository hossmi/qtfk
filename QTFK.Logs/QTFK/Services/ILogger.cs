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
        void log(T level, string message);
    }
}
