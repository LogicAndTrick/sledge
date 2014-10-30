using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sledge.Gui.Attributes;
using Sledge.Gui.Interfaces.Dialogs;

namespace Sledge.Gui.WinForms.Dialogs
{
    [ControlImplementation("WinForms")]
    public class WinFormsFileOpenDialog : IFileOpenDialog
    {
        private readonly OpenFileDialog _dialog;

        public WinFormsFileOpenDialog()
        {
            _dialog = new OpenFileDialog();
        }

        public bool Prompt()
        {
            return _dialog.ShowDialog() == DialogResult.OK;
        }

        public bool Multiple
        {
            get { return _dialog.Multiselect; }
            set { _dialog.Multiselect = value; }
        }

        public string Filter
        {
            get { return _dialog.Filter; }
            set { _dialog.Filter = value; }
        }

        public string File
        {
            get { return _dialog.FileName; }
            set { _dialog.FileName = value; }
        }

        public IEnumerable<string> Files
        {
            get { return _dialog.FileNames; }
        }

        public void Dispose()
        {
            _dialog.Dispose();
        }
    }
}
