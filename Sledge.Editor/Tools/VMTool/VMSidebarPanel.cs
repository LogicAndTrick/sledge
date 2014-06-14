using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sledge.Common.Mediator;
using Sledge.Editor.Brushes;

namespace Sledge.Editor.Tools.VMTool
{
    public partial class VMSidebarPanel : UserControl, IMediatorListener
    {
        #region Events

        public delegate void ToolSelectedEventHandler(object sender, VMSubTool tool);
        public delegate void DeselectAllEventHandler(object sender);
        public delegate void ResetEventHandler(object sender);

        public event ToolSelectedEventHandler ToolSelected;
        public event DeselectAllEventHandler DeselectAll;
        public event ResetEventHandler Reset;

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

        #endregion

        private readonly List<VMSubTool> _tools;
        public Documents.Document Document { get; set; }

        public VMSidebarPanel()
        {
            InitializeComponent();
            _tools = new List<VMSubTool>();
        }

        public void AddTool(VMSubTool tool)
        {
            var rdo = new RadioButton
            {
                Name = tool.GetName(),
                Text = tool.GetName(),
                //Appearance = Appearance.Button,
                AutoSize = true,
                //Size = new Size(110, 17)
            };
            rdo.Click += (sender, e) => SelectTool(tool);
            ButtonLayoutPanel.Controls.Add(rdo);
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
                //ControlPanel.Height = tool.Control.PreferredSize.Height;
                tool.Control.Dock = DockStyle.Top;
            }
            OnToolSelected(tool);
        }

        public void SetSelectedTool(VMSubTool tool)
        {
            foreach (RadioButton rb in ButtonLayoutPanel.Controls)
            {
                if (rb.Name != tool.GetName()) rb.Checked = false;
                else if (!rb.Checked) rb.Checked = true;
            }
        }

        private void DeselectAllButtonClicked(object sender, EventArgs e)
        {
            OnDeselectAll();
        }

        private void ResetButtonClicked(object sender, EventArgs e)
        {
            OnReset();
        }

        public void Notify(string message, object data)
        {
            Mediator.ExecuteDefault(this, message, data);
        }
    }
}
