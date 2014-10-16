using System.Collections.Generic;

namespace Sledge.Gui.Interfaces
{
    public interface ITreeModel
    {
        IEnumerable<ITreeNode> GetRootNodes();
        IEnumerable<ITreeNode> GetChildNodes(ITreeNode parent);
        ITreeNode GetParentNode(ITreeNode child);

        void AddRootNode(ITreeNode node);
        void AddChildNode(ITreeNode parent, ITreeNode node);

        void RemoveNode(ITreeNode node);
        void Clear();

        event NodeEventHandler NodesAdded;
        event NodeEventHandler NodesRemoved;
        event NodeEventHandler NodesToggled;
        event NodeEventHandler NodesSelected;
    }
}