using System;
using System.Windows.Forms;
using Sledge.DataStructures.GameData;

namespace Sledge.BspEditor.Editing.Components.Properties.SmartEdit
{
    internal class SmartEditBoolean : SmartEditControl
    {
        private readonly CheckBox _checkBox;
        public SmartEditBoolean()
        {
            _checkBox = new CheckBox {AutoSize = true, Checked = false, Text = "Enabled / Active"};
            _checkBox.CheckedChanged += (sender, e) => OnValueChanged();
            Controls.Add(_checkBox);
        }

        public override bool SupportsType(VariableType type)
        {
            return type == VariableType.Boolean;
        }

        protected override string GetName()
        {
            return OriginalName;
        }

        protected override string GetValue()
        {
            return _checkBox.Checked ? "Yes" : "No";
        }

        protected override void OnSetProperty()
        {
            _checkBox.Text = Property.DisplayText();
            _checkBox.Checked = String.Equals(PropertyValue, "Yes", StringComparison.CurrentCultureIgnoreCase);
        }
    }
}