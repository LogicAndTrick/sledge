using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Sledge.Common.Mediator;
using Sledge.Editor.Tools.VMTools;
using Sledge.Editor.UI;
using Sledge.Settings;

namespace Sledge.Editor.Tools
{
    public partial class VMForm : HotkeyForm
    {

        #region Events

        public delegate void ToolSelectedEventHandler(object sender, VMSubTool tool);
        public delegate void DeselectAllEventHandler(object sender);
        public delegate void ResetEventHandler(object sender);
        public delegate void SelectErrorEventHandler(object sender, VMError error);
        public delegate void FixErrorEventHandler(object sender, VMError error);
        public delegate void FixAllErrorsEventHandler(object sender);

        public event ToolSelectedEventHandler ToolSelected;
        public event DeselectAllEventHandler DeselectAll;
        public event ResetEventHandler Reset;
        public event SelectErrorEventHandler SelectError;
        public event FixErrorEventHandler FixError;
        public event FixAllErrorsEventHandler FixAllErrors;

        protected virtual void OnToolSelected(VMSubTool tool)
        {
            if (ToolSelected != null)
            {
                ToolSelected(this, tool);
            }
        }

        protected virtual void OnDeselectAll()
        {
            if (DeselectAll != null)
            {
                DeselectAll(this);
            }
        }

        protected virtual void OnReset()
        {
            if (Reset != null)
            {
                Reset(this);
            }
        }

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

        private readonly List<VMSubTool> _tools;

        public Documents.Document Document { get; set; }

        public VMForm()
        {
            _tools = new List<VMSubTool>();
            InitializeComponent();
        }

        public void AddTool(VMSubTool tool)
        {
            var rdo = new RadioButton
            {
                Text = tool.GetName(),
                AutoSize = false,
                Size = new Size(110, 17)
            };
            rdo.Click += (sender, e) => SelectTool(tool);
            radioLayoutPanel.Controls.Add(rdo);
            if (!_tools.Any())
            {
                rdo.Checked = true;
                SelectTool(tool);
            }
            _tools.Add(tool);
        }

        public void SelectTool(VMSubTool tool)
        {
            ControlPanel.Text = tool.GetName();
            ControlPanel.Controls.Clear();
            if (tool.Control != null)
            {
                ControlPanel.Controls.Add(tool.Control);
                tool.Control.Dock = DockStyle.Fill;
            }
            OnToolSelected(tool);
        }

        public void SetSelectedTool(VMSubTool tool)
        {
            foreach (RadioButton rb in radioLayoutPanel.Controls)
            {
                if (rb.Text == tool.GetName())
                {
                    if (!rb.Checked) rb.Checked = true;
                    return;
                }
            }
        }

        public void SetErrorList(IEnumerable<VMError> errors)
        {
            ErrorList.Items.Clear();
            ErrorList.Items.AddRange(errors.OfType<object>().ToArray());
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            Focus();
            base.OnMouseEnter(e);
        }

        private void DeselectAllButtonClicked(object sender, EventArgs e)
        {
            OnDeselectAll();
        }

        private void ResetButtonClicked(object sender, EventArgs e)
        {
            OnReset();
        }

        private void FixErrorButtonClicked(object sender, EventArgs e)
        {

        }

        private void FixAllErrorsButtonClicked(object sender, EventArgs e)
        {
            OnFixAllErrors();
        }

        private void ErrorListSelectionChanged(object sender, EventArgs e)
        {
            var error = ErrorList.SelectedItem as VMError;
            OnSelectError(error);
        }

        private void OnClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Mediator.Publish(HotkeysMediator.SwitchTool, HotkeyTool.VM);
            }
        }
    }
}
