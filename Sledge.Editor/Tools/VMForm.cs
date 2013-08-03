using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Sledge.Common.Mediator;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Tools.VMTools;
using Sledge.Providers.Texture;
using Sledge.Editor.Editing;
using Sledge.Editor.UI;
using Sledge.Settings;

namespace Sledge.Editor.Tools
{
    public partial class VMForm : HotkeyForm
    {

        #region Events

        public delegate void ToolSelectedEventHandler(object sender, VMSubTool tool);

        public event ToolSelectedEventHandler ToolSelected;

        protected virtual void OnToolSelected(VMSubTool tool)
        {
            if (ToolSelected != null)
            {
                ToolSelected(this, tool);
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

        public void SelectionChanged()
        {

        }

        public void Clear()
        {
            
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            Focus();
            base.OnMouseEnter(e);
        }
    }
}
