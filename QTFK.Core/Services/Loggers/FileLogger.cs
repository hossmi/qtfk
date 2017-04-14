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
        private readonly ILogger<T> _failSafeLogger;

        /// <summary>
        /// Initializes a new instance of <see cref="FileLogger{T}"/> 
        /// </summary>
        /// <param name="filepath">Path to the file were the logs will be stored</param>
        /// <param name="messageBuilder">Defines how the message will be crafted. By default: [time] level | message</param>
        /// <param name="failSafeLogger"><see cref="ILogger{T}"/> to use if this one fails. 
        /// It will log one entry for the exception thrown, and another one with the original entry. 
        /// It is encouraged to use one that won't throw. No logger will be used by default</param>
        public FileLogger(String filepath, Func<T, String, String> messageBuilder = null, ILogger<T> failSafeLogger = null)
        {
            if (String.IsNullOrEmpty(filepath))
                throw new ArgumentNullException(nameof(filepath));

            _filepath = filepath;
            _messageBuilder = messageBuilder ?? _defaultMessageBuilder;
            _failSafeLogger = failSafeLogger ?? NullLogger<T>.Instance;
        }

        /// <summary>
        /// Logs to the file indicated
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        public void Log(T level, string message)
        {
            try
            {
                System.IO.File.AppendAllLines(_filepath, new[] { _messageBuilder(level, message) });
            }
            catch(Exception ex)
            {
                _failSafeLogger.Log(default(T), $"{nameof(FileLogger)} threw an exception trying to log: {ex.Message}");
                _failSafeLogger.Log(level, message);
            }
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
        public FileLogger(String filepath, Func<LogLevel, String, String> messageBuilder = null, ILogger<LogLevel> failSafeLogger = null)
            : base(filepath, messageBuilder, failSafeLogger)
        {

        }
    }
}
