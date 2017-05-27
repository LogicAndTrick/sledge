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
        private readonly List<IEnvironmentFactory> _factories;
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

        private Label _nameLabel;
        private TextBox _nameBox;

        public EnvironmentCollectionEditor(IEnumerable<IEnvironmentFactory> factories)
        {
            _factories = factories.ToList();
            InitializeComponent();
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom;

            _nameLabel = new Label {Text = "Name", Padding = new Padding(0, 6, 0, 0), AutoSize = true};
            _nameBox = new TextBox{Width = 250};
            _nameBox.TextChanged += UpdateEnvironment;

            if (_factories.Any())
            {
                ctxEnvironmentMenu.Items.Clear();
                foreach (var ef in _factories)
                {
                    var mi = new ToolStripMenuItem(ef.Description) { Tag = ef };
                    mi.Click += AddEnvironment;
                    ctxEnvironmentMenu.Items.Add(mi);
                }
            }
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
            var factory = (sender as ToolStripItem)?.Tag as IEnvironmentFactory;
            if (factory != null && _value != null)
            {
                var newEnv = new SerialisedEnvironment
                {
                    Name = "New Environment",
                    Type = factory.TypeName
                };
                _value.Add(newEnv);
                UpdateTreeNodes();
                OnValueChanged?.Invoke(this, Key);
            }
        }

        private void RemoveEnvironment(object sender, EventArgs e)
        {
            var node = treEnvironments.SelectedNode?.Tag as SerialisedEnvironment;
            if (node != null && _value != null)
            {
                _value.Remove(node);
                UpdateTreeNodes();
                OnValueChanged?.Invoke(this, Key);
                EnvironmentSelected(null, null);
            }
        }

        private IEnvironmentEditor _currentEditor = null;

        private void EnvironmentSelected(object sender, TreeViewEventArgs e)
        {
            if (_currentEditor != null) _currentEditor.EnvironmentChanged -= UpdateEnvironment;

            _currentEditor = null;
            pnlSettings.Controls.Clear();

            var node = e?.Node?.Tag as SerialisedEnvironment;
            if (node != null)
            {
                var factory = _factories.FirstOrDefault(x => x.TypeName == node.Type);
                if (factory != null)
                {
                    var fp = new FlowLayoutPanel
                    {
                        Height = 30,
                        Width = 400,
                        FlowDirection = FlowDirection.LeftToRight,
                        Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
                    };
                    fp.Controls.Add(_nameLabel);
                    fp.Controls.Add(_nameBox);
                    pnlSettings.Controls.Add(fp);

                    _nameBox.Text = node.Name;

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
                treEnvironments.SelectedNode.Text = _nameBox.Text;
                var factory = _factories.FirstOrDefault(x => x.TypeName == node.Type);
                if (factory != null)
                {
                    var ser = factory.Serialise(_currentEditor.Environment);
                    node.Name = _nameBox.Text;
                    node.Properties = ser.Properties;
                }
                OnValueChanged?.Invoke(this, Key);
            }
        }
    }
}
