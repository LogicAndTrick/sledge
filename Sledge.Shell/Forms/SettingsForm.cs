using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Data;
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
        private Dictionary<ISettingsContainer, FakeSettingsStore> _values;

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
                    var fss = new FakeSettingsStore();
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

        private void LoadEditorList()
        {
            SettingsPanel.SuspendLayout();
            SettingsPanel.Controls.Clear();
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
                        editor.Label = key.Key;
                        editor.Value = values.Get<object>(key.Key);
                        var ctrl = (Control) editor.Control;
                        ctrl.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                        SettingsPanel.Controls.Add(ctrl);
                    }
                }
            }

            SettingsPanel.ResumeLayout();
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

        private class FakeSettingsStore : ISettingsStore
        {
            public Dictionary<string, object> Values { get; set; }

            public FakeSettingsStore()
            {
                Values = new Dictionary<string, object>();
            }

            public IEnumerable<string> GetKeys()
            {
                return Values.Keys;
            }

            public bool Contains(string key)
            {
                return Values.ContainsKey(key);
            }

            public object Get(Type type, string key, object defaultValue = null)
            {
                return Values.ContainsKey(key) ? Convert.ChangeType(Values[key], type) : defaultValue;
            }

            public T Get<T>(string key, T defaultValue = default(T))
            {
                return (T) Get(typeof(T), key, defaultValue);
            }

            public void Set<T>(string key, T value)
            {
                Values[key] = value;
            }

            public void Delete(string key)
            {
                if (Values.ContainsKey(key)) Values.Remove(key);
            }
        }
    }
}
