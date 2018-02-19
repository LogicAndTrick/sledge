using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Sledge.BspEditor.Tools.Vertex.Controls
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
            SelectError?.Invoke(this, error);
        }

        protected virtual void OnFixError(VMError error)
        {
            FixError?.Invoke(this, error);
        }

        protected virtual void OnFixAllErrors()
        {
            FixAllErrors?.Invoke(this);
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
