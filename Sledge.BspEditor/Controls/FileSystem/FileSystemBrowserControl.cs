using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Sledge.BspEditor.Properties;
using Sledge.FileSystem;

namespace Sledge.BspEditor.Controls.FileSystem
{
    public partial class FileSystemBrowserControl : UserControl
    {
        #region Events

        public delegate void ConfirmedButtonEventHandler(object sender, IEnumerable<IFile> selection);
        public delegate void CancelButtonEventHandler(object sender);

        public event ConfirmedButtonEventHandler Confirmed;
        public event CancelButtonEventHandler Cancelled;

        protected virtual void OnConfirmed(IEnumerable<IFile> files)
        {
            Confirmed?.Invoke(this, files);
        }

        protected virtual void OnCancelled()
        {
            Cancelled?.Invoke(this);
        }

        private const string IconDirectory = "Directory";
        private const string IconGeneric = "Generic";

        #endregion Events

        private IFile _file;
        private string _filter;
        private readonly List<Regex> _regexes; 

        public FileSystemBrowserControl()
        {
            _regexes = new List<Regex>();
            InitializeComponent();
            FileImages.Images.Add(IconDirectory, Resources.File_Folder);
            FileImages.Images.Add(IconGeneric, Resources.File_Generic);
            FileImages.Images.Add("mdl", Resources.File_Mdl);
            FileImages.Images.Add("mp3", Resources.File_Mp3);
            FileImages.Images.Add("txt", Resources.File_Txt);
            FileImages.Images.Add("wav", Resources.File_Wav);
        }

        public IFile File
        {
            get => _file;
            set { 
                _file = value;
                UpdateFile();
            }
        }

        public string FilterText
        {
            get => FilterLabel.Text == "(none)" ? "" : FilterLabel.Text;
            set => FilterLabel.Text = value == "" ? "(none)" : value;
        }

        public string Filter
        {
            get => _filter;
            set {
                _filter = value;
                _regexes.Clear();
                _regexes.AddRange((_filter ?? "").Split(',').Select(f => new Regex("^" + Regex.Escape(f).Replace("\\*", ".*") + "$", RegexOptions.IgnoreCase | RegexOptions.Compiled)));
                UpdateFile();
            }
        }

        private string GetIcon(string file)
        {
            var ext = (Path.GetExtension(file) ?? "").Trim('.').ToLower();
            return FileImages.Images.ContainsKey(ext) ? ext : IconGeneric;
        }

        private void UpdateFile()
        {
            FileList.Items.Clear();
            if (File == null) return;
            LocationTextbox.Text = File.FullPathName;
            foreach (var child in File.GetChildren().OrderBy(x => x.Name.ToLower()))
            {
                FileList.Items.Add(new ListViewItem(child.Name, IconDirectory) {Tag = child});
            }
            foreach (var file in File.GetFiles().Where(x => Matches(x.Name)).OrderBy(x => x.Name.ToLower()))
            {
                FileList.Items.Add(new ListViewItem(file.Name, GetIcon(file.FullPathName)) {Tag = file});
            }
        }

        private bool Matches(string name)
        {
            return string.IsNullOrEmpty(Filter) || _regexes.Any(x => x.IsMatch(name));
        }

        private void FileListDoubleClicked(object sender, EventArgs e)
        {
            if (FileList.SelectedIndices.Count != 1) return;
            if (FileList.SelectedItems[0].ImageKey == IconDirectory)
            {
                File = File.GetChild(FileList.SelectedItems[0].Text);
            }
            else
            {
                OnConfirmed(FileList.SelectedItems.OfType<ListViewItem>().Select(x => x.Tag).OfType<IFile>());
            }
        }

        private void UpdateSelection(object sender, EventArgs e)
        {
            var str = "";
            foreach (ListViewItem si in FileList.SelectedItems)
            {
                str += si.Text + "; ";
            }
            SelectionTextbox.Text = str;
        }

        private void UpButtonClicked(object sender, EventArgs e)
        {
            var parent = File.Parent;
            if (parent != null) File = parent;
        }

        private void CancelButtonClicked(object sender, EventArgs e)
        {
            OnCancelled();
        }

        private void OkButtonClicked(object sender, EventArgs e)
        {
            OnConfirmed(FileList.SelectedItems.OfType<ListViewItem>().Select(x => x.Tag).OfType<IFile>());
        }
    }
}
