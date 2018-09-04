using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.Common.Translations;
using Sledge.Editor.Properties;

namespace Sledge.Editor.Update
{
    public partial class UpdaterForm : Form
    {
        private readonly UpdateReleaseDetails _details;
        private readonly string _filename;
        private readonly string _downloadingLabel;

        public UpdaterForm(UpdateReleaseDetails details, ITranslationStringProvider translations)
        {
            _details = details;
            _filename = Path.Combine(Path.GetTempPath(), details.FileName);

            InitializeComponent();

            var prefix = GetType().FullName;

            Icon = Resources.Sledge;
            Text = translations.GetString(prefix + ".Title");
            StatusLabel.Text = translations.GetString(prefix + ".UpdateAvailable");
            _downloadingLabel = translations.GetString(prefix + ".Downloading") ?? "";

            ReleaseDetails.Text = $"{_details.Name}\r\n\r\n{_details.Changelog.Replace("\r", "").Replace("\n", "\r\n")}";
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
                Oy.Publish("Sledge:Editor:UpdateDownloaded", _filename);
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
            StatusLabel.Text = $"{_downloadingLabel} {Path.GetFileName(_filename)}, {e.ProgressPercentage:0}%";
        }

        private void CancelButtonClicked(object sender, EventArgs e)
        {
            Close();
        }

        private void ReleaseNotesLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/LogicAndTrick/sledge/releases");
        }
    }
}
