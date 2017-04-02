using QTFK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTFK.Services.Loggers
{
    public class MultiLogger<T> : ILogger<T>
    {
        private readonly IEnumerable<ILogger<T>> _logs;

        public MultiLogger(IEnumerable<ILogger<T>> logs)
        {
            _logs = logs;
        }

        public void Log(T level, string message)
        {
            foreach (var log in _logs)
                log.Log(level, message);
        }
    }

    public class MultiLogger : MultiLogger<LogLevel>
    {
        public MultiLogger(IEnumerable<ILogger<LogLevel>> logs) : base(logs) { }
    }
}
