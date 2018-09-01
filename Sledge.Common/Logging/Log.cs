using System;
using System.Diagnostics;
using System.Threading.Tasks;
using LogicAndTrick.Oy;

namespace Sledge.Common.Logging
{
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

        [Conditional("DEBUG")]
        public static async void Debug(string source, string message)
        {
            await Send(LogMessage.Debug, source, message, null);
        }
    }
}