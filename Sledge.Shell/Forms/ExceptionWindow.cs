using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Windows.Forms;

namespace Sledge.Shell.Forms
{
    public partial class ExceptionWindow : Form
    {
        public ExceptionInfo Info { get; set; }

        public ExceptionWindow(Exception ex)
        {
            InitializeComponent();

            var info = new ExceptionInfo(ex, "");
            Info = info;
            FrameworkVersion.Text = info.RuntimeVersion;
            OperatingSystem.Text = info.OperatingSystem;
            SledgeVersion.Text = info.ApplicationVersion;
            FullError.Text = info.FullStackTrace;
        }

        private void SubmitButtonClicked(object sender, EventArgs e)
        {
            Submit();
            Close();
        }

        private void Submit()
        {
            try
            {
                Info.UserEnteredInformation = InfoTextBox.Text;
                using (var client = new WebClient())
                {
                    var values = new NameValueCollection();
                    values["Message"] = Info.Message;
                    values["Runtime"] = Info.RuntimeVersion;
                    values["OS"] = Info.OperatingSystem;
                    values["Version"] = Info.ApplicationVersion;
                    values["StackTrace"] = Info.FullStackTrace;
                    values["UserInfo"] = Info.UserEnteredInformation;
                    values["Source"] = Info.Source;
                    values["Date"] = Info.Date.ToUniversalTime().ToString("yyyy-MM-ddThh:mm:ssZ");
                    client.UploadValues("http://bugs.sledge-editor.com/Bug/AutoSubmit", "POST", values);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error sending bug report: " + ex.Message);
            }
        }

        private void CancelButtonClicked(object sender, EventArgs e)
        {
            Close();
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

            public string Source => Exception.Source;

            public string Message
            {
                get
                {
                    var msg = String.IsNullOrWhiteSpace(InformationMessage) ? Exception.Message : InformationMessage;
                    return msg.Split('\n').Select(x => x.Trim()).FirstOrDefault(x => !String.IsNullOrWhiteSpace(x));
                }
            }

            public string StackTrace => Exception.StackTrace;

            public string FullStackTrace { get; set; }

            public ExceptionInfo(Exception exception, string info)
            {
                Exception = exception;
                RuntimeVersion = System.Environment.Version.ToString();
                Date = DateTime.Now;
                InformationMessage = info;
                ApplicationVersion = FileVersionInfo.GetVersionInfo(typeof(ExceptionWindow).Assembly.Location).FileVersion;
                // ApplicationVersion = FileVersionInfo.GetVersionInfo(exception.TargetSite?.DeclaringType?.Assembly.Location ?? typeof(ExceptionWindow).Assembly.Location).FileVersion;
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
}
