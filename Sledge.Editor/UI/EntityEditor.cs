using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Sledge.DataStructures.GameData;
using Sledge.DataStructures.MapObjects;
using Property = Sledge.DataStructures.MapObjects.Property;

namespace Sledge.Editor.UI
{
    public partial class EntityEditor : Form
    {
        private readonly Dictionary<VariableType, SmartEditControl> _smartEditControls; 
        public List<MapObject> Objects { get; set; }

        private bool _populating;

        public EntityEditor()
        {
            InitializeComponent();
            Objects = new List<MapObject>();
            _smartEditControls = new Dictionary<VariableType, SmartEditControl>();

            RegisterSmartEditControl(VariableType.String, new SmartEditString());
            RegisterSmartEditControl(VariableType.Integer, new SmartEditInteger());
            RegisterSmartEditControl(VariableType.Choices, new SmartEditChoices());
        }

        private void RegisterSmartEditControl(VariableType type, SmartEditControl ctrl)
        {
            ctrl.ValueChanged += PropertyValueChanged;
            ctrl.Dock = DockStyle.Fill;
            _smartEditControls.Add(type, ctrl);
        }

        protected override void OnLoad(System.EventArgs e)
        {
            if (!Objects.All(x => x is Entity))
            {
                Controls.Remove(ClassInfoTab);
                Controls.Remove(InputsTab);
                Controls.Remove(OutputsTab);
                Controls.Remove(FlagsTab);
                return;
            }
            if (Document.Game.EngineID == 1) // Goldsource
            {
                Tabs.TabPages.Remove(InputsTab);
                Tabs.TabPages.Remove(OutputsTab);
            }
            _populating = true;
            Class.Items.Clear();
            Class.Items.AddRange(Document.GameData.Classes
                                     .Where(x => x.ClassType != ClassType.Base && x.Name != "worldspawn")
                                     .Select(x => x.Name).OrderBy(x => x.ToLower()).ToArray());
            if (!Objects.Any()) return;
            var classes = Objects.OfType<Entity>().Select(x => x.EntityData.Name.ToLower()).Distinct().ToList();
            var cls = classes.Count > 1 ? "" : classes[0];
            if (classes.Count > 1)
            {
                Class.Text = @"<multiple types> - " + String.Join(", ", classes);
                SmartEditButton.Checked = SmartEditButton.Enabled = false;
            }
            else
            {
                var idx = Class.Items.IndexOf(cls);
                if (idx >= 0)
                {
                    Class.SelectedIndex = idx;
                    SmartEditButton.Checked = SmartEditButton.Enabled = true;
                }
                else
                {
                    Class.Text = cls;
                    SmartEditButton.Checked = SmartEditButton.Enabled = false;
                }
            }
            PopulateKeyValues(Objects.OfType<Entity>().SelectMany(x => x.EntityData.Properties).Where(x => x.Key != "spawnflags").ToList());
            PopulateFlags(cls, Objects.OfType<Entity>().Select(x => x.EntityData.Flags).ToList());
            _populating = false;
        }

        private void PopulateFlags(string className, List<int> flags)
        {
            FlagsTable.Items.Clear();
            var cls = Document.GameData.Classes.FirstOrDefault(x => x.Name == className);
            if (cls == null) return;
            var flagsProp = cls.Properties.FirstOrDefault(x => x.Name == "spawnflags");
            if (flagsProp == null) return;
            foreach (var option in flagsProp.Options.OrderBy(x => int.Parse(x.Key)))
            {
                var key = int.Parse(option.Key);
                var numChecked = flags.Count(x => (x & key) > 0);
                FlagsTable.Items.Add(option.Description, numChecked == flags.Count ? CheckState.Checked : (numChecked == 0 ? CheckState.Unchecked : CheckState.Indeterminate));
            }
        }

        private void PopulateKeyValues(List<Property> props)
        {
            var smartEdit = SmartEditButton.Checked;
            var className = Class.Text;
            var cls = Document.GameData.Classes.FirstOrDefault(x => x.Name == className);
            var gameDataProps = smartEdit && cls != null ? cls.Properties : new List<DataStructures.GameData.Property>();
            KeyValuesList.Items.Clear();
            if (smartEdit)
            {
                foreach (var gdProps in gameDataProps.Where(x => x.Name != "spawnflags").GroupBy(x => x.Name))
                {
                    var gdProp = gdProps.First();
                    var vals = props.Where(x => x.Key == gdProp.Name).Select(x => x.Value).Distinct().ToList();
                    var value = vals.Count == 0 ? gdProp.DefaultValue : (vals.Count == 1 ? vals.First() : "<multiple values>");
                    KeyValuesList.Items.Add(new ListViewItem(gdProp.DisplayText()) { Tag = gdProp.Name }).SubItems.Add(value);
                }
            }
            foreach (var group in props.Where(x => !gameDataProps.Any(y => x.Key == y.Name)).GroupBy(x => x.Key))
            {
                var vals = group.Select(x => x.Value).Distinct().ToList();
                var value = vals.Count == 1 ? vals.First() : "<multiple values> - " + String.Join(", ", vals);
                KeyValuesList.Items.Add(new ListViewItem(group.Key) { Tag = group.Key }).SubItems.Add(value);
            }
        }

        private void SmartEditToggled(object sender, EventArgs e)
        {
            if (_populating) return;
            // Relabel the table keys without destroying the current data which is in an unsaved state
            var smartEdit = SmartEditButton.Checked;
            if (smartEdit)
            {
                var className = Class.Text;
                var cls = Document.GameData.Classes.FirstOrDefault(x => x.Name == className);
                var gameDataProps = cls != null ? cls.Properties : new List<DataStructures.GameData.Property>();
                foreach (ListViewItem item in KeyValuesList.Items)
                {
                    var key = (string)item.Tag;
                    item.Text = gameDataProps.Where(x => x.Name == key).Take(1).Select(x => x.DisplayText()).FirstOrDefault() ?? key;
                }
            }
            else
            {
                foreach (ListViewItem item in KeyValuesList.Items)
                {
                    item.Text = (string) item.Tag;
                }
            }
            KeyValuesListSelectedIndexChanged(null, null);
        }

        private void ClassChanged(object sender, EventArgs e)
        {
            if (_populating) return;
            Class.BackColor = Color.LightBlue;
            var smartEdit = SmartEditButton.Checked;
            var className = Class.Text;
            var cls = Document.GameData.Classes.FirstOrDefault(x => x.Name == className);
            FlagsTable.Items.Clear();
            // If the new class does not exist, do not change the table
            if (cls == null)
            {
                SmartEditButton.Checked = SmartEditButton.Enabled = false;
                SmartEditToggled(null, null);
                return;
            }
            // If the new class does exist, retain keys that match the new definition, delete the rest, and add default values for the new keys
            var vals = KeyValuesList.Items.OfType<ListViewItem>().ToDictionary(x => (string) x.Tag, x => x.SubItems[1].Text);
            KeyValuesList.Items.Clear();
            foreach (var prop in cls.Properties.Where(x => x.Name != "spawnflags"))
            {
                var text = smartEdit ? prop.DisplayText() : prop.Name;
                var value = vals.ContainsKey(prop.Name) ? vals[prop.Name] : prop.DefaultValue;
                KeyValuesList.Items.Add(new ListViewItem(text) { Tag = prop.Name, BackColor = Color.LightBlue }).SubItems.Add(value);
            }
            var flagsProp = cls.Properties.FirstOrDefault(x => x.Name == "spawnflags");
            if (flagsProp == null) return;
            foreach (var option in flagsProp.Options.OrderBy(x => int.Parse(x.Key)))
            {
                FlagsTable.Items.Add(option.Description, option.On ? CheckState.Checked : CheckState.Unchecked);
            }
        }

        private void PropertyValueChanged(object sender, string propertyname, string propertyvalue)
        {
            var kv = KeyValuesList.Items.OfType<ListViewItem>().FirstOrDefault(x => ((string) x.Tag) == propertyname);
            if (kv == null) return;
            kv.SubItems[1].Text = propertyvalue;
            kv.BackColor = Color.LightBlue;
        }

        private void KeyValuesListSelectedIndexChanged(object sender, System.EventArgs e)
        {
            HelpTextbox.Text = "";
            CommentsTextbox.Text = "";
            SmartEditControlPanel.Controls.Clear();
            if (KeyValuesList.SelectedItems.Count == 0) return;
            var smartEdit = SmartEditButton.Checked;
            var className = Class.Text;
            var selected = KeyValuesList.SelectedItems[0];
            var propName = (string) selected.Tag;
            var value = selected.SubItems[1].Text;
            var cls = Document.GameData.Classes.FirstOrDefault(x => x.Name == className);
            var gdProp = smartEdit && cls != null ? cls.Properties.FirstOrDefault(x => x.Name == propName) : null;
            if (gdProp != null)
            {
                HelpTextbox.Text = gdProp.Description;
            }
            AddSmartEditControl(gdProp, propName, value);
        }

        private void AddSmartEditControl(DataStructures.GameData.Property property, string propertyName, string value)
        {
            SmartEditControlPanel.Controls.Clear();
            var ctrl = _smartEditControls[VariableType.String];
            if (property != null && _smartEditControls.ContainsKey(property.VariableType))
            {
                ctrl = _smartEditControls[property.VariableType];
            }
            ctrl.SetProperty(propertyName, value, property);
            SmartEditControlPanel.Controls.Add(ctrl);
        }

        private abstract class SmartEditControl : FlowLayoutPanel
        {
            public string PropertyName { get; private set; }
            public string PropertyValue { get; private set; }
            public DataStructures.GameData.Property Property { get; private set; }

            public delegate void ValueChangedEventHandler(object sender, string propertyName, string propertyValue);

            public event ValueChangedEventHandler ValueChanged;

            protected virtual void OnValueChanged()
            {
                if (_setting) return;
                PropertyValue = GetValue();
                if (ValueChanged != null)
                {
                    ValueChanged(this, PropertyName, PropertyValue);
                }
            }

            private bool _setting;

            public void SetProperty(string propertyName, string currentValue, DataStructures.GameData.Property property)
            {
                _setting = true;
                PropertyName = propertyName;
                PropertyValue = currentValue;
                Property = property;
                OnSetProperty();
                _setting = false;
            }

            protected abstract string GetValue();
            protected abstract void OnSetProperty();
        }

        private class SmartEditString : SmartEditControl
        {
            private readonly TextBox _textBox;
            public SmartEditString()
            {
                _textBox = new TextBox { Width = 250 };
                _textBox.TextChanged += (sender, e) => OnValueChanged();
                Controls.Add(_textBox);
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

        private class SmartEditInteger : SmartEditControl
        {
            private readonly NumericUpDown _numericUpDown;
            public SmartEditInteger()
            {
                _numericUpDown = new NumericUpDown {Width = 50, Minimum = short.MinValue, Maximum = short.MaxValue, Value = 0};
                _numericUpDown.ValueChanged += (sender, e) => OnValueChanged();
                Controls.Add(_numericUpDown);
            }

            protected override string GetValue()
            {
                return _numericUpDown.Value.ToString();
            }

            protected override void OnSetProperty()
            {
                _numericUpDown.Text = PropertyValue;
            }
        }

        private class SmartEditChoices : SmartEditControl
        {
            private readonly ComboBox _comboBox;
            public SmartEditChoices()
            {
                _comboBox = new ComboBox { Width = 250 };
                _comboBox.TextChanged += (sender, e) => OnValueChanged();
                Controls.Add(_comboBox);
            }

            protected override string GetValue()
            {
                return _comboBox.Text;
            }

            protected override void OnSetProperty()
            {
                _comboBox.Items.Clear();
                if (Property != null) _comboBox.Items.AddRange(Property.Options.Select(x => x.DisplayText()).ToArray());
                _comboBox.Text = PropertyValue;
            }
        }
    }
}
