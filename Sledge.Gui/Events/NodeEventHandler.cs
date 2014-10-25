using System.Collections.Generic;
using Sledge.Gui.Interfaces.Models;

namespace Sledge.Gui.Events
{
    public delegate void NodeEventHandler(object sender, List<ITreeNode> nodes);
}