using System;

namespace Sledge.Common.Logging
{
    /// <summary>
    /// A simple log message object.
    /// </summary>
    public class LogMessage
    {
        public const string Fatal = "Fatal";
        public const string Error = "Error";
        public const string Warning = "Warning";
        public const string Info = "Info";
        public const string Debug = "Debug";

        public string Type { get; }
        public string Source { get; }
        public string Message { get; }
        public Exception Exception { get; }
        
        public LogMessage(string type, string source, string message, Exception exception)
        {
            Type = type;
            Source = source;
            Message = message;
            Exception = exception;
        }
    }
}
