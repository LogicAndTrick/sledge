using System;
using Sledge.Gui.Controls;
using Sledge.Gui.Interfaces;

namespace Sledge.EditorNew.Brushes.Controls
{
    public class TextControl : BrushControl
    {
        public string EnteredTextKey
        {
            get { return TextBox.TextKey; }
            set { TextBox.TextKey = value; }
        }

        public string LabelTextKey
        {
            get { return Label.TextKey; }
            set { Label.TextKey = value; }
        }

        public Label Label { get; set; }
        public TextBox TextBox { get; set; }

        public TextControl(IBrush brush) : base(brush)
        {
            Label = new Label();
            this.Add(Label);

            TextBox = new TextBox();
            TextBox.TextChanged += ValueChanged;
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
