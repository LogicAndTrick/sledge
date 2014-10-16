using System.Collections.Generic;
using System.Linq;
using Sledge.Common.Mediator;

namespace Sledge.Gui.Interfaces
{
    public class TreeModel : ITreeModel
    {
        private static readonly ITreeNode Root = new TreeNode();
        private readonly MultiDictionary<ITreeNode, ITreeNode> _nodes;

        public event NodeEventHandler NodesAdded;
        public event NodeEventHandler NodesRemoved;
        public event NodeEventHandler NodesToggled;
        public event NodeEventHandler NodesSelected;

        protected virtual void OnNodesAdded(List<ITreeNode> e)
        {
            var handler = NodesAdded;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnNodesRemoved(List<ITreeNode> e)
        {
            var handler = NodesRemoved;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnNodesToggled(List<ITreeNode> nodes)
        {
            var handler = NodesToggled;
            if (handler != null) handler(this, nodes);
        }

        protected virtual void OnNodesSelected(List<ITreeNode> nodes)
        {
            var handler = NodesSelected;
            if (handler != null) handler(this, nodes);
        }

        public TreeModel()
        {
            _nodes = new MultiDictionary<ITreeNode, ITreeNode>();
        }

        public IEnumerable<ITreeNode> GetRootNodes()
        {
            return GetChildNodes(Root);
        }

        public IEnumerable<ITreeNode> GetChildNodes(ITreeNode parent)
        {
            return _nodes.GetValues(parent);
        }

        public ITreeNode GetParentNode(ITreeNode child)
        {
            var key = _nodes.GetKeyForValue(child);
            return key == Root ? null : key;
        }

        public void AddRootNode(ITreeNode node)
        {
            AddChildNode(Root, node);
        }

        public void AddChildNode(ITreeNode parent, ITreeNode node)
        {
            _nodes.AddValue(parent, node);
            OnNodesAdded(new List<ITreeNode> {node});
        }

        public void RemoveNode(ITreeNode node)
        {
            var key = _nodes.GetKeyForValue(node);
            if (_nodes.RemoveValue(key, node)) OnNodesRemoved(new List<ITreeNode> {node});
        }

        public void Clear()
        {
            var all = _nodes.SelectMany(x => x.Value).ToList();
            _nodes.Clear();
            OnNodesRemoved(all);
        }
    }
}