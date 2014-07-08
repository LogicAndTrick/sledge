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
using Sledge.Editor.UI.ObjectProperties.SmartEdit;
using Sledge.QuickForms;
using Sledge.Settings.Models;

namespace Sledge.Editor.UI.ObjectProperties
{
    public partial class ObjectPropertiesDialog : Form, IMediatorListener
    {
        private static int _numOpen = 0;
        public static bool IsShowing { get { return _numOpen > 0; } }

        private List<TableValue> _values;

        private readonly Dictionary<VariableType, SmartEditControl> _smartEditControls;
        private readonly SmartEditControl _dumbEditControl;
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

        public ObjectPropertiesDialog(Documents.Document document)
        {
            Document = document;
            InitializeComponent();
            Objects = new List<MapObject>();
            _smartEditControls = new Dictionary<VariableType, SmartEditControl>();

            _dumbEditControl = new DumbEditControl {Document = Document};
            _dumbEditControl.ValueChanged += PropertyValueChanged;
            _dumbEditControl.NameChanged += PropertyNameChanged;

            RegisterSmartEditControls();

            FollowSelection = true;
        }

        private void RegisterSmartEditControls()
        {
            var types = typeof(SmartEditControl).Assembly.GetTypes()
                .Where(x => typeof(SmartEditControl).IsAssignableFrom(x))
                .Where(x => x != typeof(SmartEditControl))
                .Where(x => x.GetCustomAttributes(typeof(SmartEditAttribute), false).Any());
            foreach (var type in types)
            {
                var attrs = type.GetCustomAttributes(typeof (SmartEditAttribute), false);
                foreach (SmartEditAttribute attr in attrs)
                {
                    var inst = (SmartEditControl) Activator.CreateInstance(type);

                    inst.Document = Document;
                    inst.ValueChanged += PropertyValueChanged;
                    inst.Dock = DockStyle.Fill;

                    _smartEditControls.Add(attr.VariableType, inst);
                }
            }
        }

        private void Apply()
        {
            string actionText = null;
            var ac = new ActionCollection();
            
            
            // Check if it's actually editing keyvalues
            if (_values != null)
            {
                var editAction = GetEditEntityDataAction();
                if (editAction != null)
                {
                    // The entity change is more important to show
                    actionText = "Edit entity data";
                    ac.Add(editAction);
                }
            }

            var visgroupAction = GetUpdateVisgroupsAction();
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
                var nonExistant = entityData.Properties.Where(x => _values.All(y => y.OriginalKey != x.Key));
                if (nonExistant.Any())
                {
                    changed = true;
                    entityData.Properties.RemoveAll(x => _values.All(y => y.OriginalKey != x.Key));
                }

                // Set updated/new properties
                foreach (var ent in _values.Where(x => x.IsModified || (x.IsAdded && !x.IsRemoved)))
                {
                    entityData.SetPropertyValue(ent.OriginalKey, ent.Value);
                    if (!String.IsNullOrWhiteSpace(ent.NewKey) && ent.NewKey != ent.OriginalKey)
                    {
                        var prop = entityData.Properties.FirstOrDefault(x => String.Equals(x.Key, ent.OriginalKey, StringComparison.InvariantCultureIgnoreCase));
                        if (prop != null && !entityData.Properties.Any(x => String.Equals(x.Key, ent.NewKey, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            prop.Key = ent.NewKey;
                        }
                    }
                    changed = true;
                }

                foreach (var ent in _values.Where(x => x.IsRemoved && !x.IsAdded))
                {
                    entityData.Properties.RemoveAll(x => x.Key == ent.OriginalKey);
                    changed = true;
                }

                // Set flags
                var flags = Enumerable.Range(0, FlagsTable.Items.Count).Select(x => FlagsTable.GetItemCheckState(x)).ToList();
                var entClass = Document.GameData.Classes.FirstOrDefault(x => x.Name == entityData.Name);
                var spawnFlags = entClass == null
                                     ? null
                                     : entClass.Properties.FirstOrDefault(x => x.Name == "spawnflags");
                var opts = spawnFlags == null ? null : spawnFlags.Options.OrderBy(x => int.Parse(x.Key)).ToList();
                if (opts != null && flags.Count == opts.Count)
                {
                    var beforeFlags = entityData.Flags;
                    for (var i = 0; i < flags.Count; i++)
                    {
                        var val = int.Parse(opts[i].Key);
                        if (flags[i] == CheckState.Unchecked) entityData.Flags &= ~val; // Switch the flag off if unchecked
                        else if (flags[i] == CheckState.Checked) entityData.Flags |= val; // Switch it on if checked
                        // No change if indeterminate
                    }
                    if (entityData.Flags != beforeFlags) changed = true;
                }

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
            if (Objects.All(x => add.All(y => x.IsInVisgroup(y, false)) && !rem.Any(y => x.IsInVisgroup(y, false)))) return null;
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
            _numOpen += 1;
            UpdateObjects();

            Mediator.Subscribe(EditorMediator.SelectionChanged, this);
            Mediator.Subscribe(EditorMediator.SelectionTypeChanged, this);

            Mediator.Subscribe(EditorMediator.EntityDataChanged, this);
            Mediator.Subscribe(EditorMediator.VisgroupsChanged, this);
        }

        protected override void OnClosed(EventArgs e)
        {
            _numOpen -= 1;
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

            if (Document.Game.Engine == Engine.Goldsource)
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
                var dt = smartEdit ? tv.DisplayText(Document.GameData) : tv.OriginalKey;
                var dv = smartEdit ? tv.DisplayValue(Document.GameData) : tv.Value;
                KeyValuesList.Items.Add(new ListViewItem(dt) { Tag = tv.OriginalKey, BackColor = tv.GetColour() }).SubItems.Add(dv);
            }

            Angles.Enabled = false;
            var angleVal = _values.FirstOrDefault(x => x.OriginalKey == "angles");
            if (angleVal != null)
            {
                Angles.Enabled = !_changingClass;
                Angles.SetAnglePropertyString(angleVal.Value);
            }

            if (selectedIndex >= 0 && KeyValuesList.Items.Count > selectedIndex) KeyValuesList.SelectedIndices.Add(selectedIndex);
            else KeyValuesListSelectedIndexChanged(null, null);

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
                var prop = props.FirstOrDefault(x => x.Name == tv.OriginalKey);
                tv.IsRemoved = prop == null;
            }

            // Add the new properties that aren't in the new class as 'added'
            foreach (var prop in props.Where(x => x.Name != "spawnflags" && _values.All(y => y.OriginalKey != x.Name)))
            {
                _values.Add(new TableValue { OriginalKey = prop.Name, NewKey = prop.Name, IsAdded = true, Value = prop.DefaultValue });
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
            var val = _values.FirstOrDefault(x => x.OriginalKey == propertyname);
            var li = KeyValuesList.Items.OfType<ListViewItem>().FirstOrDefault(x => ((string) x.Tag) == propertyname);
            if (val == null)
            {
                if (li != null) KeyValuesList.Items.Remove(li);
                return;
            }
            val.IsModified = true;
            val.Value = propertyvalue;
            if (li == null)
            {
                var dt = SmartEditButton.Checked ? val.DisplayText(Document.GameData) : val.OriginalKey;
                var dv = SmartEditButton.Checked ? val.DisplayValue(Document.GameData) : val.Value;
                li = new ListViewItem(dt) { Tag = val.OriginalKey, BackColor = val.GetColour() };
                KeyValuesList.Items.Add(li).SubItems.Add(dv);
            }
            else
            {
                li.BackColor = val.GetColour();
                li.SubItems[1].Text = SmartEditButton.Checked ? val.DisplayValue(Document.GameData) : val.Value;
            }
            if (propertyname == "angles" && propertyvalue != Angles.GetAnglePropertyString())
            {
                Angles.SetAnglePropertyString(propertyvalue);
            }
        }

        private void PropertyNameChanged(object sender, string oldName, string newName)
        {
            var val = _values.FirstOrDefault(x => x.OriginalKey == oldName);
            if (val == null)
            {
                return;
            }
            val.IsModified = true;
            val.NewKey = newName;
            var li = KeyValuesList.Items.OfType<ListViewItem>().FirstOrDefault(x => ((string)x.Tag) == oldName);
            if (li != null)
            {
                li.BackColor = val.GetColour();
                li.SubItems[0].Text = SmartEditButton.Checked ? val.DisplayText(Document.GameData) : val.NewKey;
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
                ((SmartEditControl) SmartEditControlPanel.Controls[0]).SetProperty("angles", "angles", Angles.GetAnglePropertyString(), null);
            }
        }

        private void KeyValuesListSelectedIndexChanged(object sender, EventArgs e)
        {
            HelpTextbox.Text = "";
            CommentsTextbox.Text = "";
            ClearSmartEditControls();
            if (KeyValuesList.SelectedItems.Count == 0 || _changingClass) return;
            var smartEdit = SmartEditButton.Checked;
            var className = Class.Text;
            var selected = KeyValuesList.SelectedItems[0];
            var originalName = (string)selected.Tag;
            var value = selected.SubItems[1].Text;
            var cls = Document.GameData.Classes.FirstOrDefault(x => x.Name == className);
            var prop = _values.FirstOrDefault(x => x.OriginalKey == originalName);
            var gdProp = smartEdit && cls != null && prop != null ? cls.Properties.FirstOrDefault(x => x.Name == prop.NewKey) : null;
            if (gdProp != null)
            {
                HelpTextbox.Text = gdProp.Description;
            }
            AddSmartEditControl(gdProp, originalName, value);
        }

        private void AddPropertyClicked(object sender, EventArgs e)
        {
            if (_changingClass) return;

            using (var qf = new QuickForm("Add Property") { UseShortcutKeys = true }.TextBox("Key").TextBox("Value").OkCancel())
            {
                if (qf.ShowDialog(this) != DialogResult.OK) return;

                var name = qf.String("Key");
                var newName = name;
                var num = 1;
                while (_values.Any(x => String.Equals(x.OriginalKey, newName, StringComparison.InvariantCultureIgnoreCase)))
                {
                    newName = name + "#" + (num++);
                }

                _values.Add(new TableValue
                {
                    Class = Class.Text,
                    OriginalKey = newName,
                    NewKey = newName,
                    Value = qf.String("Value"),
                    IsAdded = true,
                    IsModified = true,
                    IsRemoved = false
                });
                PropertyValueChanged(this, newName, qf.String("Value"));
            }
        }

        private void RemovePropertyClicked(object sender, EventArgs e)
        {
            if (KeyValuesList.SelectedItems.Count == 0 || _changingClass) return;
            var selected = KeyValuesList.SelectedItems[0];
            var propName = (string)selected.Tag;
            var val = _values.FirstOrDefault(x => x.OriginalKey == propName);
            if (val != null)
            {
                if (val.IsAdded)
                {
                    _values.Remove(val);
                }
                else
                {
                    val.IsRemoved = true;
                }
                PropertyValueChanged(this, val.OriginalKey, val.Value);
            }
        }

        private void ClearSmartEditControls()
        {
            foreach (var c in _smartEditControls)
            {
                c.Value.EditingEntityData = null;
            }
            _dumbEditControl.EditingEntityData = null;
            SmartEditControlPanel.Controls.Clear();
        }

        private void AddSmartEditControl(DataStructures.GameData.Property property, string propertyName, string value)
        {
            ClearSmartEditControls();
            var ctrl = _dumbEditControl;
            if (property != null && _smartEditControls.ContainsKey(property.VariableType))
            {
                ctrl = _smartEditControls[property.VariableType];
            }
            var prop = _values.FirstOrDefault(x => x.OriginalKey == propertyName);
            ctrl.EditingEntityData = Objects.Select(x => x.GetEntityData()).Where(x => x != null).ToList();
            ctrl.SetProperty(propertyName, prop == null ? propertyName : prop.NewKey, value, property);
            SmartEditControlPanel.Controls.Add(ctrl);
        }

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
