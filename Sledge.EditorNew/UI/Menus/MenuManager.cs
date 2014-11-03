using System;
using System.Collections.Generic;
using Sledge.Gui;

namespace Sledge.EditorNew.UI.Menus
{
    public static class MenuManager
    {
        public static readonly List<IMenuGroup> MenuGroups = new List<IMenuGroup>();

        static MenuManager()
        {
            UIManager.Manager.Shell.AddMenu();
            UIManager.Manager.Shell.AddToolbar();
        }

        public static void Add(IMenuGroup menuGroup)
        {
            MenuGroups.Add(menuGroup);
        }

        public static void Build()
        {
            foreach (var g in MenuGroups)
            {
                var group = GetGroup(g.Path);
                if (group.SubItems.Count > 0)
                {
                    group.AddSeparator();
                }
                foreach (var menuItem in g.GetMenuItems())
                {
                    if (menuItem.ShowInMenu)
                    {
                        var mi = group.AddSubMenuItem(menuItem.TextKey, menuItem.Text);
                        mi.Icon = menuItem.Image;
                        mi.Clicked += TriggerMenuAction(menuItem);
                        mi.IsActive = menuItem.IsActive;
                    }
                    if (menuItem.ShowInToolstrip)
                    {
                        var ti = UIManager.Manager.Shell.Toolbar.AddToolbarItem(menuItem.TextKey, menuItem.Text);
                        ti.Icon = menuItem.Image;
                        ti.Clicked += TriggerMenuAction(menuItem);
                        ti.IsActive = menuItem.IsActive;
                    }
                }
            }
        }

        private static readonly Dictionary<string, Gui.Interfaces.Shell.IMenuItem> GroupItems = new Dictionary<string, Gui.Interfaces.Shell.IMenuItem>();

        private static Gui.Interfaces.Shell.IMenuItem GetGroup(string group)
        {
            if (!GroupItems.ContainsKey(group))
            {
                var spl = group.Split('.');
                var g = "";
                Gui.Interfaces.Shell.IMenuItem parent = null;
                foreach (var s in spl)
                {
                    if (g != "") g += ".";
                    g += s;
                    var id = "MenuGroup/" + g;
                    if (!GroupItems.ContainsKey(g))
                    {
                        if (parent == null) GroupItems.Add(g, UIManager.Manager.Shell.Menu.AddMenuItem(id));
                        else GroupItems.Add(g, parent.AddSubMenuItem(id));
                    }
                    parent = GroupItems[g];
                }
            }
            return GroupItems[group];
        }

        private static EventHandler TriggerMenuAction(IMenuItem menuItem)
        {
            return (sender, eventArgs) => menuItem.Execute();
        }
    }
}
