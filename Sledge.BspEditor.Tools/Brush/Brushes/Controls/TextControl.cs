using System;

namespace Sledge.BspEditor.Tools.Brush.Brushes.Controls
{
    public partial class TextControl : BrushControl
    {
        private string _value;

        public string EnteredText
        {
            get => TextBox.Text;
            set => TextBox.Text = _value = value;
        }

        public string LabelText
        {
            get => Label.Text;
            set => Label.Text = value;
        }

        public TextControl(IBrush brush) : base(brush)
        {
            InitializeComponent();
            _value = TextBox.Text;
        }

        public string GetValue()
        {
            return _value;
        }

        private void ValueChanged(object sender, EventArgs e)
        {
            _value = TextBox.Text;
            OnValuesChanged(Brush);
        }
    }
}
