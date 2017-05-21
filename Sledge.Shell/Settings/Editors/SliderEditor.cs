using System;
using System.Windows.Forms;
using Sledge.Common.Shell.Settings;

namespace Sledge.Shell.Settings.Editors
{
    public partial class SliderEditor : UserControl, ISettingEditor
    {
        private decimal _multiplier = 1;
        private SettingKey _key;
        public event EventHandler<SettingKey> OnValueChanged;

        string ISettingEditor.Label
        {
            get => Label.Text;
            set => Label.Text = value;
        }

        public object Value
        {
            get => Slider.Value / _multiplier;
            set
            {
                var v = Convert.ToDecimal(value);
                Slider.Value = (int) (v * _multiplier);
                NumericBox.Value = v;
            }
        }

        public object Control => this;

        public SettingKey Key
        {
            get => _key;
            set
            {
                _key = value;
                SetHint(value?.EditorHint);
                SetType(value?.Type);
            }
        }

        public SliderEditor()
        {
            InitializeComponent();
        }

        private void SetType(Type type)
        {
            if (type == null) type = typeof(decimal);
            NumericBox.DecimalPlaces = type == typeof(int) ? 0 : 2;
        }

        private void SetHint(string hint)
        {
            var spl = (hint ?? "").Split();
            if (spl.Length > 0 && int.TryParse(spl[0], out int min))
            {
                Slider.Minimum = min;
                NumericBox.Minimum = min / _multiplier;
            }
            if (spl.Length > 1 && int.TryParse(spl[1], out int max))
            {
                Slider.Maximum = max;
                NumericBox.Minimum = max / _multiplier;
            }
            if (spl.Length > 2 && int.TryParse(spl[2], out int step)) Slider.SmallChange = step;
            if (spl.Length > 3 && int.TryParse(spl[3], out int step2)) Slider.LargeChange = step2;
            if (spl.Length > 4 && int.TryParse(spl[3], out int mul)) _multiplier = mul;
        }

        private void NumberChanged(object sender, EventArgs e)
        {
            Slider.Value = (int) (NumericBox.Value * _multiplier);
            OnValueChanged?.Invoke(this, Key);
        }

        private void SliderChanged(object sender, EventArgs e)
        {
            NumericBox.Value = Slider.Value / _multiplier;
            OnValueChanged?.Invoke(this, Key);
        }
    }
}
