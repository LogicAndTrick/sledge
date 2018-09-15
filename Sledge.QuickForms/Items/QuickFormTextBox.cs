using System;
using System.Drawing;
using System.Windows.Forms;

namespace Sledge.QuickForms.Items
{
    /// <summary>
    /// A control that shows a text box.
    /// </summary>
    public class QuickFormTextBox : QuickFormItem
    {
        public override object Value => _textBox.Text;

        private readonly Label _label;
        private readonly TextBox _textBox;

        public QuickFormTextBox(string text, string value)
        {
            _label = new Label
            {
                Text = text,
                AutoSize = true,
                MinimumSize = new Size(LabelWidth, 0),
                MaximumSize = new Size(LabelWidth, 1000),
                TextAlign = ContentAlignment.MiddleRight,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom
            };
            _textBox = new TextBox
            {
                Text = value
            };

            Controls.Add(_label);
            Controls.Add(_textBox);
        }

        protected override void OnResize(EventArgs eventargs)
        {
            _textBox.Width = Width - _label.Width - _label.Margin.Horizontal - _textBox.Margin.Horizontal;
            base.OnResize(eventargs);
        }
    }
}
