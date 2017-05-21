using System;
using System.Windows.Forms;
using Sledge.Common.Shell.Settings;

namespace Sledge.Shell.Settings.Editors
{
    public partial class TextEditor : UserControl, ISettingEditor
    {
        public event EventHandler<SettingKey> OnValueChanged;

        string ISettingEditor.Label
        {
            get => Label.Text;
            set => Label.Text = value;
        }

        public object Value
        {
            get => Textbox.Text;
            set => Textbox.Text = Convert.ToString(value);
        }

        public object Control => this;
        public SettingKey Key { get; set; }

        public TextEditor()
        {
            InitializeComponent();
            Textbox.TextChanged += (o, e) => OnValueChanged?.Invoke(this, Key);
        }
    }
}
