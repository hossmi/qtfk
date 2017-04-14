using QTFK.Models;
using System;

namespace QTFK.Services.Loggers
{
    /// <summary>
    /// Manages logging to a file.
    /// </summary>
    /// <typeparam name="T">Log level</typeparam>
    public class FileLogger<T> : ILogger<T>
    {
        private readonly String _filepath;
        private readonly Func<T, String, String> _messageBuilder;

        /// <summary>
        /// Initializes a new instance of <see cref="FileLogger{T}"/> 
        /// </summary>
        /// <param name="filepath">Path to the file were the logs will be stored</param>
        /// <param name="messageBuilder">Defines how the message will be crafted. By default: [time] level | message</param>
        public FileLogger(String filepath, Func<T, String, String> messageBuilder = null)
        {
            if (String.IsNullOrEmpty(filepath))
                throw new ArgumentNullException(nameof(filepath));

            _filepath = filepath;
            _messageBuilder = messageBuilder ?? _defaultMessageBuilder;
        }

        /// <summary>
        /// Logs to the file indicated
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        public void Log(T level, string message)
        {
            System.IO.File.AppendAllLines(_filepath, new[] { _messageBuilder(level, message) });
        }

        private static Func<T, String, String> _defaultMessageBuilder
            = (T level, String message) =>
            {
                return $"[{DateTime.Now}] {level.ToString()} | {message}";
            };
    }

    /// <summary>
    /// <see cref="FileLogger"/> using <see cref="Models.LogLevel"/> as log level.
    /// </summary>
    public class FileLogger : FileLogger<LogLevel>
    {
        public FileLogger(String filepath, Func<LogLevel, String, String> messageBuilder = null)
            : base(filepath, messageBuilder)
        {

        }
    }
}
