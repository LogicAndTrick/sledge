using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicAndTrick.Oy;

namespace Sledge.Common.Logging
{
    public class LogMessage
    {
        public const string Fatal = "Fatal";
        public const string Error = "Error";
        public const string Warning = "Warning";
        public const string Info = "Info";

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

    public static class Log
    {
        public static async Task Send(string type, string source, string message, Exception ex)
        {
            await Oy.Publish("Log", new LogMessage(type, source, message, ex));
        }

        public static async void Fatal(string source, string message, Exception ex)
        {
            await Send(LogMessage.Fatal, source, message, ex);
        }

        public static async void Error(string source, string message, Exception ex)
        {
            await Send(LogMessage.Error, source, message, ex);
        }

        public static async void Warning(string source, string message, Exception ex = null)
        {
            await Send(LogMessage.Warning, source, message, ex);
        }

        public static async void Info(string source, string message)
        {
            await Send(LogMessage.Info, source, message, null);
        }
    }
}
