using System;
using System.Diagnostics;
using QTFK.Services;
using QTFK.Models;

namespace QTFK.Services.Loggers
{
    public class DebugWindowsLogger<T> : ILogger<T>
    {
        public DebugWindowsLogger(string category = null)
        {
            Category = category;
        }

        public string Category { get; set; }

        public void Log(T level, string message)
        {
            Debug.WriteLine($"<<{level}>>: {message}", Category);
        }
    }

    public class DebugWindowsLogger : DebugWindowsLogger<LogLevel>
    {
        public DebugWindowsLogger(string category = null) : base(category) { }
    }
}