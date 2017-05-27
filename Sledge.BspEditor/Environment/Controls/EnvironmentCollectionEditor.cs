using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Sledge.Common.Shell.Settings;

namespace Sledge.BspEditor.Environment.Controls
{
    public partial class EnvironmentCollectionEditor : UserControl, ISettingEditor
    {
        private readonly IEnumerable<IEnvironmentFactory> _factories;
        private EnvironmentCollection _value;
        public event EventHandler<SettingKey> OnValueChanged;

        public string Label { get; set; }

        public object Value
        {
            get => _value;
            set
            {
                _value = value as EnvironmentCollection;
                UpdateTreeNodes();
            }
        }

        public object Control => this;

        public SettingKey Key { get; set; }

        public EnvironmentCollectionEditor(IEnumerable<IEnvironmentFactory> factories)
        {
            _factories = factories;
            InitializeComponent();
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom;
        }

        private void UpdateTreeNodes()
        {
            treEnvironments.Nodes.Clear();
            if (_value == null) return;

            foreach (var g in _value.GroupBy(x => x.Type))
            {
                var groupNode = new TreeNode(g.Key);
                foreach (var se in g)
                {
                    var envNode = new TreeNode(se.Name) { Tag = se };
                    groupNode.Nodes.Add(envNode);
                }
                treEnvironments.Nodes.Add(groupNode);
            }
            treEnvironments.ExpandAll();
        }

        private void AddEnvironment(object sender, EventArgs e)
        {

        }

        private void RemoveEnvironment(object sender, EventArgs e)
        {

        }

        private IEnvironmentEditor _currentEditor = null;
        private TreeNode _lastSelected = null;

        private void EnvironmentSelected(object sender, TreeViewEventArgs e)
        {
            if (_lastSelected != null) _lastSelected.BackColor = Color.Transparent;
            if (_currentEditor != null) _currentEditor.EnvironmentChanged -= UpdateEnvironment;

            _lastSelected = null;
            _currentEditor = null;
            pnlSettings.Controls.Clear();

            var node = e.Node?.Tag as SerialisedEnvironment;
            if (node != null)
            {
                e.Node.BackColor = Color.CornflowerBlue;
                _lastSelected = e.Node;

                var factory = _factories.FirstOrDefault(x => x.TypeName == node.Type);
                if (factory != null)
                {
                    var des = factory.Deserialise(node);
                    _currentEditor = factory.CreateEditor();
                    pnlSettings.Controls.Add(_currentEditor.Control);
                    _currentEditor.Environment = des;
                    _currentEditor.EnvironmentChanged += UpdateEnvironment;
                }
            }
        }

        private void UpdateEnvironment(object sender, EventArgs e)
        {
            var node = treEnvironments.SelectedNode?.Tag as SerialisedEnvironment;
            if (node != null && _currentEditor != null)
            {
                var factory = _factories.FirstOrDefault(x => x.TypeName == node.Type);
                if (factory != null)
                {
                    var ser = factory.Serialise(_currentEditor.Environment);
                    node.Properties = ser.Properties;
                }
                OnValueChanged?.Invoke(this, Key);
            }
        }
    }
}
