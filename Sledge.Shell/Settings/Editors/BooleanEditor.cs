using System;
using System.Windows.Forms;
using Sledge.Common.Shell.Settings;

namespace Sledge.Shell.Settings.Editors
{
    public partial class BooleanEditor : UserControl, ISettingEditor
    {
        public event EventHandler<SettingKey> OnValueChanged;

        string ISettingEditor.Label
        {
            get => Checkbox.Text;
            set => Checkbox.Text = value;
        }

        public object Value
        {
            get => Checkbox.Checked;
            set => Checkbox.Checked = Convert.ToBoolean(value);
        }

        public object Control => this;
        public SettingKey Key { get; set; }

        public BooleanEditor()
        {
            InitializeComponent();

            Checkbox.CheckedChanged += (o, e) => OnValueChanged?.Invoke(this, Key);
        }
    }
}
