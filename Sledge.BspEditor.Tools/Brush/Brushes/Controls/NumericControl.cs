using System;

namespace Sledge.BspEditor.Tools.Brush.Brushes.Controls
{
    public partial class NumericControl : BrushControl
    {
        private decimal _value;

        public decimal Minimum
        {
            get => Numeric.Minimum;
            set => Numeric.Minimum = value;
        }

        public decimal Maximum
        {
            get => Numeric.Maximum;
            set => Numeric.Maximum = value;
        }

        public decimal Value
        {
            get => Numeric.Value;
            set => Numeric.Value = _value = value;
        }

        public string LabelText
        {
            get => Label.Text;
            set => Label.Text = value;
        }

        public bool ControlEnabled
        {
            get => Numeric.Enabled;
            set => Numeric.Enabled = value;
        }

        public int Precision
        {
            get => Numeric.DecimalPlaces;
            set => Numeric.DecimalPlaces = value;
        }

        public decimal Increment
        {
            get => Numeric.Increment;
            set => Numeric.Increment = value;
        }

        public NumericControl(IBrush brush) : base(brush)
        {
            InitializeComponent();
            _value = Numeric.Value;
        }

        public decimal GetValue()
        {
            return _value;
        }

        private void ValueChanged(object sender, EventArgs e)
        {
            _value = Numeric.Value;
            OnValuesChanged(Brush);
        }
    }
}
