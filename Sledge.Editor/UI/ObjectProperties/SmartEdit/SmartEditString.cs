using System.Windows.Forms;
using Sledge.DataStructures.GameData;

namespace Sledge.Editor.UI.ObjectProperties.SmartEdit
{
    [SmartEdit(VariableType.String)]
    internal class SmartEditString : SmartEditControl
    {
        private readonly TextBox _textBox;
        public SmartEditString()
        {
            _textBox = new TextBox { Width = 250 };
            _textBox.TextChanged += (sender, e) => OnValueChanged();
            Controls.Add(_textBox);
        }

        protected override string GetName()
        {
            return OriginalName;
        }

        protected override string GetValue()
        {
            return _textBox.Text;
        }

        protected override void OnSetProperty()
        {
            _textBox.Text = PropertyValue;
        }
    }
}