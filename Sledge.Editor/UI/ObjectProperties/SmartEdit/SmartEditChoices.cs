using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Sledge.DataStructures.GameData;

namespace Sledge.Editor.UI.ObjectProperties.SmartEdit
{
    [SmartEdit(VariableType.Choices)]
    internal class SmartEditChoices : SmartEditControl
    {
        private readonly ComboBox _comboBox;
        public SmartEditChoices()
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
            if (Property != null)
            {
                var opt = Property.Options.FirstOrDefault(x => x.Description == _comboBox.Text);
                if (opt != null) return opt.Key;
                opt = Property.Options.FirstOrDefault(x => x.Key == _comboBox.Text);
                if (opt != null) return opt.Key;
            }
            return _comboBox.Text;
        }

        private IEnumerable<Option> GetSortedOptions()
        {
            int key;
            if (Property.Options.All(x => int.TryParse(x.Key, out key)))
            {
                return Property.Options.OrderBy(x => int.Parse(x.Key));
            }
            return Property.Options.OrderBy(x => x.Key.ToLowerInvariant());
        }

        protected override void OnSetProperty()
        {
            _comboBox.Items.Clear();
            if (Property != null)
            {
                var options = GetSortedOptions().ToList();
                _comboBox.Items.AddRange(options.Select(x => x.DisplayText()).OfType<object>().ToArray());
                var index = options.FindIndex(x => String.Equals(x.Key, PropertyValue, StringComparison.InvariantCultureIgnoreCase));
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