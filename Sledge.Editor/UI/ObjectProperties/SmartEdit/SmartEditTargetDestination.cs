using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Sledge.DataStructures.GameData;

namespace Sledge.Editor.UI.ObjectProperties.SmartEdit
{
    [SmartEdit(VariableType.TargetDestination)]
    internal class SmartEditTargetDestination : SmartEditControl
    {
        private readonly ComboBox _comboBox;
        public SmartEditTargetDestination()
        {
            _comboBox = new ComboBox { Width = 250 };
            _comboBox.TextChanged += (sender, e) => OnValueChanged();
            Controls.Add(_comboBox);
        }

        protected override string GetName()
        {
            return OriginalName;
        }

        protected override string GetValue()
        {
            return _comboBox.Text;
        }

        private IEnumerable<string> GetSortedTargetNames()
        {
            return Document.Map.WorldSpawn.Find(x => x.GetEntityData() != null)
                .Select(x => x.GetEntityData().GetPropertyValue("targetname"))
                .Where(x => !String.IsNullOrWhiteSpace(x))
                .Distinct()
                .OrderBy(x => x.ToLowerInvariant());
        }

        protected override void OnSetProperty()
        {
            _comboBox.Items.Clear();
            if (Property != null)
            {
                var options = GetSortedTargetNames().ToList();
                _comboBox.Items.AddRange(options.OfType<object>().ToArray());
                var index = options.FindIndex(x => String.Equals(x, PropertyValue, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    _comboBox.SelectedIndex = index;
                    return;
                }
            }
            _comboBox.Text = PropertyValue;
        }
    }
}