using System;

namespace Sledge.BspEditor.Tools.Brush.Brushes.Controls
{
    public partial class BooleanControl : BrushControl
    {
        public bool Checked
        {
            get => Checkbox.Checked;
            set => Checkbox.Checked = value;
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
        }

        public bool GetValue()
        {
            return Checkbox.Checked;
        }

        private void ValueChanged(object sender, EventArgs e)
        {
            OnValuesChanged(Brush);
        }
    }
}
