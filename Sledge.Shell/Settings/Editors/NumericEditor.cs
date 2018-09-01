using System;
using System.Windows.Forms;
using Sledge.Common.Shell.Settings;

namespace Sledge.Shell.Settings.Editors
{
    public partial class NumericEditor : UserControl, ISettingEditor
    {
        private SettingKey _key;
        public event EventHandler<SettingKey> OnValueChanged;

        string ISettingEditor.Label
        {
            get => Label.Text;
            set => Label.Text = value;
        }

        public object Value
        {
            get => Numericbox.Value;
            set => Numericbox.Value = Math.Min(Numericbox.Maximum, Math.Max(Numericbox.Minimum, Convert.ToDecimal(value)));
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

        private void SetType(Type type)
        {
            if (type == null) type = typeof(decimal);
            Numericbox.DecimalPlaces = type == typeof(int) ? 0 : 2;
        }

        public NumericEditor()
        {
            InitializeComponent();
            Numericbox.ValueChanged += (o, e) => OnValueChanged?.Invoke(this, Key);
        }

        private void SetHint(string hint)
        {
            var spl = (hint ?? "").Split(',');
            if (spl.Length > 0 && decimal.TryParse(spl[0], out decimal min)) Numericbox.Minimum = min;
            if (spl.Length > 1 && decimal.TryParse(spl[1], out decimal max)) Numericbox.Maximum = max;
            if (spl.Length > 2 && decimal.TryParse(spl[2], out decimal step)) Numericbox.Increment = step;
        }
    }
}
