using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ICSharpCode.SharpZipLib.Zip;

namespace Sledge.Editor.Updater
{
    public partial class UpdaterForm : Form
    {
        private class UpdateSource
        {
            public string Name { get; set; }
            public string Url { get; set; }

            public string GetUrl(string version)
            {
                return String.Format(Url, version);
            }
        }

        private class UpdateCheckResult
        {
            public string Version { get; set; }
            public DateTime Date { get; set; }
            public string DownloadUrl { get; set; }
        }

        public UpdaterForm()
        {
            InitializeComponent();

            this.Shown += (s, e) => DoUpdate();
        }

        private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();

        private void UpdaterFormFormClosing(object sender, FormClosingEventArgs e)
        {
            _tokenSource.Cancel();
        }

        private void UpdaterFormLoad(object sender, EventArgs e)
        {
        }

        private void DoUpdate()
        {
            var sources = GetUpdateSources();
            var version = GetCurrentVersion();
            var directory = GetCurrentDirectory();
            var found = false;
            if (version != null && directory != null)
            {
                foreach (var source in sources)
                {
                    StatusLabel.Text = "Checking " + source.Name + "...";
                    var result = GetUpdateCheckResult(source, version);
                    if (result == null) continue;
                    found = true;
                    if (String.Equals(result.Version, version, StringComparison.InvariantCultureIgnoreCase))
                    {
                        MessageBox.Show("You are already on the latest version.", "Sledge Updater");
                        break;
                    }
                    var updateDirectory = Path.Combine(directory, "Update");
                    if (!Directory.Exists(updateDirectory)) Directory.CreateDirectory(updateDirectory);
                    var fileName = result.DownloadUrl.Split('\\', '/').Last();
                    var download = Path.Combine(updateDirectory, fileName);
                    if (!File.Exists(download))
                    {
                        DownloadUpdate(result.DownloadUrl, download, _tokenSource.Token)
                            .ContinueWith(t =>
                                              {
                                                  InstallUpdate(directory, download);
                                                  Directory.Delete(updateDirectory, true);
                                                  Process.Start(Path.Combine(directory, "Sledge.Editor.exe"));
                                                  Application.Exit();
                                              });
                    }
                    return;
                }
            }
            if (!found)
            {
                MessageBox.Show("Update failed. Check that the UpdateSource.txt file is up-to-date.", "Sledge Updater");
            }
            Close();
        }

        private string GetCurrentDirectory()
        {
            return Path.GetDirectoryName(typeof(UpdaterForm).Assembly.Location);
        }

        private IEnumerable<UpdateSource> GetUpdateSources()
        {
            var dir = Path.GetDirectoryName(typeof (UpdaterForm).Assembly.Location);
            if (dir == null) yield break;
            var file = Path.Combine(dir, "UpdateSources.txt");
            if (!File.Exists(file)) yield break;
            var lines = File.ReadAllLines(file);
            foreach (var line in lines)
            {
                if (String.IsNullOrWhiteSpace(line) || line.StartsWith("#")) continue;
                var split = line.Split(':');
                if (split.Length < 2) continue;
                var us = new UpdateSource
                             {
                                 Name = split[0],
                                 Url = String.Join(":", split.Skip(1))
                             };
                yield return us;
            }
        }

        private String GetCurrentVersion()
        {
            var dir = Path.GetDirectoryName(typeof(UpdaterForm).Assembly.Location);
            if (dir == null) return null;
            var file = Path.Combine(dir, "Sledge.Editor.exe");
            if (!File.Exists(file)) return null;
            var info = FileVersionInfo.GetVersionInfo(file);
            return info.FileVersion;
        }

        private UpdateCheckResult GetUpdateCheckResult(UpdateSource source, string version)
        {
            try
            {
                using (var downloader = new WebClient())
                {
                    var str = downloader.DownloadString(source.GetUrl(version)).Split('\n', '\r');
                    if (str.Length < 3 || String.IsNullOrWhiteSpace(str[0]))
                    {
                        return null;
                    }
                    return new UpdateCheckResult {Version = str[0], Date = DateTime.Parse(str[1]), DownloadUrl = str[2]};
                }
            }
            catch
            {
                return null;
            }
        }

        private Task DownloadUpdate(string url, string file, CancellationToken token)
        {
            var tcs = new TaskCompletionSource<bool>();
            var downloader = new WebClient();
            downloader.DownloadFileCompleted += (obj, args) => tcs.SetResult(true);
            downloader.DownloadProgressChanged += UpdateProgress;
            token.Register(downloader.CancelAsync);
            downloader.DownloadFileAsync(new Uri(url), file);
            return tcs.Task;
        }

        private void UpdateProgress(object sender, DownloadProgressChangedEventArgs e)
        {
            ProgressBar.Value = e.ProgressPercentage;
            if (!ProgressBar.Visible) ProgressBar.Visible = true;
            StatusLabel.Text = "Downloading update... " + e.ProgressPercentage.ToString("0") + "%";
        }

        private void InstallUpdate(string installDir, string file)
        {
            StatusLabel.Text = "Installing update...";
            // File is already in a folder at this point.
            var updateDir = Path.GetDirectoryName(file);
            if (updateDir == null) return;

            UnZip(file); // Extract in-place
            File.Delete(file); // Delete archive
            InstallDirectory(installDir, updateDir); // Move all files across
        }

        private void InstallDirectory(string oldDirectory, string newDirectory)
        {
            foreach (var newDir in Directory.GetDirectories(newDirectory))
            {
                var oldDir = Path.Combine(oldDirectory, Path.GetFileName(newDir));
                if (Directory.Exists(oldDir)) InstallDirectory(oldDir, newDir);
            }
            foreach (var newFile in Directory.GetFiles(newDirectory))
            {
                var oldFile = Path.Combine(oldDirectory, Path.GetFileName(newFile));
                File.Copy(newFile, oldFile, true);
            }
        }

        private static bool UnZip(string file)
        {
            var folder = Path.GetDirectoryName(file);
            if (folder == null) return false;
            using (var fs = File.OpenRead(file))
            {
                using (var zf = new ZipFile(fs))
                {
                    foreach (ZipEntry entry in zf)
                    {
                        if (!entry.IsFile) continue; // Handle directories below

                        var outputFile = Path.Combine(folder, entry.Name);
                        var dir = Path.GetDirectoryName(outputFile);
                        if (dir == null) continue;
                        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                        using (var stream = zf.GetInputStream(entry))
                        {
                            using (var sw = File.Create(outputFile))
                            {
                                stream.CopyTo(sw);
                            }
                        }

                    }
                }
            }
            return true;
        }
    }
}
