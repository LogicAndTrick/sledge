using System;
using System.Drawing;
using System.Linq;
using Sledge.Gui.Controls;
using Sledge.Gui.Interfaces;
using Sledge.Gui.Models;

namespace Sledge.EditorNew.Brushes.Controls
{
    public partial class FontChooserControl : BrushControl
    {
        public string FontName
        {
            get { return FontPicker.SelectedItem == null ? null : FontPicker.SelectedItem.Value as string; }
            set { FontPicker.SelectedItem = FontPicker.Items.FirstOrDefault(x => x.Value == value); }
        }

        public ComboBox FontPicker { get; set; }

        public FontChooserControl(IBrush brush) : base(brush)
        {
            // todo label
            FontPicker = new ComboBox();
            FontPicker.SelectedItemChanged += ValueChanged;

            var families = FontFamily.Families.Select(x => new ComboBoxItem {Value = x.Name}).ToList();
            FontPicker.Items.Clear();
            FontPicker.Items.AddRange(families);
            FontPicker.SelectedItem = families.FirstOrDefault(x => x.Value == GetFontFamily().Name);
            
            this.Add(FontPicker, true);
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
