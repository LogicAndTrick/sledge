using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Newtonsoft.Json;
using Sledge.BspEditor.Primitives;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Documents;

namespace Sledge.BspEditor.Documents
{
    public class MapDocument : IDocument
    {
        private TreeView _control;
        public event PropertyChangedEventHandler PropertyChanged;
        public string Name { get; }
        public object Control => _control;

        public Map Map { get; set; }

        public MapDocument(Map map)
        {
            Name = "Untitled";
            Map = map;

            Init();
        }

        private void Init()
        {
            _control = new TreeView(); {};

            var root = new TreeNode("Root");
            AddData(root, Map.Root);
            AddChildren(root, Map.Root);
            _control.Nodes.Add(root);
            _control.ExpandAll();

            Oy.Subscribe<IDocument>("Document:RequestClose", IfThis(RequestClose));
        }

        private void AddData(TreeNode treeNode, IMapObject obj)
        {
            foreach (var d in obj.Data.Data)
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

        private Action<IDocument> IfThis(Func<Task> callback)
        {
            return d =>
            {
                if (d == this) callback();
            };
        }

        private async Task RequestClose()
        {
            await Oy.Publish("Document:Closed", this);
        }
    }
}