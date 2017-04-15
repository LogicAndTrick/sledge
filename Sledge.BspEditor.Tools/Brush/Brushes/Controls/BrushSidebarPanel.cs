using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Context;

namespace Sledge.BspEditor.Tools.Brush.Brushes.Controls
{
    [Export(typeof(ISidebarComponent))]
    public partial class BrushSidebarPanel : UserControl, ISidebarComponent
    {
        [ImportMany] private IEnumerable<Lazy<IBrush>> _brushes;

        private IBrush _selectedBrush;
        private readonly List<BrushControl> _currentControls;

        public string Title => "Brush";
        public object Control => this;

        public BrushSidebarPanel()
        {
            InitializeComponent();
            _currentControls = new List<BrushControl>();
        }

        public bool IsInContext(IContext context)
        {
            return context.TryGet("ActiveTool", out BrushTool _);
        }

        protected override void OnLoad(EventArgs e)
        {
            BrushTypeList.BeginUpdate();
            BrushTypeList.Items.Clear();
            foreach (var brush in _brushes)
            {
                BrushTypeList.Items.Add(new BrushWrapper(brush.Value));
            }
            BrushTypeList.SelectedIndex = 0;
            BrushTypeList.EndUpdate();

            _selectedBrush = _brushes.FirstOrDefault()?.Value;
            UpdateControls();
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
            
            OnChange();
        }

        private void OnChange()
        {
            Oy.Publish("Context:Add", new ContextInfo("BrushTool:ActiveBrush", _selectedBrush));
        }

        private void ControlValuesChanged(object sender, IBrush brush)
        {
            OnChange();
        }

        private void RoundCreatedVerticesChanged(object sender, EventArgs e)
        {
            OnChange();
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
