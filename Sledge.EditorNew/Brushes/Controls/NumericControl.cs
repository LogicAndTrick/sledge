using System;
using Sledge.Gui.Controls;
using Sledge.Gui.Interfaces;

namespace Sledge.EditorNew.Brushes.Controls
{
    public class NumericControl : BrushControl
    {
        public decimal Minimum
        {
            get { return Numeric.Minimum; }
            set { Numeric.Minimum = value; }
        }

        public decimal Maximum
        {
            get { return Numeric.Maximum; }
            set { Numeric.Maximum = value; }
        }

        public decimal Value
        {
            get { return Numeric.Value; }
            set { Numeric.Value = value; }
        }

        public string LabelTextKey
        {
            get { return Label.TextKey; }
            set { Label.TextKey = value; }
        }

        public bool ControlEnabled
        {
            get { return Numeric.Enabled; }
            set { Numeric.Enabled = value; }
        }

        public int Precision
        {
            get { return Numeric.Precision; }
            set { Numeric.Precision = value; }
        }

        public decimal Increment
        {
            get { return Numeric.Increment; }
            set { Numeric.Increment = value; }
        }

        public Label Label { get; set; }
        public NumericSpinner Numeric { get; set; }

        public NumericControl(IBrush brush) : base(brush)
        {
            Label = new Label();
            this.Add(Label);

            Numeric = new NumericSpinner{Minimum = 3, Maximum = 128, Value = 8, Precision = 0, Increment = 1};
            Numeric.ValueChanged += ValueChanged;
            this.Add(Numeric, true);
        }

        public decimal GetValue()
        {
            return Numeric.Value;
        }

        private void ValueChanged(object sender, EventArgs e)
        {
            OnValuesChanged(Brush);
        }
    }
}
