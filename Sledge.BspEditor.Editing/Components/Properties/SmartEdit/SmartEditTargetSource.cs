using System.ComponentModel.Composition;
using System.Drawing;
using System.Windows.Forms;
using Sledge.BspEditor.Documents;
using Sledge.DataStructures.GameData;

namespace Sledge.BspEditor.Editing.Components.Properties.SmartEdit
{
    [Export(typeof(IObjectPropertyEditor))]
    public class SmartEditTargetSource : SmartEditControl
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

        public override string PriorityHint => "H";

        public override bool SupportsType(VariableType type)
        {
            return type == VariableType.TargetSource;
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

        protected override void OnSetProperty(MapDocument document)
        {
            _textBox.Text = PropertyValue;
            DoValidation();
        }

        private void DoValidation()
        {
            // todo BETA: smartedit controls need to know more about what they're editing
            // _validationLabel.Text = "";
            // if (EditingEntityData.Count > 1)
            // {
            //     _validationLabel.Text = "Multiple selected, creating duplicates";
            // }
            // else
            // {
            //     var duplicate = document.Map.Root
            //         .Find(x => x.Data.GetOne<EntityData>() != null
            //                    && !EditingEntityData.Contains(x.Data.GetOne<EntityData>())
            //                    && x.Data.GetOne<EntityData>().Get<string>("targetname") == _textBox.Text)
            //         .Any();
            //     if (duplicate)
            //     {
            //         _validationLabel.Text = "Another target with this name already exists.";
            //     }
            // }
        }
    }
}