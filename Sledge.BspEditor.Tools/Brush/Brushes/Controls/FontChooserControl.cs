using System;
using System.Drawing;
using System.Linq;

namespace Sledge.BspEditor.Tools.Brush.Brushes.Controls
{
    public partial class FontChooserControl : BrushControl
    {
        public string FontName
        {
            get => FontPicker.SelectedItem as string;
            set => FontPicker.SelectedItem = value;
        }

        public FontChooserControl(IBrush brush) : base(brush)
        {
            InitializeComponent();

            FontPicker.Items.Clear();
            FontPicker.Items.AddRange(FontFamily.Families.Select(x => x.Name).OfType<object>().ToArray());
            FontPicker.SelectedItem = GetFontFamily().Name;
        }

        public FontFamily GetFontFamily()
        {
            return FontFamily.Families.FirstOrDefault(x => x.Name == FontName) ?? FontFamily.GenericSansSerif;
        }

        private void ValueChanged(object sender, EventArgs e)
        {
            OnValuesChanged(Brush);
        }
    }
}
