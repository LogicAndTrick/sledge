using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Context;
using Sledge.Common.Translations;
using Sledge.Shell;

namespace Sledge.BspEditor.Editing.Components
{
    [Export(typeof(IDialog))]
    [AutoTranslate]
    public partial class SelectionDetailsDialog : Form, IDialog, IManualTranslate
    {
        [Import("Shell", typeof(Form))] private Lazy<Form> _parent;
        [Import] private IContext _context;

        private List<Subscription> _subscriptions;

        public SelectionDetailsDialog()
        {
            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            Oy.Publish("Context:Remove", new ContextInfo("BspEditor:SelectionDetails"));
        }

        public bool IsInContext(IContext context)
        {
            return context.HasAny("BspEditor:SelectionDetails");
        }

        public void SetVisible(IContext context, bool visible)
        {
            this.InvokeLater(() =>
            {
                if (visible)
                {
                    if (!Visible) Show(_parent.Value);
                    Subscribe();
                    UpdateTree();
                }
                else
                {
                    Hide();
                    Unsubscribe();
                }
            });
        }

        public void Translate(ITranslationStringProvider strings)
        {
            CreateHandle();
            var prefix = GetType().FullName;
            this.InvokeLater(() =>
            {
                Text = strings.GetString(prefix, "Title");
            });
        }

        private void Subscribe()
        {
            if (_subscriptions != null) return;
            _subscriptions = new List<Subscription>
            {
                Oy.Subscribe<Change>("MapDocument:Changed", DocumentChanged),
                Oy.Subscribe<MapDocument>("Document:Activated", DocumentActivated),
                Oy.Subscribe<MapDocument>("MapDocument:SelectionChanged", SelectionChanged)
            };
        }

        private void Unsubscribe()
        {
            if (_subscriptions == null) return;
            _subscriptions.ForEach(x => x.Dispose());
            _subscriptions = null;
        }

        public async Task DocumentActivated(MapDocument document)
        {
            UpdateTree();
        }

        public async Task SelectionChanged(MapDocument document)
        {
            UpdateTree();
        }

        private async Task DocumentChanged(Change change)
        {
            UpdateTree();
        }

        private void UpdateTree()
        {
            this.InvokeLater(() =>
            {
                SelectionTree.BeginUpdate();

                SelectionTree.Nodes.Clear();

                var doc = _context.Get<MapDocument>("ActiveDocument");
                if (doc != null)
                {
                    foreach (var root in doc.Selection.GetSelectedParents())
                    {
                        SelectionTree.Nodes.Add(CreateNode(root));
                    }
                }

                SelectionTree.ExpandAll();
                SelectionTree.EndUpdate();
            });
        }

        private TreeNode CreateNode(IMapObject obj)
        {
            var node = new TreeNode(GetNodeText(obj));

            foreach (var child in obj.Hierarchy)
            {
                var c = CreateNode(child);
                node.Nodes.Add(c);
            }

            return node;
        }

        private string GetNodeText(IMapObject obj)
        {
            var text = obj.ID + " - " + obj.GetType().Name;

            var ed = obj.Data.GetOne<EntityData>();
            if (ed != null)
            {
                if (!String.IsNullOrWhiteSpace(ed.Name)) text += " - " + ed.Name;
                var tn = ed.Get<string>("targetname") ?? ed.Get<string>("name");
                if (!String.IsNullOrWhiteSpace(tn)) text += " - " + tn;
            }

            return text;
        }
    }
}
