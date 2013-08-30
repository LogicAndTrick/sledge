using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Sledge.Common.Mediator;
using Sledge.DataStructures.GameData;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Actions;
using Sledge.Editor.Actions.MapObjects.Entities;
using Sledge.Editor.Actions.Visgroups;
using Sledge.Editor.Visgroups;
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
        public bool FollowSelection { get; set; }

        public bool AllowClassChange
        {
            set
            {
                CancelClassChangeButton.Enabled
                    = ConfirmClassChangeButton.Enabled
                      = Class.Enabled
                        = value; // It's like art or something!
            }
        }

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

            FollowSelection = true;
        }

        private void RegisterSmartEditControl(VariableType type, SmartEditControl ctrl)
        {
            ctrl.ValueChanged += PropertyValueChanged;
            ctrl.Dock = DockStyle.Fill;
            _smartEditControls.Add(type, ctrl);
        }

        private void Apply()
        {
            string actionText = null;
            var ac = new ActionCollection();
            var editAction = GetEditEntityDataAction();
            var visgroupAction = GetUpdateVisgroupsAction();

            if (editAction != null)
            {
                // The entity change is more important to show
                actionText = "Edit entity data";
                ac.Add(editAction);
            }

            if (visgroupAction != null)
            {
                // Visgroup change shows if entity data not changed
                if (actionText == null) actionText = "Edit object visgroups";
                ac.Add(visgroupAction);
            }

            if (!ac.IsEmpty())
            {
                // Run if either action shows changes
                Document.PerformAction(actionText, ac);
            }

            Class.BackColor = Color.White;
        }

        private EditEntityData GetEditEntityDataAction()
        {
            var ents = Objects.Where(x => x is Entity || x is World).ToList();
            if (!ents.Any()) return null;
            var action = new EditEntityData();

            foreach (var entity in ents)
            {
                var entityData = entity.GetEntityData().Clone();
                var changed = false;
                // Updated class
                if (Class.BackColor == Color.LightGreen)
                {
                    entityData.Name = Class.Text;
                    changed = true;
                }

                // Remove nonexistant properties
                var nonExistant = entityData.Properties.Where(x => _values.All(y => y.Key != x.Key));
                if (nonExistant.Any())
                {
                    changed = true;
                    entityData.Properties.RemoveAll(x => _values.All(y => y.Key != x.Key));
                }

                // Set updated/new properties
                foreach (var ent in _values.Where(x => x.IsModified))
                {
                    var prop = entityData.Properties.FirstOrDefault(x => x.Key == ent.Key);
                    if (prop == null)
                    {
                        prop = new Property {Key = ent.Key};
                        entityData.Properties.Add(prop);
                    }
                    prop.Value = ent.Value;
                    changed = true;
                }

                // Set flags
                var flags =
                    Enumerable.Range(0, FlagsTable.Items.Count).Select(x => FlagsTable.GetItemCheckState(x)).ToList();
                var entClass = Document.GameData.Classes.FirstOrDefault(x => x.Name == entityData.Name);
                var spawnFlags = entClass == null
                                     ? null
                                     : entClass.Properties.FirstOrDefault(x => x.Name == "spawnflags");
                var opts = spawnFlags == null ? null : spawnFlags.Options;
                if (opts == null || flags.Count != opts.Count) continue;

                var beforeFlags = entityData.Flags;
                for (var i = 0; i < flags.Count; i++)
                {
                    var val = int.Parse(opts[i].Key);
                    if (flags[i] == CheckState.Unchecked) entityData.Flags &= ~val; // Switch the flag off if unchecked
                    else if (flags[i] == CheckState.Checked) entityData.Flags |= val; // Switch it on if checked
                    // No change if indeterminate
                }
                if (entityData.Flags != beforeFlags) changed = true;

                if (changed) action.AddEntity(entity, entityData);
            }

            return action.IsEmpty() ? null : action;
        }

        private IAction GetUpdateVisgroupsAction()
        {
            var states = VisgroupPanel.GetAllCheckStates();
            var add = states.Where(x => x.Value == CheckState.Checked).Select(x => x.Key).ToList();
            var rem = states.Where(x => x.Value == CheckState.Unchecked).Select(x => x.Key).ToList();
            // If all the objects are in the add groups and none are in the remove groups, nothing needs to be changed
            if (Objects.All(x => add.All(x.IsInVisgroup) && !rem.Any(x.IsInVisgroup))) return null;
            return new EditObjectVisgroups(Objects, add, rem);
        }

        public void Notify(string message, object data)
        {
            if (message == EditorMediator.SelectionChanged.ToString()
                || message == EditorMediator.SelectionTypeChanged.ToString())
            {
                UpdateObjects();
            }

            if (message == EditorMediator.EntityDataChanged.ToString())
            {
                RefreshData();
            }

            if (message == EditorMediator.VisgroupsChanged.ToString())
            {
                UpdateVisgroups(true);
            }
        }

        public void SetObjects(IEnumerable<MapObject> objects)
        {
            Objects.Clear();
            Objects.AddRange(objects);
            RefreshData();
        }

        private void UpdateObjects()
        {
            if (!FollowSelection)
            {
                UpdateKeyValues();
                UpdateVisgroups(false);
                return;
            }
            Objects.Clear();
            if (!Document.Selection.InFaceSelection)
            {
                Objects.AddRange(Document.Selection.GetSelectedParents());
            }
            RefreshData();
        }

        private void EditVisgroupsClicked(object sender, EventArgs e)
        {
            Mediator.Publish(EditorMediator.VisgroupShowEditor);
        }

        private void UpdateVisgroups(bool retainCheckStates)
        {
            _populating = true;

            var visgroups = Document.Map.Visgroups.Select(x => x.Clone()).ToList();

            Action<Visgroup> setVisible = null;
            setVisible = x =>
                             {
                                 x.Visible = false;
                                 x.Children.ForEach(y => setVisible(y));
                             };
            visgroups.ForEach(x => setVisible(x));

            Dictionary<int, CheckState> states;

            if (retainCheckStates)
            {
                 states = VisgroupPanel.GetAllCheckStates();
            }
            else
            {
                states = Objects.SelectMany(x => x.Visgroups)
                    .GroupBy(x => x)
                    .Select(x => new {ID = x.Key, Count = x.Count()})
                    .Where(g => g.Count > 0)
                    .ToDictionary(g => g.ID, g => g.Count == Objects.Count
                                                      ? CheckState.Checked
                                                      : CheckState.Indeterminate);
            }

            VisgroupPanel.Update(visgroups);

            foreach (var kv in states)
            {
                VisgroupPanel.SetCheckState(kv.Key, kv.Value);
            }

            VisgroupPanel.ExpandAllNodes();

            _populating = false;
        }

        protected override void OnLoad(EventArgs e)
        {
            UpdateObjects();

            Mediator.Subscribe(EditorMediator.SelectionChanged, this);
            Mediator.Subscribe(EditorMediator.SelectionTypeChanged, this);

            Mediator.Subscribe(EditorMediator.EntityDataChanged, this);
            Mediator.Subscribe(EditorMediator.VisgroupsChanged, this);
        }

        protected override void OnClosed(EventArgs e)
        {
            Mediator.UnsubscribeAll(this);
            base.OnClosed(e);
        }
        
        private void RefreshData()
        {
            if (!Objects.Any())
            {
                Tabs.TabPages.Clear();
                return;
            }

            UpdateVisgroups(false);

            var beforeTabs = Tabs.TabPages.OfType<TabPage>().ToArray();

            if (!Tabs.TabPages.Contains(VisgroupTab)) Tabs.TabPages.Add(VisgroupTab);

            if (!Objects.All(x => x is Entity || x is World))
            {
                Tabs.TabPages.Remove(ClassInfoTab);
                Tabs.TabPages.Remove(InputsTab);
                Tabs.TabPages.Remove(OutputsTab);
                Tabs.TabPages.Remove(FlagsTab);
                return;
            }

            if (!Tabs.TabPages.Contains(ClassInfoTab)) Tabs.TabPages.Insert(0, ClassInfoTab);
            if (!Tabs.TabPages.Contains(FlagsTab)) Tabs.TabPages.Insert(Tabs.TabPages.Count - 1, FlagsTab);

            if (Document.Game.EngineID == 1)
            {
                // Goldsource
                Tabs.TabPages.Remove(InputsTab);
                Tabs.TabPages.Remove(OutputsTab);
            }
            else
            {
                // Source
                if (!Tabs.TabPages.Contains(InputsTab)) Tabs.TabPages.Insert(1, InputsTab);
                if (!Tabs.TabPages.Contains(OutputsTab)) Tabs.TabPages.Insert(2, OutputsTab);
            }

            var afterTabs = Tabs.TabPages.OfType<TabPage>().ToArray();

            // If the tabs changed, we want to reset to the first tab
            if (beforeTabs.Length != afterTabs.Length || beforeTabs.Except(afterTabs).Any())
            {
                Tabs.SelectedIndex = 0;
            }

            _populating = true;
            Class.Items.Clear();
            var allowWorldspawn = Objects.Any(x => x is World);
            Class.Items.AddRange(Document.GameData.Classes
                                     .Where(x => x.ClassType != ClassType.Base && (allowWorldspawn || x.Name != "worldspawn"))
                                     .Select(x => x.Name).OrderBy(x => x.ToLower()).OfType<object>().ToArray());
            if (!Objects.Any()) return;
            var classes = Objects.Where(x => x is Entity || x is World).Select(x => x.GetEntityData().Name.ToLower()).Distinct().ToList();
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
            _values = TableValue.Create(Document.GameData, cls, Objects.Where(x => x is Entity || x is World).SelectMany(x => x.GetEntityData().Properties).Where(x => x.Key != "spawnflags").ToList());
            _prevClass = cls;
            PopulateFlags(cls, Objects.Where(x => x is Entity || x is World).Select(x => x.GetEntityData().Flags).ToList());
            _populating = false;

            UpdateKeyValues();
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

            FlagsTable.Enabled = OkButton.Enabled = false;
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
            FlagsTable.Enabled = OkButton.Enabled = true;
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
            FlagsTable.Enabled = OkButton.Enabled = true;
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

        #endregion

        private void ApplyButtonClicked(object sender, EventArgs e)
        {
            Apply();
        }

        private void CancelButtonClicked(object sender, EventArgs e)
        {
            Close();
        }

        private void OkButtonClicked(object sender, EventArgs e)
        {
            Apply();
            Close();
        }
    }
}
