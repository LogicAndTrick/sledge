using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sledge.Common.Commands;
using Sledge.Common.Context;
using Sledge.Common.Hooks;
using Sledge.Common.Hotkeys;
using Sledge.Common.Menu;
using Sledge.Common.Settings;
using Sledge.Shell.Forms;

namespace Sledge.Shell.Registers
{
    /// <summary>
    /// The hotkey register registers and handles hotkeys
    /// </summary>
    [Export(typeof(IStartupHook))]
    [Export(typeof(IInitialiseHook))]
    internal class MenuRegister : IStartupHook, IInitialiseHook
    {
        // The menu register needs direct access to the shell
        [Import] private Forms.Shell _shell;

        // Store the context (the menu register is one of the few things that should need static access to the context)
        [Import] private IContext _context;

        [ImportMany] private IEnumerable<Lazy<ICommand>> _commands;

        public async Task OnStartup()
        {
            // Register commands as menu items
            foreach (var export in _commands)
            {
                var ty = export.Value.GetType();
                var mia = ty.GetCustomAttributes(typeof(MenuItemAttribute), false).OfType<MenuItemAttribute>().FirstOrDefault();
                if (mia == null) continue;
                Add(new CommandMenuItem(export.Value, mia.Section, mia.Path, mia.Group));
            }
        }

        public async Task OnInitialise()
        {
            _tree = new VirtualMenuTree(_shell.MenuStrip);

            _shell.Invoke((MethodInvoker) delegate
            {
                foreach (var mi in _menuItems.Values)
                {
                    _tree.Add(mi);
                }
            });
        }

        /// <summary>
        /// The list of all menu items by ID
        /// </summary>
        private readonly Dictionary<string, IMenuItem> _menuItems;

        private VirtualMenuTree _tree;

        public MenuRegister()
        {
            _menuItems = new Dictionary<string, IMenuItem>();
        }

        /// <summary>
        /// Add a menu item to the list
        /// </summary>
        /// <param name="menuItem">The menu item to add</param>
        private void Add(IMenuItem menuItem)
        {
            _menuItems[menuItem.ID] = menuItem;
        }

        private class VirtualMenuTree
        {
            public MenuStrip MenuStrip { get; set; }
            public Dictionary<string, MenuTreeNode> RootNodes { get; set; }

            public VirtualMenuTree(MenuStrip menuStrip)
            {
                MenuStrip = menuStrip;
                RootNodes = new Dictionary<string, MenuTreeNode>();
            }

            public void Add(IMenuItem item)
            {
                if (!RootNodes.ContainsKey(item.Section))
                {
                    var rtn = new MenuTreeNode(item.Section);
                    RootNodes.Add(item.Section, rtn);
                    MenuStrip.Items.Add(rtn.ToolStripMenuItem);
                }
                var node = RootNodes[item.Section];
                var path = (item.Path ?? "").Split('/').Where(x => x.Length > 0).ToList();
                foreach (var p in path)
                {
                    if (!node.Children.ContainsKey(p)) node.Add(p, new MenuTreeNode(p));
                    node = node.Children[p];
                }
                node.Add(item.ID, new MenuTreeNode(item));
            }
        }

        private class MenuTreeNode
        {
            public ToolStripMenuItem ToolStripMenuItem { get; set; }
            public IMenuItem MenuItem { get; set; }
            public Dictionary<string, MenuTreeNode> Children { get; set; }

            public MenuTreeNode(IMenuItem menuItem)
            {
                ToolStripMenuItem = new ToolStripMenuItem(menuItem.Description);
                ToolStripMenuItem.Click += Fire;
                MenuItem = menuItem;
                Children = new Dictionary<string, MenuTreeNode>();
            }

            public MenuTreeNode(string text)
            {
                ToolStripMenuItem = new ToolStripMenuItem(text);
                Children = new Dictionary<string, MenuTreeNode>();
            }

            private void Fire(object sender, EventArgs e)
            {
                MenuItem?.Invoke();
            }

            public void Add(string name, MenuTreeNode menuTreeNode)
            {
                Children.Add(name, menuTreeNode);
                ToolStripMenuItem.DropDownItems.Add(menuTreeNode.ToolStripMenuItem);
            }
        }
    }
}
