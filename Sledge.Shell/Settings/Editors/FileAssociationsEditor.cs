using System;
using System.Windows.Forms;
using Sledge.Common.Shell.Settings;
using Sledge.Shell.Registers;

namespace Sledge.Shell.Settings.Editors
{
    public partial class FileAssociationsEditor : UserControl, ISettingEditor
    {
        public event EventHandler<SettingKey> OnValueChanged;

        public string Label { get; set; }
        
        private DocumentRegister.FileAssociations _bindings;

        public object Value
        {
            get => _bindings;
            set
            {
                _bindings = ((DocumentRegister.FileAssociations) value).Clone();
                UpdateAssociationsList();
            }
        }
        
        public object Control => this;
        public SettingKey Key { get; set; }

        public FileAssociationsEditor()
        {
            InitializeComponent();
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom;
        }

        private void UpdateAssociationsList()
        {
            CheckboxPanel.Controls.Clear();
            
            if (_bindings == null) return;

            foreach (var b in _bindings)
            {
                var checkbox = new CheckBox
                {
                    Text = b.Key,
                    Checked = b.Value,
                    Tag = b.Key,
                    Margin = new Padding(2)
                };
                checkbox.CheckedChanged += SetAssociation;
                CheckboxPanel.Controls.Add(checkbox);
            }
        }

        private void SetAssociation(object sender, EventArgs e)
        {
            var assoc = (sender as CheckBox)?.Checked ?? false;
            _bindings[(sender as CheckBox)?.Tag as string ?? ""] = assoc;
            OnValueChanged?.Invoke(this, Key);
        }
    }
}
