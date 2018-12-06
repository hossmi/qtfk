using QTFK.Models;
using System;
using System.Collections.Generic;

namespace QTFK.Services.Loggers
{

    /// <summary>
    /// Manages logging to a file.
    /// </summary>
    /// <typeparam name="T">Log level</typeparam>
    public class FileLogger<T> : ILogger<T>
    {
        public delegate string MessageBuilderDelegate(T level, string message);

        private readonly String filepath;
        private readonly MessageBuilderDelegate messageBuilder;

        /// <summary>
        /// Initializes a new instance of <see cref="FileLogger{T}"/> 
        /// </summary>
        /// <param name="filepath">Path to the file were the logs will be stored</param>
        /// <param name="messageBuilder">Defines how the message will be crafted. By default: [time] level | message</param>
        public FileLogger(String filepath, MessageBuilderDelegate messageBuilder = null)
        {
            Asserts.isFilled(filepath, $"{nameof(filepath)} cannot be empty");

            this.filepath = filepath;
            this.messageBuilder = messageBuilder ?? prv_defaultMessageBuilder;
        }

        /// <summary>
        /// Logs to the file indicated
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        public void log(T level, string message)
        {
            IEnumerable<string> lines;

            lines = new[] { this.messageBuilder(level, message) };

            System.IO.File.AppendAllLines(this.filepath, lines);
        }

        private static string prv_defaultMessageBuilder(T level, string message)
        {
            string result;

            result = $"[{DateTime.Now}] {level.ToString()} | {message}";

            return result;
        }
    }

    /// <summary>
    /// <see cref="FileLogger"/> using <see cref="Models.LogLevel"/> as log level.
    /// </summary>
    public class FileLogger : FileLogger<LogLevel>
    {
        public FileLogger(String filepath, MessageBuilderDelegate messageBuilder = null)
            : base(filepath, messageBuilder)
        {

        }
    }
}
