using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Sledge.Common.Logging;
using Sledge.FileSystem;

namespace Sledge.BspEditor.Controls.FileSystem
{
    public partial class FileSystemBrowserDialog : Form
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

        #endregion Events

        public List<IFile> SelectedFiles { get; private set; }

        public string Filter
        {
            get => Browser.Filter;
            set => Browser.Filter = value;
        }

        public string FilterText
        {
            get => Browser.FilterText;
            set => Browser.FilterText = value;
        }

        public FileSystemBrowserDialog(IFile root)
        {
            InitializeComponent();
            Browser.Cancelled += Cancel;
            Browser.Confirmed += Confirm;

            try
            {
                // DO NOT REMOVE THIS TRY/CATCH BLOCK!
                // Without it, the CLR crashes, even if no exception is thrown! I have no idea why this is the case.
                Browser.File = root;
            }
            catch (Exception ex)
            {
                Log.Error(nameof(FileSystemBrowserDialog), "Exception when mounting file root", ex);
            }
        }

        private void Confirm(object sender, IEnumerable<IFile> selection)
        {
            SelectedFiles = selection.ToList();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void Cancel(object sender)
        {
            SelectedFiles = new List<IFile>();
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
