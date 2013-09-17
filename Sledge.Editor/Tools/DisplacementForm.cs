using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Sledge.Common.Mediator;
using Sledge.Editor.Tools.DisplacementTools;
using Sledge.Editor.UI;
using Sledge.Settings;

namespace Sledge.Editor.Tools
{
    public partial class DisplacementForm : HotkeyForm
    {

        #region Events

        public delegate void ToolSelectedEventHandler(object sender, DisplacementSubTool tool);

        public event ToolSelectedEventHandler ToolSelected;

        protected virtual void OnToolSelected(DisplacementSubTool tool)
        {
            if (ToolSelected != null)
            {
                ToolSelected(this, tool);
            }
        }

        #endregion

        private readonly List<DisplacementSubTool> _tools;

        public DisplacementForm()
        {
            _tools = new List<DisplacementSubTool>();
            InitializeComponent();
        }

        public void AddTool(DisplacementSubTool tool)
        {
            var rdo = new RadioButton
                          {
                              Text = tool.GetName(), 
                              AutoSize = false, 
                              Size = new Size(110, 17)
                          };
            rdo.Click += (sender, e) => SelectTool(tool);
            radioLayoutPanel.Controls.Add(rdo);
            if (!_tools.Any()) rdo.Checked = true;
            _tools.Add(tool);
        }

        public void SelectTool(DisplacementSubTool tool)
        {
            ControlPanel.Controls.Clear();
            if (tool.Control != null)
            {
                ControlPanel.Controls.Add(tool.Control);
                tool.Control.Dock = DockStyle.Fill;
            }
            OnToolSelected(tool);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            Focus();
            base.OnMouseEnter(e);
        }

        private void OnClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Mediator.Publish(HotkeysMediator.SwitchTool, HotkeyTool.Displacement);
            }
        }
    }
}
