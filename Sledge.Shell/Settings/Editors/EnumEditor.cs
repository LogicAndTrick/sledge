using System;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Sledge.Common.Shell.Settings;

namespace Sledge.Shell.Settings.Editors
{
    public partial class EnumEditor : UserControl, ISettingEditor
    {
        private readonly Type _enumType;
        public event EventHandler<SettingKey> OnValueChanged;

        string ISettingEditor.Label
        {
            get => Label.Text;
            set => Label.Text = value;
        }

        public object Value
        {
            get => Combobox.SelectedValue;
            set => Combobox.SelectedItem = Combobox.Items.OfType<EnumValue>().FirstOrDefault(x => x.Value == Convert.ToString(value));
        }

        public object Control => this;
        public SettingKey Key { get; set; }

        public EnumEditor(Type enumType)
        {
            _enumType = enumType;
            InitializeComponent();

            Combobox.DisplayMember = "Label";
            Combobox.ValueMember = "Value";

            var values = enumType.GetFields(BindingFlags.Static | BindingFlags.Public);
            Combobox.Items.AddRange(values.Select(x => new EnumValue(x)).OfType<object>().ToArray());

            Combobox.SelectedIndexChanged += (o, e) => OnValueChanged?.Invoke(this, Key);
        }

        private class EnumValue
        {
            public string Label { get; set; }
            public string Value { get; set; }
            public EnumValue(FieldInfo field)
            {
                Label = field.Name;
                Value = field.Name;
            }
        }
    }
}
