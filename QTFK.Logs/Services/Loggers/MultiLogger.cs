using QTFK.Models;
using System.Collections.Generic;

namespace QTFK.Services.Loggers
{
    public class MultiLogger<T> : ILogger<T>
    {
        private readonly IEnumerable<ILogger<T>> logs;

        public MultiLogger(IEnumerable<ILogger<T>> logs)
        {
            Asserts.isSomething(logs, $"{nameof(logs)} cannot be null.");

            this.logs = logs;
        }

        public void log(T level, string message)
        {
            foreach (var log in this.logs)
                log.log(level, message);
        }
    }

    public class MultiLogger : MultiLogger<LogLevel>
    {
        public MultiLogger(IEnumerable<ILogger<LogLevel>> logs) 
            : base(logs)
        {
        }
    }
}
