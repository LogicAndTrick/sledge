using System.Drawing;
using System.Windows.Forms;

namespace Sledge.QuickForms.Items
{
    /// <summary>
    /// A control that shows a NumericUpDown control.
    /// </summary>
    public class QuickFormNumericUpDown : QuickFormItem
    {
        public override object Value => _numericUpDown.Value;

        private readonly Label _label;
        private readonly NumericUpDown _numericUpDown;

        public QuickFormNumericUpDown(string text, int min, int max, int numDecimals, decimal value)
        {
            _label = new Label
            {
                Text = text,
                AutoSize = true,
                MinimumSize = new Size(LabelWidth, 0),
                MaximumSize = new Size(LabelWidth, 1000),
                TextAlign = ContentAlignment.MiddleRight,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom
            };
            _numericUpDown = new NumericUpDown
            {
                Maximum = max,
                Minimum = min,
                DecimalPlaces = numDecimals,
                Name = Name,
                Anchor = AnchorStyles.Right,
                Increment = (numDecimals > 0) ? (1m / (numDecimals * 10m)) : (1),
                Width = 80,
                Value = value
            };

            Controls.Add(_label);
            Controls.Add(_numericUpDown);
        }
    }
}
