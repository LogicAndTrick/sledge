using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Sledge.DataStructures.GameData;

namespace Sledge.Editor.UI.ObjectProperties.SmartEdit
{
    [SmartEdit(VariableType.TargetSource)]
    internal class SmartEditTargetSource : SmartEditControl
    {
        private readonly TextBox _textBox;
        private readonly Label _validationLabel;

        public SmartEditTargetSource()
        {
            _textBox = new TextBox { Width = 250 };
            _textBox.TextChanged += (sender, e) => OnValueChanged();
            Controls.Add(_textBox);

            _validationLabel = new Label
                                   {
                                       Text = "",
                                       AutoSize = false,
                                       Height = 18,
                                       Width = 250,
                                       TextAlign = ContentAlignment.BottomLeft,
                                       ForeColor = Color.Red
                                   };
            Controls.Add(_validationLabel);
        }

        protected override void OnValueChanged()
        {
            DoValidation();
            base.OnValueChanged();
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
            DoValidation();
        }

        private void DoValidation()
        {
            _validationLabel.Text = "";
            if (EditingEntityData.Count > 1)
            {
                _validationLabel.Text = "Multiple selected, creating duplicates";
            }
            else
            {
                var duplicate = Document.Map.WorldSpawn
                    .Find(x => x.GetEntityData() != null
                               && !EditingEntityData.Contains(x.GetEntityData())
                               && x.GetEntityData().GetPropertyValue("targetname") == _textBox.Text)
                    .Any();
                if (duplicate)
                {
                    _validationLabel.Text = "Another target with this name already exists.";
                }
            }
        }
    }
}