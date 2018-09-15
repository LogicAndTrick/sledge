using System;
using System.Drawing;
using System.Windows.Forms;

namespace Sledge.QuickForms.Items
{
    /// <summary>
    /// A control that shows a text box and a browse button for file selection.
    /// </summary>
    public class QuickFormBrowse : QuickFormItem
    {
	    public override object Value => _textBox.Text;

        private readonly Label _label;
        private readonly TextBox _textBox;
        private readonly string _filter;
        private readonly Button _button;

        public QuickFormBrowse(string text, string browseText, string fileFilter)
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
                Text = ""
            };
            _button = new Button
            {
                Text = browseText,
                AutoSize = true,
                MinimumSize = new Size(60, 0),
                MaximumSize = new Size(1000, _textBox.Height),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom
            };

            _button.Click += (s,e) => ShowBrowseDialog();
            
            Controls.Add(_label);
            Controls.Add(_textBox);
            Controls.Add(_button);
        }

        protected override void OnResize(EventArgs eventargs)
        {
            _textBox.Width = Width - _label.Width - _label.Margin.Horizontal - _textBox.Margin.Horizontal - _button.Width - _button.Margin.Horizontal;
            base.OnResize(eventargs);
        }

        private void ShowBrowseDialog()
        {
            using (var ofd = new OpenFileDialog {Filter = _filter, FileName = _textBox.Text})
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    _textBox.Text = ofd.FileName;
                }
            }
        }
    }
}