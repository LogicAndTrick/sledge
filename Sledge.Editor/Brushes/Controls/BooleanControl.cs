using System;

namespace Sledge.Editor.Brushes.Controls
{
    public partial class BooleanControl : BrushControl
    {
        public bool Checked
        {
            get { return Checkbox.Checked; }
            set { Checkbox.Checked = value; }
        }

        public string LabelText
        {
            get { return Checkbox.Text; }
            set { Checkbox.Text = value; }
        }

        public bool ControlEnabled
        {
            get { return Checkbox.Enabled; }
            set { Checkbox.Enabled = value; }
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
