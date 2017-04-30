using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Hooks;
using Sledge.Common.Shell.Menu;
using Sledge.Shell.Forms;

namespace Sledge.Shell.Registers
{
    /// <summary>
    /// The menu register registers and handles menu items
    /// </summary>
    [Export(typeof(IStartupHook))]
    [Export(typeof(IInitialiseHook))]
    internal class MenuRegister : IStartupHook, IInitialiseHook
    {
        // The menu register needs direct access to the shell
        [Import] private Forms.Shell _shell;

        // Store the context (the menu register is one of the few things that should need static access to the context)
        [Import] private IContext _context;
        
        [ImportMany] private IEnumerable<Lazy<IMenuItemProvider>> _itemProviders;
        [ImportMany] private IEnumerable<Lazy<IMenuMetadataProvider>> _metaDataProviders;

        public async Task OnStartup()
        {
            // Register commands as menu items
            foreach (var export in _itemProviders)
            {
                foreach (var menuItem in export.Value.GetMenuItems())
                {
                    Add(menuItem);
                }
            }

            // Register metadata providers
            foreach (var md in _metaDataProviders)
            {
                _declaredGroups.AddRange(md.Value.GetMenuGroups());
                _declaredSections.AddRange(md.Value.GetMenuSections());
            }
        }

        public async Task OnInitialise()
        {
            _tree = new VirtualMenuTree(_context, _shell.MenuStrip, _declaredSections, _declaredGroups);

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

        private readonly List<MenuSection> _declaredSections;
        private readonly List<MenuGroup> _declaredGroups;

        private VirtualMenuTree _tree;

        public MenuRegister()
        {
            _menuItems = new Dictionary<string, IMenuItem>();
            _declaredSections = new List<MenuSection>();
            _declaredGroups = new List<MenuGroup>();
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
            private readonly IContext _context;
            private readonly List<MenuSection> _declaredSections;
            private readonly List<MenuGroup> _declaredGroups;

            public MenuStrip MenuStrip { get; set; }
            public Dictionary<string, MenuTreeNode> RootNodes { get; set; }

            public VirtualMenuTree(IContext context, MenuStrip menuStrip, List<MenuSection> declaredSections, List<MenuGroup> declaredGroups)
            {
                _context = context;
                _declaredSections = declaredSections;
                _declaredGroups = declaredGroups;
                MenuStrip = menuStrip;
                RootNodes = new Dictionary<string, MenuTreeNode>();

                // Add known sections straight away
                foreach (var ds in _declaredSections.OrderBy(x => x.OrderHint)) AddSection(ds.Name);
            }

            private void AddSection(string name)
            {
                var rtn = new MenuTreeNode(_context, name, null);
                rtn.ToolStripMenuItem.DropDownClosed += (s, a) => { Oy.Publish("Status:Information", ""); };
                RootNodes.Add(name, rtn);
                MenuStrip.Items.Add(rtn.ToolStripMenuItem);
            }

            public void Add(IMenuItem item)
            {
                // If the section isn't known, add it to the end
                if (!RootNodes.ContainsKey(item.Section)) AddSection(item.Section);

                // Find the parent node for this item
                // Start at the section root node
                var node = RootNodes[item.Section];

                // Traverse the path until we get to the target
                var path = (item.Path ?? "").Split('/').Where(x => x.Length > 0).ToList();
                var currentPath = new List<string>();
                foreach (var p in path)
                {
                    currentPath.Add(p);
                    // If the current node isn't found, add it in
                    if (!node.Children.ContainsKey(p))
                    {
                        var gr = _declaredGroups.FirstOrDefault(x => x.Name == p && x.Path == String.Join("/", currentPath) && x.Section == item.Section);
                        node.Add(p, new MenuTreeNode(_context, p, gr));
                    }
                    node = node.Children[p];
                }

                // Add the node to the parent node
                var group = _declaredGroups.FirstOrDefault(x => x.Name == item.Group && x.Path == item.Path && x.Section == item.Section);
                node.Add(item.ID, new MenuTreeNode(_context, item, group));
            }
        }

        private class MenuTreeGroup
        {
            public MenuGroup Group { get; set; }
            public List<MenuTreeNode> Nodes { get; set; }

            public bool HasSplitter { get; set; }

            public MenuTreeGroup(MenuGroup group)
            {
                Group = group;
                Nodes = new List<MenuTreeNode>();
            }
        }

        private class MenuTreeNode
        {
            private readonly MenuGroup _group;
            private IContext _context;

            public ToolStripMenuItem ToolStripMenuItem { get; set; }
            public IMenuItem MenuItem { get; set; }
            public List<MenuTreeGroup> Groups { get; set; }
            public Dictionary<string, MenuTreeNode> Children { get; }

            public MenuTreeNode(IContext context, IMenuItem menuItem, MenuGroup group)
            {
                _context = context;
                _group = group ?? new MenuGroup("", "", "", "T");
                ToolStripMenuItem = new ToolStripMenuItem(menuItem.Name, menuItem.Icon) {Tag = this};
                ToolStripMenuItem.Click += Fire;
                ToolStripMenuItem.MouseEnter += (s, a) => { Oy.Publish("Status:Information", menuItem.Description); };
                ToolStripMenuItem.MouseLeave += (s, a) => { Oy.Publish("Status:Information", ""); };
                MenuItem = menuItem;
                Groups = new List<MenuTreeGroup>();
                Children = new Dictionary<string, MenuTreeNode>();
            }

            public MenuTreeNode(IContext context, string text, MenuGroup group)
            {
                _context = context;
                _group = group ?? new MenuGroup("", "", "", "T");
                ToolStripMenuItem = new ToolStripMenuItem(text) {Tag = this};
                Groups = new List<MenuTreeGroup>();
                Children = new Dictionary<string, MenuTreeNode>();
            }

            private void Fire(object sender, EventArgs e)
            {
                MenuItem?.Invoke(_context);
            }

            public void Add(string name, MenuTreeNode menuTreeNode)
            {
                Children.Add(name, menuTreeNode);
                if (Groups.All(x => x.Group.Name != menuTreeNode._group.Name))
                {
                    Groups.Add(new MenuTreeGroup(menuTreeNode._group));
                    Groups = Groups.OrderBy(x => x.Group.OrderHint).ToList();
                }

                // Insert the item into the correct index
                var groupIndex = Groups.FindIndex(x => x.Group.Name == menuTreeNode._group.Name);

                // Skip to the start of the group
                var groupStart = 0;
                for (var i = 0; i < groupIndex; i++)
                {
                    var g = Groups[i];
                    groupStart += g.Nodes.Count + (g.HasSplitter ? 1 : 0);
                }

                // Add the node to the list and sort
                var group = Groups[groupIndex];
                group.Nodes = group.Nodes.Union(new[] {menuTreeNode}).OrderBy(x => x.MenuItem.OrderHint ?? "").ToList();

                // Skip to the start of the node and insert
                var idx = group.Nodes.IndexOf(menuTreeNode);
                ToolStripMenuItem.DropDownItems.Insert(groupStart + idx, menuTreeNode.ToolStripMenuItem);

                // Check groups for splitters
                groupStart = 0;
                for (var i = 0; i < Groups.Count - 1; i++)
                {
                    var g = Groups[i];
                    groupStart += g.Nodes.Count;

                    // Add a splitter to the group if needed
                    if (!g.HasSplitter && g.Nodes.Count > 0)
                    {
                        ToolStripMenuItem.DropDownItems.Insert(groupStart, new ToolStripSeparator());
                        g.HasSplitter = true;
                    }

                    groupStart++;
                }
            }
        }
    }
}
