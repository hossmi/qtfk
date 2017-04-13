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
        private readonly IDictionary<T, ConsoleColor> _foreGroundColors;

        public ConsoleLogger(IDictionary<T, ConsoleColor> foreGroundColors = null)
        {
            _foreGroundColors = foreGroundColors ?? new Dictionary<T, ConsoleColor>();
        }

        public LoggerFilterDelegate<T> Filter { get; set; }

        public void Log(T level, string message)
        {
            if (Filter == null || Filter(level))
            {
                Console.ResetColor();
                if (_foreGroundColors.ContainsKey(level))
                    Console.ForegroundColor = _foreGroundColors[level];
                Console.WriteLine(message);
                Console.ResetColor();
            }
        }
    }

    public class ConsoleLogger : ConsoleLogger<LogLevel>
    {
        public ConsoleLogger(IDictionary<LogLevel, ConsoleColor> colors = null) 
            : base(colors ?? new Dictionary<LogLevel, ConsoleColor>
            {
                { LogLevel.Debug, ConsoleColor.DarkGray },
                { LogLevel.Info, ConsoleColor.Gray },
                { LogLevel.Warning, ConsoleColor.Yellow },
                { LogLevel.Error, ConsoleColor.Red },
                { LogLevel.Fatal, ConsoleColor.Magenta },
            })
        {

        }
    }
}
