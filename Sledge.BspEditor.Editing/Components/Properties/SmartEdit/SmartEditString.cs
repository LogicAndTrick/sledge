using System.ComponentModel.Composition;
using System.Windows.Forms;
using Sledge.BspEditor.Documents;
using Sledge.DataStructures.GameData;

namespace Sledge.BspEditor.Editing.Components.Properties.SmartEdit
{
    [Export(typeof(IObjectPropertyEditor))]
    public class SmartEditString : SmartEditControl
    {
        private readonly TextBox _textBox;
        public SmartEditString()
        {
            _textBox = new TextBox { Width = 250 };
            _textBox.TextChanged += (sender, e) => OnValueChanged();
            Controls.Add(_textBox);
        }

        public override string PriorityHint => "H";

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

        protected override void OnSetProperty(MapDocument document)
        {
            _textBox.Text = PropertyValue;
        }
    }
}