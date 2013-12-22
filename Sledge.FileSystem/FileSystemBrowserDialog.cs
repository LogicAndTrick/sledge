using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sledge.FileSystem
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

        public List<IFile> SelectedFiles { get; private set; }

        public string Filter
        {
            get
            {
                return Browser.Filter;
            }
            set
            {
                Browser.Filter = value;
            }
        }

        public string FilterText
        {
            get
            {
                return Browser.FilterText;
            }
            set
            {
                Browser.FilterText = value;
            }
        }

        public FileSystemBrowserDialog(IFile root)
        {
            InitializeComponent();
            Browser.Cancelled += Cancel;
            Browser.Confirmed += Confirm;

            Browser.File = root;
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
