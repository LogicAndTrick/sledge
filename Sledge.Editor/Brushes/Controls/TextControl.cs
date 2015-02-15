using System;
using Sledge.Editor.UI;

namespace Sledge.Editor.Brushes.Controls
{
    public partial class TextControl : BrushControl
    {
        public string EnteredText
        {
            get { return TextBox.Text; }
            set { TextBox.Text = value; }
        }

        public TextControl(IBrush brush) : base(brush)
        {
            InitializeComponent();
            TextBox.Tag = Hotkeys.SuppressHotkeysTag;
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
