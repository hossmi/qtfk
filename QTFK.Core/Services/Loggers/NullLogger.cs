using QTFK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTFK.Services.Loggers
{
    public class NullLogger<T> : ILogger<T>
    {
        protected static NullLogger<T> _instance;

        protected NullLogger() { }

        public static ILogger<T> Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new NullLogger<T>();
                return _instance;
            }
        }

        public void Log(T level, string message) { }
    }

    public class NullLogger : NullLogger<LogLevel>
    {
    }
}
