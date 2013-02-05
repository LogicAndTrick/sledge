using System.Collections.Generic;
using System.Windows.Forms;

namespace Sledge.Editor.Menu
{
    public interface IMenuBuilder
    {
        IEnumerable<ToolStripItem> Build();
    }
}