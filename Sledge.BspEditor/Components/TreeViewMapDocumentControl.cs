using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Newtonsoft.Json;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Shell.Documents;

namespace Sledge.BspEditor.Components
{
    public class TreeViewMapDocumentControl : IMapDocumentControl
    {
        private readonly TreeView _view;

        public bool IsFocused { get; set; }

        public string Type => "TreeView";
        public Control Control => _view;

        private readonly List<Subscription> _subscriptions;

        public TreeViewMapDocumentControl()
        {
            _view = new TreeView() { Dock = DockStyle.Fill };
            _subscriptions = new List<Subscription>
            {
                Oy.Subscribe<IDocument>("Document:Activated", DocumentActivated)
            };
        }

        private Task DocumentActivated(IDocument doc)
        {
            _view.Nodes.Clear();

            if (doc is MapDocument mapDoc)
            {
                _view.Invoke((MethodInvoker)delegate
               {
                   _view.BeginUpdate();
                   var root = new TreeNode("Root");
                   AddData(root, mapDoc.Map.Root);
                   AddChildren(root, mapDoc.Map.Root);
                   _view.Nodes.Add(root);
                   _view.ExpandAll();
                   _view.EndUpdate();
               });
            }

            return Task.CompletedTask;
        }

        public string GetSerialisedSettings()
        {
            return "";
        }

        public void SetSerialisedSettings(string settings)
        {
            //
        }

        private void AddData(TreeNode treeNode, IMapObject obj)
        {
            foreach (var d in obj.Data)
            {
                var node = new TreeNode(d.GetType().Name);
                foreach (var pi in d.GetType().GetProperties().Where(x => x.CanRead))
                {
                    var val = JsonConvert.SerializeObject(pi.GetValue(d));
                    node.Nodes.Add(new TreeNode($"{pi.Name} = {val}"));
                }
                treeNode.Nodes.Add(node);
            }
        }

        private void AddChildren(TreeNode treeNode, IMapObject obj)
        {
            foreach (var child in obj.Hierarchy)
            {
                var node = new TreeNode(child.GetType().Name);
                AddData(node, child);
                AddChildren(node, child);
                treeNode.Nodes.Add(node);
            }
        }

        public void Dispose()
        {
            _subscriptions.ForEach(Oy.Unsubscribe);
            _view.Dispose();
        }
    }
}