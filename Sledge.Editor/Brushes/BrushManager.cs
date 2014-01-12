using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Sledge.Editor.Brushes.Controls;

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
        private static Control _brushControl;
        private static ComboBox _comboBox;

        static BrushManager()
        {
            Brushes = new List<IBrush>();
            CurrentControls = new List<BrushControl>();
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
        }

        public static void SetBrushControl(Control brushControl)
        {
            _brushControl = brushControl;
            _comboBox = _brushControl.Controls.OfType<ComboBox>().FirstOrDefault();
            if (_comboBox != null)
            {
                _comboBox.SelectedIndexChanged += (sender, e) => UpdateSelectedBrush(Brushes[((ComboBox) sender).SelectedIndex]);
            }
            UpdateBrushControl();
        }

        private static void UpdateSelectedBrush(IBrush brush)
        {
            CurrentControls.ForEach(x => x.ValuesChanged -= ControlValuesChanged);
            CurrentControls.ForEach(x => _brushControl.Controls.Remove(x));
            CurrentControls.Clear();
            CurrentBrush = brush;
            if (CurrentBrush == null) return;
            CurrentControls.AddRange(CurrentBrush.GetControls());
            CurrentControls.ForEach(x => x.ValuesChanged += ControlValuesChanged);
            CurrentControls.ForEach(x => x.Width = _brushControl.Width);
            CurrentControls.ForEach(x => _brushControl.Controls.Add(x));
            _brushControl.Height = _brushControl.Controls.OfType<Control>().Sum(x => x.Height + 6);
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
            if (_brushControl == null || _comboBox == null) return;
            var sel = _comboBox.SelectedIndex;
            _comboBox.Items.Clear();
            _comboBox.Items.AddRange(Brushes.Select(x => x.Name).OfType<object>().ToArray());
            _comboBox.SelectedIndex = Math.Max(sel, 0);
        }
    }
}
