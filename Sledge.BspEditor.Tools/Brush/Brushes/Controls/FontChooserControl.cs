using System;
using System.Drawing;
using System.Linq;

namespace Sledge.BspEditor.Tools.Brush.Brushes.Controls
{
    public partial class FontChooserControl : BrushControl
    {
        private string _value;

        public string FontName
        {
            get => FontPicker.SelectedItem as string;
            set => FontPicker.SelectedItem = _value = value;
        }

        public string LabelText
        {
            get => Label.Text;
            set => Label.Text = value;
        }

        public FontChooserControl(IBrush brush) : base(brush)
        {
            InitializeComponent();

            FontPicker.Items.Clear();
            FontPicker.Items.AddRange(FontFamily.Families.Select(x => x.Name).OfType<object>().ToArray());
            FontPicker.SelectedItem = _value = GetFontFamily().Name;
        }

        public FontFamily GetFontFamily()
        {
            return FontFamily.Families.FirstOrDefault(x => x.Name == _value) ?? FontFamily.GenericSansSerif;
        }

        private void ValueChanged(object sender, EventArgs e)
        {
            _value = FontPicker.SelectedItem as string;
            OnValuesChanged(Brush);
        }
    }
}
