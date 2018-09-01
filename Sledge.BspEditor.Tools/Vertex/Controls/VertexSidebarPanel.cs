using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Tools.Vertex.Tools;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Context;
using Sledge.Shell;

namespace Sledge.BspEditor.Tools.Vertex.Controls
{
    [Export(typeof(ISidebarComponent))]
    [OrderHint("F")]
    public partial class VertexSidebarPanel : UserControl, ISidebarComponent
    {
        [Import] private VertexTool _tool;
        [ImportMany] private IEnumerable<Lazy<VertexSubtool>> _subTools;

        public string Title => "Vertex Tool";
        public object Control => this;
        #region Translations
        
        public string DeselectAll { set => this.InvokeLater(() => DeselectAllButton.Text = value); }
        public string ResetToOriginal { set => this.InvokeLater(() => ResetButton.Text = value); }

        #endregion

        public VertexSidebarPanel()
        {
            InitializeComponent();

            Oy.Subscribe<VertexTool>("Tool:Activated", t =>
            {
                SetSelectedTool(t.CurrentSubTool?.GetType());
            });
            Oy.Subscribe<Type>("VertexTool:SubToolChanged", t =>
            {
                SetSelectedTool(t);
            });

            CreateHandle();
        }

        public bool IsInContext(IContext context)
        {
            return context.TryGet("ActiveTool", out VertexTool _);
        }

        public void SelectTool(Type tool)
        {
            Oy.Publish("VertexTool:SetSubTool", tool);
        }

        public void SetSelectedTool(Type tool)
        {
            AddToolButtons();

            foreach (RadioButton rb in ButtonLayoutPanel.Controls)
            {
                if (rb.Tag as Type != tool) rb.Checked = false;
                else if (!rb.Checked) rb.Checked = true;
            }

            ControlPanel.Controls.Clear();

            var t = _tool.CurrentSubTool;
            if (t == null)
            {
                ControlPanel.Text = "";
                return;
            }

            ControlPanel.Text = t.Title;
            if (t.Control != null)
            {
                ControlPanel.Controls.Add(t.Control);
                ControlPanel.Height = t.Control.PreferredSize.Height;
                t.Control.Dock = DockStyle.Top;
            }
        }
        
        public void AddToolButtons()
        {
            if (ButtonLayoutPanel.Controls.Count > 0) return;

            foreach (var subTool in _subTools.Select(x => x.Value).OrderBy(x => x.OrderHint))
            {
                AddSubTool(subTool);
            }
        }

        private void AddSubTool(VertexSubtool tool)
        {
            var rdo = new RadioButton
            {
                Name = tool.Title,
                Text = tool.Title,
                AutoSize = true,
                Margin = new Padding(1),
                Tag = tool.GetType()
            };
            rdo.Click += (sender, e) => SelectTool((sender as RadioButton)?.Tag as Type);
            if (ButtonLayoutPanel.Controls.Count == 0) rdo.Checked = true;
            ButtonLayoutPanel.Controls.Add(rdo);
            ButtonLayoutPanel.Size = ButtonLayoutPanel.GetPreferredSize(rdo.Size);
        }

        private void DeselectAllButtonClicked(object sender, EventArgs e)
        {
            Oy.Publish("VertexTool:DeselectAll", String.Empty);
        }

        private void ResetButtonClicked(object sender, EventArgs e)
        {
            Oy.Publish("VertexTool:Reset", String.Empty);
        }
    }
}
