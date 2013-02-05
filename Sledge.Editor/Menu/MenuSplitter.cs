using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Sledge.Editor.Menu
{
    public class MenuSplitter : IMenuBuilder
    {
        public Func<bool> IsVisible { get; set; }

        public IEnumerable<ToolStripItem> Build()
        {
            if (IsVisible != null && !IsVisible()) yield break;
            yield return new ToolStripSeparator();
        }
    }
}