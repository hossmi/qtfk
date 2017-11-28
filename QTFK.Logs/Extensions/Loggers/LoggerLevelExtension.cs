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
        public static void debug(this ILogger<LogLevel> log, string message)
        {
            log.log(LogLevel.Debug, message);
        }
        public static void info(this ILogger<LogLevel> log, string message)
        {
            log.log(LogLevel.Info, message);
        }
        public static void warning(this ILogger<LogLevel> log, string message)
        {
            log.log(LogLevel.Warning, message);
        }
        public static void error(this ILogger<LogLevel> log, string message)
        {
            log.log(LogLevel.Error, message);
        }
        public static void fatal(this ILogger<LogLevel> log, string message)
        {
            log.log(LogLevel.Fatal, message);
        }
    }
}
