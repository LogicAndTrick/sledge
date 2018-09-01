using System;
using System.Drawing;
using System.Windows.Forms;
using Sledge.Common.Shell.Settings;

namespace Sledge.Shell.Settings.Editors
{
    public class DefaultSettingEditor : UserControl, ISettingEditor
    {
        public event EventHandler<SettingKey> OnValueChanged;
        public string Label { get; set; }

        public object Value
        {
            get => _box.Text;
            set => _box.Text = Convert.ToString(value);
        }

        public object Control => this;
        public SettingKey Key { get; set; }

        private readonly TextBox _box;

        public DefaultSettingEditor()
        {
            Size = new Size(400, 30);
            _box = new TextBox();
            Controls.Add(_box);
        }
    }
}
