using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Sledge.FileSystem;

namespace Sledge.Editor.UI.FileSystem
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
            if (Confirmed != null)
            {
                Confirmed(this, files);
            }
        }

        protected virtual void OnCancelled()
        {
            if (Cancelled != null)
            {
                Cancelled(this);
            }
        }

        #endregion Events

        private IFile _file;
        private string _filter;
        private readonly List<Regex> _regexes; 

        public FileSystemBrowserControl()
        {
            _regexes = new List<Regex>();
            InitializeComponent();
            FileImages.Images.Add("_Folder", FileSystemIcons.GetFolderIcon(FileSystemIcons.SystemIconSize.Small, FileSystemIcons.SystemFolderType.Open));
            FileImages.Images.Add("_Unknown", FileSystemIcons.IconFromExtension(".unknown", FileSystemIcons.SystemIconSize.Small));
        }

        public IFile File
        {
            get { return _file; }
            set { 
                _file = value;
                UpdateFile();
            }
        }

        public string FilterText
        {
            get { return FilterLabel.Text == "(none)" ? "" : FilterLabel.Text; }
            set { FilterLabel.Text = value == "" ? "(none)" : value; }
        }

        public string Filter
        {
            get { return _filter; }
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
            if (ext == "") return "_Unknown";
            if (!FileImages.Images.ContainsKey(ext))
            {
                FileImages.Images.Add(ext, FileSystemIcons.IconFromExtension(ext, FileSystemIcons.SystemIconSize.Small));
            }
            return ext;
        }

        private void UpdateFile()
        {
            FileList.Items.Clear();
            if (File == null) return;
            LocationTextbox.Text = File.FullPathName;
            foreach (var child in File.GetChildren().OrderBy(x => x.Name.ToLower()))
            {
                FileList.Items.Add(new ListViewItem(child.Name, "_Folder") {Tag = child});
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
            if (FileList.SelectedItems[0].ImageKey == "_Folder")
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
