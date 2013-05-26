using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Sledge.Common.Mediator;
using Sledge.DataStructures.GameData;
using Sledge.DataStructures.MapObjects;
using Property = Sledge.DataStructures.MapObjects.Property;

namespace Sledge.Editor.UI
{
    public partial class EntityEditor : Form, IMediatorListener
    {
        private class TableValue
        {
            public string Class { get; set; }
            public string Key { get; set; }
            public string DisplayText { get; set; }
            public string Value { get; set; }
            public bool IsModified { get; set; }
            public bool IsAdded { get; set; }
            public bool IsRemoved { get; set; }

            public Color GetColour()
            {
                if (IsAdded) return Color.LightBlue;
                if (IsRemoved) return Color.LightPink;
                if (IsModified) return Color.LightGreen;
                return Color.Transparent;
            }

            public string DisplayValue(GameData gd)
            {
                var cls = gd.Classes.FirstOrDefault(x => x.Name == Class);
                var prop = cls == null ? null : cls.Properties.FirstOrDefault(x => x.Name == Key && x.VariableType == VariableType.Choices);
                var opt = prop == null ? null : prop.Options.FirstOrDefault(x => x.Key == Value);
                return opt == null ? Value : opt.Description;
            }

            public static List<TableValue> Create(GameData gd, string className, List<Property> props)
            {
                var list = new List<TableValue>();
                var cls = gd.Classes.FirstOrDefault(x => x.Name == className);
                var gameDataProps = cls != null ? cls.Properties : new List<DataStructures.GameData.Property>();
                foreach (var gdProps in gameDataProps.Where(x => x.Name != "spawnflags").GroupBy(x => x.Name))
                {
                    var gdProp = gdProps.First();
                    var vals = props.Where(x => x.Key == gdProp.Name).Select(x => x.Value).Distinct().ToList();
                    var value = vals.Count == 0 ? gdProp.DefaultValue : (vals.Count == 1 ? vals.First() : "<multiple values>" + String.Join(", ", vals));
                    list.Add(new TableValue { Class = className, DisplayText = gdProp.DisplayText(), Key = gdProp.Name, Value = value});
                }
                foreach (var group in props.Where(x => gameDataProps.All(y => x.Key != y.Name)).GroupBy(x => x.Key))
                {
                    var vals = group.Select(x => x.Value).Distinct().ToList();
                    var value = vals.Count == 1 ? vals.First() : "<multiple values> - " + String.Join(", ", vals);
                    list.Add(new TableValue { Class = className, DisplayText = group.Key, Key = group.Key, Value = value });
                }
                return list;
            }
        }

        private List<TableValue> _values;

        private readonly Dictionary<VariableType, SmartEditControl> _smartEditControls;
        public List<MapObject> Objects { get; set; }
        private bool _changingClass;
        private string _prevClass;
        private Documents.Document Document { get; set; }

        private bool _populating;

        public EntityEditor(Documents.Document document)
        {
            Document = document;
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

        private void Apply()
        {
            var ents = Objects.OfType<Entity>().ToList();
            foreach (var entity in ents)
            {
                // var currentProperties = entity.EntityData.Properties.ToDictionary(x => x.Key, x => x.Value);
                // Updated class
                if (Class.BackColor == Color.LightGreen)
                {
                    entity.EntityData.Name = Class.Text;
                }

                // Remove nonexistant properties
                entity.EntityData.Properties.RemoveAll(x => _values.All(y => y.Key != x.Key));

                // Set updated/new properties
                foreach (var ent in _values.Where(x => x.IsModified))
                {
                    var prop = entity.EntityData.Properties.FirstOrDefault(x => x.Key == ent.Key);
                    if (prop == null)
                    {
                        prop = new Property {Key = ent.Key};
                        entity.EntityData.Properties.Add(prop);
                    }
                    prop.Value = ent.Value;
                }

                // Set flags
                var flags = Enumerable.Range(0, FlagsTable.Items.Count).Select(x => FlagsTable.GetItemCheckState(x)).ToList();
                var entClass = Document.GameData.Classes.FirstOrDefault(x => x.Name == entity.EntityData.Name);
                var spawnFlags = entClass == null ? null : entClass.Properties.FirstOrDefault(x => x.Name == "spawnflags");
                var opts = spawnFlags == null ? null : spawnFlags.Options;
                if (opts == null || flags.Count != opts.Count) continue;

                for (var i = 0; i < flags.Count; i++)
                {
                    var val = int.Parse(opts[i].Key);
                    if (flags[i] == CheckState.Unchecked) entity.EntityData.Flags &= ~val; // Switch the flag off if unchecked
                    else if (flags[i] == CheckState.Checked) entity.EntityData.Flags |= val; // Switch it on if checked
                    // No change if indeterminate
                }
            }
            var classes = Objects.OfType<Entity>().Select(x => x.EntityData.Name.ToLower()).Distinct().ToList();
            var cls = classes.Count > 1 ? "" : classes[0];
            _values = TableValue.Create(Document.GameData, cls, Objects.OfType<Entity>().SelectMany(x => x.EntityData.Properties).Where(x => x.Key != "spawnflags").ToList());
            Class.BackColor = Color.White;
            UpdateKeyValues();
        }

        public void Notify(string message, object data)
        {
            if (message == EditorMediator.SelectionChanged.ToString() || message == EditorMediator.SelectionTypeChanged.ToString())
            {
                UpdateObjects();
            }
        }

        private void UpdateObjects()
        {
            Objects.Clear();
            if (!Document.Selection.InFaceSelection)
            {
                Objects.AddRange(Document.Selection.GetSelectedParents());
            }
            RefreshData();
        }

        protected override void OnLoad(EventArgs e)
        {
            UpdateObjects();

            Mediator.Subscribe(EditorMediator.SelectionChanged, this);
            Mediator.Subscribe(EditorMediator.SelectionTypeChanged, this);
        }

        protected override void OnClosed(EventArgs e)
        {
            Mediator.UnsubscribeAll(this);
            base.OnClosed(e);
        }
        
        private void RefreshData()
        {
            Tabs.TabPages.Clear();
            if (!Objects.Any()) return;
            Tabs.TabPages.AddRange(new[] {ClassInfoTab, InputsTab, OutputsTab, FlagsTab, VisgroupTab});
            if (!Objects.All(x => x is Entity))
            {
                Tabs.TabPages.Remove(ClassInfoTab);
                Tabs.TabPages.Remove(InputsTab);
                Tabs.TabPages.Remove(OutputsTab);
                Tabs.TabPages.Remove(FlagsTab);
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
                                     .Select(x => x.Name).OrderBy(x => x.ToLower()).OfType<object>().ToArray());
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
            _values = TableValue.Create(Document.GameData, cls, Objects.OfType<Entity>().SelectMany(x => x.EntityData.Properties).Where(x => x.Key != "spawnflags").ToList());
            _prevClass = cls;
            UpdateKeyValues();
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

        private void UpdateKeyValues()
        {
            _populating = true;

            var smartEdit = SmartEditButton.Checked;
            var selectedIndex = KeyValuesList.SelectedIndices.Count == 0 ? -1 : KeyValuesList.SelectedIndices[0];
            KeyValuesList.Items.Clear();
            foreach (var tv in _values)
            {
                var dt = smartEdit ? tv.DisplayText : tv.Key;
                var dv = smartEdit ? tv.DisplayValue(Document.GameData) : tv.Value;
                KeyValuesList.Items.Add(new ListViewItem(dt) { Tag = tv.Key, BackColor = tv.GetColour() }).SubItems.Add(dv);
            }

            Angles.Enabled = false;
            var angleVal = _values.FirstOrDefault(x => x.Key == "angles");
            if (angleVal != null)
            {
                Angles.Enabled = !_changingClass;
                Angles.SetAnglePropertyString(angleVal.Value);
            }

            if (selectedIndex >= 0) KeyValuesList.SelectedIndices.Add(selectedIndex);

            //KeyValuesListSelectedIndexChanged(null, null);

            _populating = false;
        }

        private void SmartEditToggled(object sender, EventArgs e)
        {
            if (_populating) return;
            UpdateKeyValues();
            KeyValuesListSelectedIndexChanged(null, null);
        }

        #region Class Change

        private void StartClassChange(object sender, EventArgs e)
        {
            if (_populating) return;
            KeyValuesList.SelectedItems.Clear();
            _changingClass = true;
            Class.BackColor = Color.LightBlue;

            var className = Class.Text;
            if (_values.All(x => x.Class == null || x.Class == className))
            {
                CancelClassChange(null, null);
                return;
            }

            var cls = Document.GameData.Classes.FirstOrDefault(x => x.Name == className);
            var props = cls != null ? cls.Properties : new List<DataStructures.GameData.Property>();

            // Mark the current properties that aren't in the new class as 'removed'
            foreach (var tv in _values)
            {
                var prop = props.FirstOrDefault(x => x.Name == tv.Key);
                tv.IsRemoved = prop == null;
                tv.DisplayText = prop == null ? tv.Key : prop.DisplayText();
            }

            // Add the new properties that aren't in the new class as 'added'
            foreach (var prop in props.Where(x => x.Name != "spawnflags" && _values.All(y => y.Key != x.Name)))
            {
                _values.Add(new TableValue { DisplayText = prop.DisplayText(), Key = prop.Name, IsAdded = true, Value = prop.DefaultValue });
            }

            FlagsTable.Enabled = ApplyButton.Enabled = false;
            ConfirmClassChangeButton.Enabled = CancelClassChangeButton.Enabled = ChangingClassWarning.Visible = true;
            UpdateKeyValues();
        }

        private void ConfirmClassChange(object sender, EventArgs e)
        {
            // Changing class: remove all the 'removed' properties, reset the rest to normal
            var className = Class.Text;
            var cls = Document.GameData.Classes.FirstOrDefault(x => x.Name == className);
            Class.BackColor = Color.LightGreen;
            _values.RemoveAll(x => x.IsRemoved);
            foreach (var tv in _values)
            {
                tv.Class = className;
                tv.IsModified = tv.IsModified || tv.IsAdded;
                tv.IsAdded = false;
                var prop = cls != null ? cls.Properties.FirstOrDefault(x => x.Name == tv.Key) : null;
                tv.DisplayText = prop == null ? tv.Key : prop.DisplayText();
            }

            // Update the flags table
            FlagsTable.Items.Clear();
            var flagsProp = cls == null ? null : cls.Properties.FirstOrDefault(x => x.Name == "spawnflags");
            if (flagsProp != null)
            {
                foreach (var option in flagsProp.Options.OrderBy(x => int.Parse(x.Key)))
                {
                    FlagsTable.Items.Add(option.Description, option.On ? CheckState.Checked : CheckState.Unchecked);
                }
            }

            _changingClass = false;
            UpdateKeyValues();
            FlagsTable.Enabled = ApplyButton.Enabled = true;
            ConfirmClassChangeButton.Enabled = CancelClassChangeButton.Enabled = ChangingClassWarning.Visible = false;
            _prevClass = className;
        }

        private void CancelClassChange(object sender, EventArgs e)
        {
            // Cancelling class change: remove all the 'added' properties, reset the rest to normal
            Class.Text = _prevClass;
            var className = Class.Text;
            var cls = Document.GameData.Classes.FirstOrDefault(x => x.Name == className);
            Class.BackColor = Color.White;
            _values.RemoveAll(x => x.IsAdded);
            foreach (var tv in _values)
            {
                tv.IsRemoved = false;
                var prop = cls != null ? cls.Properties.FirstOrDefault(x => x.Name == tv.Key) : null;
                tv.DisplayText = prop == null ? tv.Key : prop.DisplayText();
            }

            _changingClass = false;
            UpdateKeyValues();
            FlagsTable.Enabled = ApplyButton.Enabled = true;
            ConfirmClassChangeButton.Enabled = CancelClassChangeButton.Enabled = ChangingClassWarning.Visible = false;
        }

        private void KeyValuesListItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (_changingClass && e.Item.Selected) e.Item.Selected = false;
        }

        #endregion

        private void PropertyValueChanged(object sender, string propertyname, string propertyvalue)
        {
            var val = _values.FirstOrDefault(x => x.Key == propertyname);
            if (val == null) return;
            val.IsModified = true;
            val.Value = propertyvalue;
            var li = KeyValuesList.Items.OfType<ListViewItem>().FirstOrDefault(x => ((string) x.Tag) == propertyname);
            if (li != null)
            {
                li.BackColor = val.GetColour();
                li.SubItems[1].Text = val.Value;
            }
            if (propertyname == "angles" && propertyvalue != Angles.GetAnglePropertyString())
            {
                Angles.SetAnglePropertyString(propertyvalue);
            }
        }

        private void AnglesChanged(object sender, AngleControl.AngleChangedEventArgs e)
        {
            if (_populating) return;
            PropertyValueChanged(sender, "angles", Angles.GetAnglePropertyString());
            if (KeyValuesList.SelectedIndices.Count > 0
                && ((string) KeyValuesList.SelectedItems[0].Tag) == "angles"
                && SmartEditControlPanel.Controls.Count > 0
                && SmartEditControlPanel.Controls[0] is SmartEditControl)
            {
                ((SmartEditControl) SmartEditControlPanel.Controls[0]).SetProperty("angles", Angles.GetAnglePropertyString(), null);
            }
        }

        private void KeyValuesListSelectedIndexChanged(object sender, EventArgs e)
        {
            HelpTextbox.Text = "";
            CommentsTextbox.Text = "";
            SmartEditControlPanel.Controls.Clear();
            if (KeyValuesList.SelectedItems.Count == 0 || _changingClass) return;
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

        #region Smart Edit Controls

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
                if (Property != null)
                {
                    var opt = Property.Options.FirstOrDefault(x => x.Description == _comboBox.Text);
                    if (opt != null) return opt.Key;
                    opt = Property.Options.FirstOrDefault(x => x.Key == _comboBox.Text);
                    if (opt != null) return opt.Key;
                }
                return _comboBox.Text;
            }

            protected override void OnSetProperty()
            {
                _comboBox.Items.Clear();
                if (Property != null)
                {
                    var options = Property.Options.OrderBy(x => int.Parse(x.Key)).ToList();
                    _comboBox.Items.AddRange(options.Select(x => x.DisplayText()).OfType<object>().ToArray());
                    int selection;
                    if (int.TryParse(PropertyValue, out selection))
                    {
                        var index = options.FindIndex(x => int.Parse(x.Key) == selection);
                        if (index >= 0)
                        {
                            _comboBox.SelectedIndex = index;
                            return;
                        }
                    }
                }
                _comboBox.Text = PropertyValue;
            }
        }

        #endregion

        private void ApplyButtonClicked(object sender, EventArgs e)
        {
            Apply();
        }
    }
}
