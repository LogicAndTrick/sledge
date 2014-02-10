using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sledge.Editor.UI
{
    public partial class UpdaterForm : Form
    {
        private readonly string _url;
        private readonly string _filename;
        public bool Completed { get; private set; }

        public UpdaterForm(string url, string filename)
        {
            _url = url;
            _filename = filename;
            Completed = false;

            InitializeComponent();
        }

        private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();

        private void UpdaterFormFormClosing(object sender, FormClosingEventArgs e)
        {
            _tokenSource.Cancel();
        }

        private void UpdaterFormLoad(object sender, EventArgs e)
        {
            DownloadUpdate(_url, _filename, _tokenSource.Token);
        }

        private Task DownloadUpdate(string url, string file, CancellationToken token)
        {
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
    }
}
