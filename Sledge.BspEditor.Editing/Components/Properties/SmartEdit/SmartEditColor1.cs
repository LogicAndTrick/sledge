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
    public class SmartEditColor1 : SmartEditControl
    {
        private readonly TextBox _textBox;
        public SmartEditColor1()
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
            return type == VariableType.Color1;
        }

        private void OpenColorPicker(object sender, EventArgs e)
        {
            var spl = _textBox.Text.Split(' ');
            float r = 0, g = 0, b = 0;
            if (spl.Length >= 3)
            {
                float.TryParse(spl[0], NumberStyles.Float, CultureInfo.InvariantCulture, out r);
                float.TryParse(spl[1], NumberStyles.Float, CultureInfo.InvariantCulture, out g);
                float.TryParse(spl[2], NumberStyles.Float, CultureInfo.InvariantCulture, out b);
            }
            r *= 255;
            g *= 255;
            b *= 255;
            using (var cd = new ColorDialog { Color = Color.FromArgb((int)r, (int)g, (int)b) })
            {
                if (cd.ShowDialog() == DialogResult.OK)
                {
                    r = cd.Color.R / 255f;
                    g = cd.Color.G / 255f;
                    b = cd.Color.B / 255f;
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