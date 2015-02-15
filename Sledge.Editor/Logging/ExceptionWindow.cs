using System;
using System.Collections.Specialized;
using System.Net;
using System.Windows.Forms;

namespace Sledge.Editor.Logging
{
    public partial class ExceptionWindow : Form
    {
        public ExceptionInfo ExceptionInfo { get; set; }

        public ExceptionWindow(ExceptionInfo info)
        {
            ExceptionInfo = info;
            InitializeComponent();
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
                ExceptionInfo.UserEnteredInformation = InfoTextBox.Text;
                using (var client = new WebClient())
                {
                    var values = new NameValueCollection();
                    values["Message"] = ExceptionInfo.Message;
                    values["Runtime"] = ExceptionInfo.RuntimeVersion;
                    values["OS"] = ExceptionInfo.OperatingSystem;
                    values["Version"] = ExceptionInfo.ApplicationVersion;
                    values["StackTrace"] = ExceptionInfo.FullStackTrace;
                    values["UserInfo"] = ExceptionInfo.UserEnteredInformation;
                    values["Source"] = ExceptionInfo.Source;
                    values["Date"] = ExceptionInfo.Date.ToUniversalTime().ToString("yyyy-MM-ddThh:mm:ssZ");
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
    }
}
