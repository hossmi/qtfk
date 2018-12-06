using System.Diagnostics;
using QTFK.Models;

namespace QTFK.Services.Loggers
{
    public class DebugLogger<T> : ILogger<T>
    {
        public DebugLogger(string category = null)
        {
            Category = category;
        }

        public string Category { get; set; }

        public void log(T level, string message)
        {
            Debug.WriteLine($"<<{level}>>: {message}", Category);
        }
    }

    public class DebugLogger : DebugLogger<LogLevel>
    {
        public DebugLogger(string category = null)
            : base(category)
        {
        }
    }
}