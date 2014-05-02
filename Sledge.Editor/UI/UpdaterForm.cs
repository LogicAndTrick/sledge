using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sledge.Common.Mediator;

namespace Sledge.Editor.UI
{
    public partial class UpdaterForm : Form
    {
        private readonly UpdateReleaseDetails _details;
        private readonly string _filename;
        public bool Completed { get; private set; }

        public UpdaterForm(UpdateReleaseDetails details, string filename)
        {
            _details = details;
            _filename = filename;
            Completed = false;

            InitializeComponent();

            Text = "Update Available! Current version: " + FileVersionInfo.GetVersionInfo(typeof (Editor).Assembly.Location).FileVersion;
            
            StatusLabel.Text = "A new version of Sledge is available!\nWould you like to download it now?";
            ReleaseDetails.Text = _details.Name + "\r\n\r\n" + _details.Changelog.Replace("\r", "").Replace("\n", "\r\n");
        }

        private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();

        private void UpdaterFormFormClosing(object sender, FormClosingEventArgs e)
        {
            _tokenSource.Cancel();
        }

        private void DownloadButtonClicked(object sender, EventArgs e)
        {
            DownloadUpdate(_details.DownloadUrl, _filename, _tokenSource.Token);
        }

        private Task DownloadUpdate(string url, string file, CancellationToken token)
        {
            StartButton.Enabled = false;

            var tcs = new TaskCompletionSource<bool>();
            var wc = new WebClient();

            wc.Headers.Add(HttpRequestHeader.UserAgent, "LogicAndTrick/sledge");
            wc.Headers.Remove(HttpRequestHeader.Accept);
            wc.Headers.Add(HttpRequestHeader.Accept, "application/octet-stream");

            wc.DownloadFileCompleted += (obj, args) =>
            {
                tcs.SetResult(true);
                Completed = true;
                if (InvokeRequired) BeginInvoke(new Action(Close));
                else Close();
            };
            wc.DownloadProgressChanged += UpdateProgress;
            token.Register(wc.CancelAsync);
            wc.DownloadFileAsync(new Uri(url), file);
            return tcs.Task;
        }

        private void UpdateProgress(object sender, DownloadProgressChangedEventArgs e)
        {
            ProgressBar.Value = e.ProgressPercentage;
            if (!ProgressBar.Visible) ProgressBar.Visible = true;
            StatusLabel.Text = "Downloading " + (Path.GetFileNameWithoutExtension(_filename) ?? "") + "... " + e.ProgressPercentage.ToString("0") + "%";
        }

        private void CancelButtonClicked(object sender, EventArgs e)
        {
            Close();
        }

        private void ReleaseNotesLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Mediator.Publish(EditorMediator.OpenWebsite, "https://github.com/LogicAndTrick/sledge/releases");
        }
    }
}
