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
        protected static NullLogger<T> instance;

        protected NullLogger() { }

        public static ILogger<T> Instance
        {
            get
            {
                if (instance == null)
                    instance = new NullLogger<T>();
                return instance;
            }
        }

        public void log(T level, string message) { }
    }

    public class NullLogger : NullLogger<LogLevel>
    {
    }
}
