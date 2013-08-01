using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Sledge.Common.Mediator;

namespace Sledge.Editor.Menu
{
    public class RecentFilesMenu : IMenuBuilder
    {
        public bool ShowInMenu { get { return true; } }
        public bool ShowInToolStrip { get { return false; } }
        public IEnumerable<ToolStripItem> Build()
        {
            if (MenuManager.RecentFiles.Count == 0) yield break;
            yield return new ToolStripSeparator();
            foreach (var rf in MenuManager.RecentFiles.OrderBy(x => x.Order))
            {
                var file = rf.Location;
                var mi = new ToolStripMenuItem(Path.GetFileName(file));
                mi.Click += (sender, e) => Mediator.Publish(EditorMediator.LoadFile, file);
                yield return mi;
            }
        }

        public IEnumerable<ToolStripItem> BuildToolStrip()
        {
            throw new System.NotImplementedException();
        }
    }
}