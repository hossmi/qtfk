using QTFK.Models;
using QTFK.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTFK.Extensions.Loggers
{
    public static class LoggerLevelExtension
    {
        public static void Debug(this ILogger<LogLevel> log, string message)
        {
            log.Log(LogLevel.Debug, message);
        }
        public static void Info(this ILogger<LogLevel> log, string message)
        {
            log.Log(LogLevel.Info, message);
        }
        public static void Warning(this ILogger<LogLevel> log, string message)
        {
            log.Log(LogLevel.Warning, message);
        }
        public static void Error(this ILogger<LogLevel> log, string message)
        {
            log.Log(LogLevel.Error, message);
        }
        public static void Fatal(this ILogger<LogLevel> log, string message)
        {
            log.Log(LogLevel.Fatal, message);
        }
    }
}
