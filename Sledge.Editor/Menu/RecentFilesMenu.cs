using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Sledge.Common.Mediator;

namespace Sledge.Editor.Menu
{
    public class RecentFilesMenu : IMenuBuilder
    {
        public IEnumerable<ToolStripItem> Build()
        {
            if (MenuManager.RecentFiles.Count == 0) yield break;
            yield return new ToolStripSeparator();
            foreach (var rf in MenuManager.RecentFiles.OrderBy(x => x.Order))
            {
                var mi = new ToolStripMenuItem(Path.GetFileName(rf.Location));
                mi.Click += (sender, e) => Mediator.Publish("");
                yield return mi;
            }
        }
    }
}