using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Sledge.Editor.Logging
{
    public static class Logger
    {
        public static void ShowException(Exception ex, string message = "")
        {
            var info = new ExceptionInfo(ex, message);
            var window = new ExceptionWindow(info);
            if (Editor.Instance == null || Editor.Instance.IsDisposed) window.Show();
            else window.Show(Editor.Instance);
        }
    }

    public class ExceptionInfo
    {
        public Exception Exception { get; set; }
        public string RuntimeVersion { get; set; }
        public string OperatingSystem { get; set; }
        public string ApplicationVersion { get; set; }
        public DateTime Date { get; set; }
        public string InformationMessage { get; set; }
        public string UserEnteredInformation { get; set; }

        public string Source
        {
            get { return Exception.Source; }
        }

        public string Message
        {
            get
            {
                var msg = String.IsNullOrWhiteSpace(InformationMessage) ? Exception.Message : InformationMessage;
                return msg.Split('\n').Select(x => x.Trim()).FirstOrDefault(x => !String.IsNullOrWhiteSpace(x));
            }
        }

        public string StackTrace
        {
            get { return Exception.StackTrace; }
        }

        public string FullStackTrace { get; set; }

        public ExceptionInfo(Exception exception, string info)
        {
            Exception = exception;
            RuntimeVersion = System.Environment.Version.ToString();
            Date = DateTime.Now;
            InformationMessage = info;
            ApplicationVersion = FileVersionInfo.GetVersionInfo(typeof(Logger).Assembly.Location).FileVersion;
            OperatingSystem = System.Environment.OSVersion.VersionString;

            var list = new List<Exception>();
            do
            {
                list.Add(exception);
                exception = exception.InnerException;
            } while (exception != null);

            FullStackTrace = (info + "\r\n").Trim();
            foreach (var ex in Enumerable.Reverse(list))
            {
                FullStackTrace += "\r\n" + ex.Message + " (" + ex.GetType().FullName + ")\r\n" + ex.StackTrace;
            }
            FullStackTrace = FullStackTrace.Trim();
        }
    }
}
