using Sledge.Gui.Interfaces;

namespace Sledge.Gui.Controls
{
    public class TreeView : ControlBase<ITreeView>, ITreeView
    {
        public ITreeModel Model
        {
            get { return Control.Model; }
            set { Control.Model = value; }
        }

        public bool ShowCheckboxes
        {
            get { return Control.ShowCheckboxes; }
            set { Control.ShowCheckboxes = value; }
        }
    }
}