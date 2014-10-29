using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Sledge.EditorNew.Commands;
using Sledge.EditorNew.Language;
using Sledge.Gui;
using Sledge.Gui.Interfaces.Shell;

namespace Sledge.EditorNew.UI.Menus
{
    public static class MenuManager
    {
        public static readonly List<IMenuItem> MenuItems = new List<IMenuItem>();

        static MenuManager()
        {
            UIManager.Manager.Shell.AddMenu();
            UIManager.Manager.Shell.AddToolbar();
        }

        public static void Add(IMenuItem menuItem)
        {
            MenuItems.Add(menuItem);
        }

        public static void Build()
        {
            foreach (var menuItem in MenuItems)
            {
                var id = "Command/" + menuItem.Command.Identifier;
                if (menuItem.ShowInMenu)
                {
                    var group = GetGroup(menuItem.Command.Group);
                    var mi = group.AddSubMenuItem(id);
                    mi.Icon = menuItem.Image;
                    mi.Clicked += TriggerMenuAction(menuItem);
                }
                if (menuItem.ShowInToolstrip)
                {
                    var ti = UIManager.Manager.Shell.Toolbar.AddToolbarItem(id);
                    ti.Icon = menuItem.Image;
                    ti.Clicked += TriggerMenuAction(menuItem);
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
                    var id = "CommandGroup/" + g;
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
            return (sender, eventArgs) => menuItem.Command.Fire();
        }
    }

    public interface IMenuItem
    {
        ICommand Command { get; }
        Image Image { get; }
        bool IsActive { get; }
        bool ShowInMenu { get; }
        bool ShowInToolstrip { get; }
    }

    public class BasicMenuItem : IMenuItem
    {
        public ICommand Command { get; set; }
        public Image Image { get; set; }
        public bool IsActive { get; set; }
        public bool ShowInMenu { get; set; }
        public bool ShowInToolstrip { get; set; }

        public BasicMenuItem(ICommand command, Image image, bool isActive, bool showInMenu, bool showInToolstrip)
        {
            Command = command;
            Image = image;
            IsActive = isActive;
            ShowInMenu = showInMenu;
            ShowInToolstrip = showInToolstrip;
        }
    }

    public class DynamicMenuItem : IMenuItem
    {
        private readonly Func<bool> _isActive;

        public ICommand Command { get; set; }
        public Image Image { get; set; }
        public bool IsActive { get { return _isActive(); } }
        public bool ShowInMenu { get; set; }
        public bool ShowInToolstrip { get; set; }

        public DynamicMenuItem(ICommand command, Image image, Func<bool> isActive, bool showInMenu, bool showInToolstrip)
        {
            Command = command;
            Image = image;
            _isActive = isActive;
            ShowInMenu = showInMenu;
            ShowInToolstrip = showInToolstrip;
        }
    }
}
