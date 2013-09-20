using System.Globalization;
using System.Windows.Forms;

namespace Sledge.Editor.UI.ObjectProperties
{
    internal class SmartEditInteger : SmartEditControl
    {
        private readonly NumericUpDown _numericUpDown;
        public SmartEditInteger()
        {
            _numericUpDown = new NumericUpDown { Width = 50, Minimum = short.MinValue, Maximum = short.MaxValue, Value = 0 };
            _numericUpDown.ValueChanged += (sender, e) => OnValueChanged();
            Controls.Add(_numericUpDown);
        }

        protected override string GetName()
        {
            return OriginalName;
        }

        protected override string GetValue()
        {
            return _numericUpDown.Value.ToString(CultureInfo.InvariantCulture);
        }

        protected override void OnSetProperty()
        {
            _numericUpDown.Text = PropertyValue;
        }
    }
}