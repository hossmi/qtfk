using QTFK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTFK.Services.Loggers
{
    public class ConsoleLogger<T> : ILogger<T>
    {
        private readonly IDictionary<T, ConsoleColor> foreGroundColors;
        private readonly LoggerFilterDelegate<T> filter;

        public ConsoleLogger(
            IDictionary<T, ConsoleColor> foreGroundColors = null
            , LoggerFilterDelegate<T> filter = null
            )
        {
            this.foreGroundColors = foreGroundColors ?? new Dictionary<T, ConsoleColor>();
            this.filter = filter;
        }

        public void log(T level, string message)
        {
            if (this.filter == null || this.filter(level))
            {
                Console.ResetColor();
                if (this.foreGroundColors.ContainsKey(level))
                    Console.ForegroundColor = this.foreGroundColors[level];
                Console.WriteLine(message);
                Console.ResetColor();
            }
        }
    }

    public class ConsoleLogger : ConsoleLogger<LogLevel>
    {
        private static IDictionary<LogLevel, ConsoleColor> defaultForeGroundColors = new Dictionary<LogLevel, ConsoleColor>
        {
            { LogLevel.Debug, ConsoleColor.DarkGray },
            { LogLevel.Info, ConsoleColor.Gray },
            { LogLevel.Warning, ConsoleColor.Yellow },
            { LogLevel.Error, ConsoleColor.Red },
            { LogLevel.Fatal, ConsoleColor.Magenta },
        };

        public ConsoleLogger(
            IDictionary<LogLevel, ConsoleColor> colors = null
            , LoggerFilterDelegate<LogLevel> filter = null
            )
            : base(colors ?? defaultForeGroundColors, filter)
        {

        }
    }
}
