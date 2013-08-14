using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Sledge.Editor.Menu
{
    public class GroupedMenuBuilder : IMenuBuilder, IEnumerable<IMenuBuilder>
    {
        public string Name { get; set; }
        public Func<bool> IsVisible { get; set; }
        public List<IMenuBuilder> SubMenus { get; set; }
        public Image Image { get; set; }

        public bool ShowInMenu { get { return true; } }
        public bool ShowInToolStrip { get { return false; } }

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
            //if (IsVisible != null && !IsVisible()) yield break;
            var mi = new ToolStripMenuItem(Name) {Image = Image};
            mi.DropDownItems.AddRange(SubMenus.SelectMany(x => x.Build()).ToArray());
            yield return mi;
        }

        public IEnumerable<ToolStripItem> BuildToolStrip()
        {
            throw new NotImplementedException();
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