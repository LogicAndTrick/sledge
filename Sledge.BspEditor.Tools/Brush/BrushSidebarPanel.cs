using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Tools.Brush.Brushes.Controls;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Hooks;
using Sledge.Common.Translations;
using Sledge.Shell;

namespace Sledge.BspEditor.Tools.Brush
{
    [Export(typeof(ISidebarComponent))]
    [Export(typeof(IInitialiseHook))]
    [OrderHint("F")]
    [AutoTranslate]
    public partial class BrushSidebarPanel : UserControl, ISidebarComponent, IInitialiseHook
    {
        [ImportMany] private IEnumerable<Lazy<IBrush>> _brushes;
        [Import] private BrushTool _tool;

        private IBrush _selectedBrush;
        private readonly List<BrushControl> _currentControls;

        public string Title { get; set; } = "Brush";
        public object Control => this;
        
        #region Translations

        public string BrushType { set => this.InvokeLater(() => BrushTypeLabel.Text = value); }
        public string RoundVertices { set => this.InvokeLater(() => RoundCreatedVerticesCheckbox.Text = value); }

        #endregion

        public async Task OnInitialise()
        {
            _selectedBrush = null;
            this.InvokeLater(() =>
            {
                BrushTypeList.BeginUpdate();
                BrushTypeList.Items.Clear();
                foreach (var brush in _brushes.OrderBy(x => OrderHintAttribute.GetOrderHint(x.Value.GetType())))
                {
                    if (_selectedBrush == null) _selectedBrush = brush.Value;
                    BrushTypeList.Items.Add(new BrushWrapper(brush.Value));
                }

                BrushTypeList.SelectedIndex = 0;
                BrushTypeList.EndUpdate();

                UpdateControls();
            });
        }

        public BrushSidebarPanel()
        {
            InitializeComponent();
            CreateHandle();

            _currentControls = new List<BrushControl>();

            Oy.Subscribe<BrushTool>("BrushTool:ResetBrushType", ResetBrushType);
        }

        private async Task ResetBrushType(BrushTool bt)
        {
            this.InvokeLater(() =>
            {
                if (BrushTypeList.Items.Count > 0)
                {
                    BrushTypeList.SelectedIndex = 0;
                    UpdateControls();
                }
            });
        }

        public bool IsInContext(IContext context)
        {
            return context.TryGet("ActiveTool", out BrushTool _);
        }

        private void UpdateControls()
        {
            _currentControls.ForEach(x => x.ValuesChanged -= ControlValuesChanged);
            _currentControls.ForEach(x => Controls.Remove(x));
            _currentControls.Clear();

            if (_selectedBrush == null) return;

            RoundCreatedVerticesCheckbox.Enabled = _selectedBrush.CanRound;

            _currentControls.AddRange(_selectedBrush.GetControls().Reverse());
            for (var i = 0; i < _currentControls.Count; i++)
            {
                var ctrl = _currentControls[i];
                ctrl.Dock = DockStyle.Top;
                ctrl.ValuesChanged += ControlValuesChanged;
                Controls.Add(ctrl);
                Controls.SetChildIndex(ctrl, i);
            }

            OnBrushChange();
        }

        private void OnBrushChange()
        {
            Oy.Publish("Context:Add", new ContextInfo("BrushTool:ActiveBrush", _selectedBrush));
        }

        private void OnValuesChange()
        {
            Oy.Publish("BrushTool:ValuesChanged", new object());
        }

        private void ControlValuesChanged(object sender, IBrush brush)
        {
            OnValuesChange();
        }

        private void RoundCreatedVerticesChanged(object sender, EventArgs e)
        {
            _tool.RoundVertices = RoundCreatedVerticesCheckbox.Checked;
            OnValuesChange();
        }

        private void SelectedBrushTypeChanged(object sender, EventArgs e)
        {
            _selectedBrush = (BrushTypeList.SelectedItem as BrushWrapper)?.Brush;
            UpdateControls();
        }

        private class BrushWrapper
        {
            public IBrush Brush { get; set; }

            public BrushWrapper(IBrush brush)
            {
                Brush = brush;
            }

            public override string ToString()
            {
                return Brush.Name;
            }
        }
    }
}
