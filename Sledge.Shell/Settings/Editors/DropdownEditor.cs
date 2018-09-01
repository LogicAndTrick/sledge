using System;
using System.Linq;
using System.Windows.Forms;
using Sledge.Common.Shell.Settings;

namespace Sledge.Shell.Settings.Editors
{
    public partial class DropdownEditor : UserControl, ISettingEditor
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
            get => (Combobox.SelectedItem as DropdownValue)?.Value;
            set => Combobox.SelectedItem = Combobox.Items.OfType<DropdownValue>().FirstOrDefault(x => x.Value == Convert.ToString(value));
        }

        public object Control => this;

        public SettingKey Key
        {
            get => _key;
            set
            {
                _key = value;
                SetHint(value?.EditorHint);
            }
        }

        public DropdownEditor()
        {
            InitializeComponent();

            Combobox.DisplayMember = "Value";
            Combobox.ValueMember = "Value";

            Combobox.SelectedIndexChanged += (o, e) => OnValueChanged?.Invoke(this, Key);
        }

        private void SetHint(string hint)
        {
            var spl = (hint ?? "").Split(',');
            Combobox.Items.AddRange(spl.Select(x => new DropdownValue(x)).OfType<object>().ToArray());
        }

        private class DropdownValue
        {
            public string Value { get; }

            public DropdownValue(string value)
            {
                Value = value;
            }
        }
    }
}
