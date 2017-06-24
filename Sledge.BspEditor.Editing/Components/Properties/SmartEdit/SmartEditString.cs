using System.Windows.Forms;
using Sledge.DataStructures.GameData;

namespace Sledge.BspEditor.Editing.Components.Properties.SmartEdit
{
    internal class SmartEditString : SmartEditControl
    {
        private readonly TextBox _textBox;
        public SmartEditString()
        {
            _textBox = new TextBox { Width = 250 };
            _textBox.TextChanged += (sender, e) => OnValueChanged();
            Controls.Add(_textBox);
        }

        public override bool SupportsType(VariableType type)
        {
            return type == VariableType.String;
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