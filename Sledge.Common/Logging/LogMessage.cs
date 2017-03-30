using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sledge.Common.Logging
{
    public class LogMessage
    {
        public const string Fatal = "Fatal";
        public const string Error = "Error";
        public const string Warning = "Warning";
        public const string Info = "Info";
        public const string Debug = "Debug";

        public string Type { get; private set; }
        public string Source { get; private set; }
        public string Message { get; private set; }
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
