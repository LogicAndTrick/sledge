using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Sledge.Common.Shell.Settings;

namespace Sledge.Shell.Settings.Editors
{
    public partial class ColorEditor : UserControl, ISettingEditor
    {
        public event EventHandler<SettingKey> OnValueChanged;

        string ISettingEditor.Label
        {
            get => Label.Text;
            set => Label.Text = value;
        }

        public object Value
        {
            get => ColorPanel.BackColor;
            set => ColorPanel.BackColor = (Color) value;
        }

        public object Control => this;
        public SettingKey Key { get; set; }

        public ColorEditor()
        {
            InitializeComponent();
        }
        public void SetHint(string hint)
        {
            //
        }

        private void PickColor(object sender, EventArgs e)
        {
            using (var cp = new ColorDialog {Color = ColorPanel.BackColor, SolidColorOnly = true})
            {
                if (cp.ShowDialog() == DialogResult.OK)
                {
                    ColorPanel.BackColor = cp.Color;
                    HexBox.Text = ColorTranslator.ToHtml(cp.Color).Substring(1);
                    OnValueChanged?.Invoke(this, Key);
                }
            }
        }

        private void UpdateHex(object sender, EventArgs e)
        {
            // Match #000 and #000000
            if (Regex.IsMatch(HexBox.Text, "^([0-9A-Fa-f]{3}){1,2}$"))
            {
                var color = ColorTranslator.FromHtml('#' + HexBox.Text);
                ColorPanel.BackColor = color;
                OnValueChanged?.Invoke(this, Key);
            }
        }

        private void HexUnfocused(object sender, EventArgs e)
        {
            if (!Regex.IsMatch(HexBox.Text, "^[0-9A-Fa-f]{6}$"))
            {
                HexBox.Text = ColorTranslator.ToHtml(ColorPanel.BackColor).Substring(1);
            }
        }
    }
}
