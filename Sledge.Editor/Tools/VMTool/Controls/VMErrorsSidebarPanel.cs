using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Sledge.Editor.Tools.VMTool.Controls
{
    public partial class VMErrorsSidebarPanel : UserControl
    {
        #region Events

        public delegate void SelectErrorEventHandler(object sender, VMError error);
        public delegate void FixErrorEventHandler(object sender, VMError error);
        public delegate void FixAllErrorsEventHandler(object sender);

        public event SelectErrorEventHandler SelectError;
        public event FixErrorEventHandler FixError;
        public event FixAllErrorsEventHandler FixAllErrors;

        protected virtual void OnSelectError(VMError error)
        {
            if (SelectError != null)
            {
                SelectError(this, error);
            }
        }

        protected virtual void OnFixError(VMError error)
        {
            if (FixError != null)
            {
                FixError(this, error);
            }
        }

        protected virtual void OnFixAllErrors()
        {
            if (FixAllErrors != null)
            {
                FixAllErrors(this);
            }
        }

        #endregion

        public VMErrorsSidebarPanel()
        {
            InitializeComponent();
        }

        public void SetErrorList(IEnumerable<VMError> errors)
        {
            ErrorList.Items.Clear();
            ErrorList.Items.AddRange(errors.OfType<object>().ToArray());
        }

        private void ErrorListSelectionChanged(object sender, EventArgs e)
        {
            var error = ErrorList.SelectedItem as VMError;
            OnSelectError(error);
        }
    }
}
