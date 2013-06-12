using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Sledge.Editor.Menu
{
    public class GroupedMenuBuilder : IMenuBuilder, IEnumerable<IMenuBuilder>
    {
        public string Name { get; set; }
        public Func<bool> IsVisible { get; set; }
        public List<IMenuBuilder> SubMenus { get; set; }

        public GroupedMenuBuilder(string name, params IMenuBuilder[] subMenus)
        {
            Name = name;
            SubMenus = subMenus.ToList();
        }

        public void Add(IMenuBuilder builder)
        {
            SubMenus.Add(builder);
        }

        public IEnumerable<ToolStripItem> Build()
        {
            if (IsVisible != null && !IsVisible()) yield break;
            var mi = new ToolStripMenuItem(Name);
            mi.DropDownItems.AddRange(SubMenus.SelectMany(x => x.Build()).ToArray());
            yield return mi;
        }

        public IEnumerator<IMenuBuilder> GetEnumerator()
        {
            return SubMenus.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}