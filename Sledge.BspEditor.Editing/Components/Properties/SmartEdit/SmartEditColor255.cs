using System;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Editing.Properties;
using Sledge.DataStructures.GameData;

namespace Sledge.BspEditor.Editing.Components.Properties.SmartEdit
{
    [Export(typeof(IObjectPropertyEditor))]
    public class SmartEditColor255 : SmartEditControl
    {
        private readonly TextBox _textBox;
        public SmartEditColor255()
        {
            _textBox = new TextBox { Width = 200 };
            _textBox.TextChanged += (sender, e) => OnValueChanged();
            Controls.Add(_textBox);

            var btn = new Button { Image = Resources.Button_ColourPicker, Text = "", Margin = new Padding(1), Width = 24, Height = 24 };
            btn.Click += OpenColorPicker;
            Controls.Add(btn);
        }

        public override string PriorityHint => "H";

        public override bool SupportsType(VariableType type)
        {
            return type == VariableType.Color255;
        }

        private void OpenColorPicker(object sender, EventArgs e)
        {
            var spl = _textBox.Text.Split(' ');
            int r = 0, g = 0, b = 0;
            if (spl.Length >= 3)
            {
                int.TryParse(spl[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out r);
                int.TryParse(spl[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out g);
                int.TryParse(spl[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out b);
            }
            using (var cd = new ColorDialog { Color = Color.FromArgb(r, g, b) })
            {
                if (cd.ShowDialog() == DialogResult.OK)
                {
                    r = cd.Color.R;
                    g = cd.Color.G;
                    b = cd.Color.B;
                    if (spl.Length < 3) spl = new string[3];
                    spl[0] = r.ToString(CultureInfo.InvariantCulture);
                    spl[1] = g.ToString(CultureInfo.InvariantCulture);
                    spl[2] = b.ToString(CultureInfo.InvariantCulture);
                    _textBox.Text = String.Join(" ", spl);
                }
            }
        }

        protected override string GetName()
        {
            return OriginalName;
        }

        protected override string GetValue()
        {
            return _textBox.Text;
        }

        protected override void OnSetProperty(MapDocument document)
        {
            _textBox.Text = PropertyValue;
        }
    }
}