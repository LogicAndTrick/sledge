using System;
using Sledge.Gui.Controls;
using Sledge.Gui.Interfaces;

namespace Sledge.EditorNew.Brushes.Controls
{
    public class TextControl : BrushControl
    {
        public string EnteredText
        {
            get { return TextBox.Text; }
            set { TextBox.Text = value; }
        }

        public Label Label { get; set; }
        public TextBox TextBox { get; set; }

        public TextControl(IBrush brush) : base(brush)
        {
            TextBox = new TextBox();
            TextBox.TextChanged += ValueChanged;
            // TextBox.Tag = Hotkeys.SuppressHotkeysTag;
            // todo label
            this.Add(TextBox, true);
        }

        public string GetValue()
        {
            return TextBox.Text;
        }

        private void ValueChanged(object sender, EventArgs e)
        {
            OnValuesChanged(Brush);
        }
    }
}
