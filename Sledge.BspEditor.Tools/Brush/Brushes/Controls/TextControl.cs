using System;

namespace Sledge.BspEditor.Tools.Brush.Brushes.Controls
{
    public partial class TextControl : BrushControl
    {
        public string EnteredText
        {
            get => TextBox.Text;
            set => TextBox.Text = value;
        }

        public TextControl(IBrush brush) : base(brush)
        {
            InitializeComponent();
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
