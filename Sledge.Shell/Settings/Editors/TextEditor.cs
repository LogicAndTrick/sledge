using System;
using System.Windows.Forms;
using Sledge.Common.Shell.Settings;

namespace Sledge.Shell.Settings.Editors
{
    public partial class TextEditor : UserControl, ISettingEditor
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
            get => Textbox.Text;
            set => Textbox.Text = Convert.ToString(value);
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

        private void SetHint(string hint)
        {
            switch (hint)
            {
                case "Directory":
                case "File":
                    BrowseButton.Visible = true;
                    Textbox.Width = BrowseButton.Left - Textbox.Left - 5;
                    break;
                default:
                    BrowseButton.Visible = false;
                    Textbox.Width = BrowseButton.Right - Textbox.Left;
                    break;
            }
        }

        public TextEditor()
        {
            InitializeComponent();
            Textbox.TextChanged += (o, e) => OnValueChanged?.Invoke(this, Key);
        }

        private void BrowseButtonClicked(object sender, EventArgs e)
        {
            if (Key.EditorHint == "Directory") BrowseDirectory();
            if (Key.EditorHint == "File") BrowseFile();
        }

        private void BrowseDirectory()
        {
            using (var bfd = new FolderBrowserDialog())
            {
                bfd.SelectedPath = Textbox.Text;
                if (bfd.ShowDialog() == DialogResult.OK) Textbox.Text = bfd.SelectedPath;
            }
        }

        private void BrowseFile()
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.FileName = Textbox.Text;
                if (ofd.ShowDialog() == DialogResult.OK) Textbox.Text = ofd.FileName;
            }
        }
    }
}
