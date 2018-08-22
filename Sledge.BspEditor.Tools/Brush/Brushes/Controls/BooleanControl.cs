using System;

namespace Sledge.BspEditor.Tools.Brush.Brushes.Controls
{
    public partial class BooleanControl : BrushControl
    {
        private bool _value;

        public bool Checked
        {
            get => _value;
            set => Checkbox.Checked = _value = value;
        }

        public string LabelText
        {
            get => Checkbox.Text;
            set => Checkbox.Text = value;
        }

        public bool ControlEnabled
        {
            get => Checkbox.Enabled;
            set => Checkbox.Enabled = value;
        }

        public BooleanControl(IBrush brush) : base(brush)
        {
            InitializeComponent();
            _value = Checkbox.Checked;
        }

        public bool GetValue()
        {
            return _value;
        }

        private void ValueChanged(object sender, EventArgs e)
        {
            _value = Checkbox.Checked;
            OnValuesChanged(Brush);
        }
    }
}
