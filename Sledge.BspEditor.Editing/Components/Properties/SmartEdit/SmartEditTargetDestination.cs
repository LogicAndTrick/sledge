using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.DataStructures.GameData;

namespace Sledge.BspEditor.Editing.Components.Properties.SmartEdit
{
    internal class SmartEditTargetDestination : SmartEditControl
    {
        private readonly ComboBox _comboBox;
        public SmartEditTargetDestination()
        {
            _comboBox = new ComboBox { Width = 250 };
            _comboBox.TextChanged += (sender, e) => OnValueChanged();
            Controls.Add(_comboBox);
        }

        public override bool SupportsType(VariableType type)
        {
            return type == VariableType.TargetDestination;
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
            return Document.Map.Root.Find(x => x.Data.GetOne<EntityData>() != null)
                .Select(x => x.Data.GetOne<EntityData>().Get<string>("targetname"))
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