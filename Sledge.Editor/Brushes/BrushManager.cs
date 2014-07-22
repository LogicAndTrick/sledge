using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Sledge.Editor.Brushes.Controls;
using Sledge.Editor.UI.Sidebar;

namespace Sledge.Editor.Brushes
{
    public static class BrushManager
    {
        public delegate void ValuesChangedEventHandler(IBrush brush);

        public static event ValuesChangedEventHandler ValuesChanged;

        private static void OnValuesChanged(IBrush brush)
        {
            if (ValuesChanged != null)
            {
                ValuesChanged(brush);
            }
        }

        public static IBrush CurrentBrush { get; private set; }

        private static readonly List<IBrush> Brushes;
        private static readonly List<BrushControl> CurrentControls;
        private static ComboBox _comboBox;
        private static bool _roundCreatedVertices;

        public static bool RoundCreatedVertices
        {
            get { return _roundCreatedVertices; }
            set
            {
                _roundCreatedVertices = value;
                if (CurrentBrush != null) OnValuesChanged(CurrentBrush);
            }
        }

        public static BrushSidebarPanel SidebarControl { get; private set; }

        static BrushManager()
        {
            Brushes = new List<IBrush>();
            CurrentControls = new List<BrushControl>();
            RoundCreatedVertices = true;
        }

        public static void Init()
        {
            Brushes.Add(new BlockBrush());
            Brushes.Add(new TetrahedronBrush());
            Brushes.Add(new PyramidBrush());
            Brushes.Add(new WedgeBrush());
            Brushes.Add(new CylinderBrush());
            Brushes.Add(new ConeBrush());
            Brushes.Add(new PipeBrush());
            Brushes.Add(new ArchBrush());
            Brushes.Add(new SphereBrush());
            Brushes.Add(new TorusBrush());
            Brushes.Add(new TextBrush());

            SetBrushControl(new BrushSidebarPanel());
        }

        private static ComboBox FindComboBox(Control parent)
        {
            if (parent is ComboBox) return (ComboBox) parent;
            return parent.Controls.OfType<Control>().Select(FindComboBox).FirstOrDefault(x => x != null);
        }

        public static void SetBrushControl(BrushSidebarPanel brushControl)
        {
            SidebarControl = brushControl;
            _comboBox = FindComboBox(SidebarControl);
            if (_comboBox != null)
            {
                _comboBox.SelectedIndexChanged += (sender, e) => UpdateSelectedBrush(Brushes[((ComboBox) sender).SelectedIndex]);
            }
            UpdateBrushControl();
        }

        private static void UpdateSelectedBrush(IBrush brush)
        {
            CurrentControls.ForEach(x => x.ValuesChanged -= ControlValuesChanged);
            CurrentControls.ForEach(x => SidebarControl.Controls.Remove(x));
            CurrentControls.Clear();
            CurrentBrush = brush;
            if (CurrentBrush == null) return;
            SidebarControl.RoundCheckboxEnabled = CurrentBrush.CanRound;
            CurrentControls.AddRange(CurrentBrush.GetControls().Reverse());
            for (var i = 0; i < CurrentControls.Count; i++)
            {
                var ctrl = CurrentControls[i];
                ctrl.Dock = DockStyle.Top;
                ctrl.ValuesChanged += ControlValuesChanged;
                SidebarControl.Controls.Add(ctrl);
                SidebarControl.Controls.SetChildIndex(ctrl, i);
            }
            //_brushControl.MinimumSize = new Size(0, _brushControl.Controls.OfType<Control>().Sum(x => x.Height + 6));
            OnValuesChanged(CurrentBrush);
        }

        private static void ControlValuesChanged(object sender, IBrush brush)
        {
            OnValuesChanged(brush);
        }

        public static void Register(IBrush brush)
        {
            Brushes.Add(brush);
            UpdateBrushControl();
        }

        private static void UpdateBrushControl()
        {
            if (SidebarControl == null || _comboBox == null) return;
            var sel = _comboBox.SelectedIndex;
            _comboBox.Items.Clear();
            _comboBox.Items.AddRange(Brushes.Select(x => x.Name).OfType<object>().ToArray());
            _comboBox.SelectedIndex = Math.Max(sel, 0);
        }
    }
}
