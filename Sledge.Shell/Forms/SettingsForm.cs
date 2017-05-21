using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Settings;
using Sledge.Common.Translations;
using Sledge.Shell.Settings.Editors;

namespace Sledge.Shell.Forms
{
    [Export(typeof(IDialog))]
    [AutoTranslate]
    public partial class SettingsForm : Form, IDialog
    {
        [ImportMany] private IEnumerable<Lazy<ISettingEditorFactory>> _editorFactories;
        [ImportMany] private IEnumerable<Lazy<ISettingsContainer>> _settingsContainers;
        [Import("Shell", typeof(Form))] private Lazy<Form> _parent;

        private Dictionary<ISettingsContainer, List<SettingKey>> _keys;
        private Dictionary<ISettingsContainer, JsonSettingsStore> _values;

        public string Title
        {
            get => Text;
            set => this.Invoke(() => Text = value);
        }

        public string OK
        {
            get => OKButton.Text;
            set => this.Invoke(() => OKButton.Text = value);
        }

        public string Cancel
        {
            get => CancelButton.Text;
            set => this.Invoke(() => CancelButton.Text = value);
        }
        
        public SettingsForm()
        {
            InitializeComponent();
            CreateHandle();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            if (Visible)
            {
                _keys = _settingsContainers.ToDictionary(x => x.Value, x => x.Value.GetKeys().ToList());
                _values = _settingsContainers.ToDictionary(x => x.Value, x =>
                {
                    var fss = new JsonSettingsStore();
                    x.Value.StoreValues(fss);
                    return fss;
                });
                LoadGroupList();
            }
            base.OnVisibleChanged(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            Oy.Publish("Context:Remove", new ContextInfo("SettingsForm"));
        }

        private void LoadGroupList()
        {
            GroupList.BeginUpdate();

            GroupList.Items.Clear();
            foreach (var k in _keys.SelectMany(x => x.Value).GroupBy(x => x.Group))
            {
                GroupList.Items.Add(k.Key);
            }

            GroupList.EndUpdate();
        }
        
        private List<ISettingEditor> _editors = new List<ISettingEditor>();

        private void LoadEditorList()
        {
            _editors.ForEach(x => x.OnValueChanged -= OnValueChanged);
            _editors.Clear();

            SettingsPanel.SuspendLayout();
            SettingsPanel.Controls.Clear();

            SettingsPanel.AutoSize = true;
            SettingsPanel.RowStyles.Clear();

            var group = GroupList.SelectedItem as string;
            if (group != null)
            {
                foreach (var kv in _keys)
                {
                    var container = kv.Key;
                    var keys = kv.Value.Where(x => x.Group == group);
                    var values = _values[container];
                    foreach (var key in keys)
                    {
                        var editor = GetEditor(key);
                        editor.Key = key;
                        editor.Label = key.Key;
                        editor.Value = values.Get(key.Type, key.Key);

                        if (SettingsPanel.Controls.Count > 0)
                        {
                            // Add a separator
                            var line = new Label
                            {
                                Height = 1,
                                BackColor = Color.FromArgb(128, Color.Black),
                                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
                            };
                            SettingsPanel.Controls.Add(line);
                        }

                        var ctrl = (Control) editor.Control;
                        ctrl.Anchor |= AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                        SettingsPanel.Controls.Add(ctrl);

                        if (ctrl.Anchor.HasFlag(AnchorStyles.Bottom))
                        {
                            SettingsPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
                            SettingsPanel.AutoSize = false;
                        }

                        _editors.Add(editor);
                    }
                }
            }

            SettingsPanel.ResumeLayout();

            _editors.ForEach(x => x.OnValueChanged += OnValueChanged);
        }

        private void OnValueChanged(object sender, SettingKey key)
        {
            var se = sender as ISettingEditor;
            var store = _values.Where(x => x.Value.Contains(key.Key)).Select(x => x.Value).FirstOrDefault();
            if (store != null && se != null)
            {
                store.Set(key.Key, se.Value);
            }
        }

        private ISettingEditor GetEditor(SettingKey key)
        {
            foreach (var ef in _editorFactories)
            {
                if (ef.Value.Supports(key)) return ef.Value.CreateEditorFor(key);
            }
            return new DefaultSettingEditor();
        }

        public bool IsInContext(IContext context)
        {
            return context.HasAny("SettingsForm");
        }

        public void SetVisible(bool visible)
        {
            this.Invoke(() =>
            {
                if (visible)
                {
                    Show(_parent.Value);
                }
                else
                {
                    Hide();
                }
            });
        }

        private void GroupListSelectionChanged(object sender, EventArgs e)
        {
            LoadEditorList();
        }

        private void OkClicked(object sender, EventArgs e)
        {
            foreach (var kv in _values)
            {
                kv.Key.LoadValues(kv.Value);
            }
            Oy.Publish("SettingsChanged", new object());
            Close();
        }

        private void CancelClicked(object sender, EventArgs e)
        {
            Close();
        }
    }
}
