using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Sledge.FileSystem
{
    public partial class FileSystemBrowserControl : UserControl
    {
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
            LocationTextbox.Text = File.FullPathName;
            FileList.Items.Clear();
            foreach (var child in File.GetChildren().OrderBy(x => x.Name.ToLower()))
            {
                FileList.Items.Add(new ListViewItem(child.Name, "_Folder"));
            }
            foreach (var file in File.GetFiles().Where(x => Matches(x.Name)).OrderBy(x => x.Name.ToLower()))
            {
                FileList.Items.Add(new ListViewItem(file.Name, GetIcon(file.FullPathName)));
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
                // File selected
            }
        }

        private void UpButtonClicked(object sender, EventArgs e)
        {
            var parent = File.Parent;
            if (parent != null) File = parent;
        }

        private void CancelButtonClicked(object sender, EventArgs e)
        {

        }

        private void OkButtonClicked(object sender, EventArgs e)
        {

        }
    }
}
