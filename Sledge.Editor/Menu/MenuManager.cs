using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Sledge.Database.Models;
using Sledge.Editor.Documents;
using Sledge.Settings;

namespace Sledge.Editor.Menu
{
    public static class MenuManager
    {
        private static MenuStrip _menu;
        private static readonly Dictionary<string, List<IMenuBuilder>> MenuItems;

        public static List<RecentFile> RecentFiles { get; private set; }

        static MenuManager()
        {
            RecentFiles = new List<RecentFile>();
            MenuItems = new Dictionary<string, List<IMenuBuilder>>();
        }

        public static void Init(MenuStrip menu)
        {
            _menu = menu;
            AddDefault();
        }

        public static void Add(string menuName, IMenuBuilder builder)
        {
            if (!MenuItems.ContainsKey(menuName)) MenuItems.Add(menuName, new List<IMenuBuilder>());
            MenuItems[menuName].Add(builder);
        }

        public static void Insert(string menuName, int index, IMenuBuilder builder)
        {
            if (!MenuItems.ContainsKey(menuName)) MenuItems.Add(menuName, new List<IMenuBuilder>());
            if (index < 0) index = 0;
            if (index > MenuItems[menuName].Count) index = MenuItems[menuName].Count;
            MenuItems[menuName].Insert(index, builder);
        }

        private static void AddDefault()
        {
            Func<bool> mapOpen = () => DocumentManager.CurrentDocument != null;
            Add("File", new SimpleMenuBuilder("New", HotkeysMediator.FileNew));
            Add("File", new SimpleMenuBuilder("Open", HotkeysMediator.FileOpen));
            Add("File", new SimpleMenuBuilder("Close", "") {IsVisible = mapOpen});
            Add("File", new SimpleMenuBuilder("Save", HotkeysMediator.FileSave) {IsVisible = mapOpen});
            Add("File", new SimpleMenuBuilder("Save As...", "") {IsVisible = mapOpen});
            Add("File", new MenuSplitter {IsVisible = mapOpen});
            Add("File", new SimpleMenuBuilder("Run", HotkeysMediator.FileCompile) {IsVisible = mapOpen});
            Add("File", new RecentFilesMenu());
            Add("File", new MenuSplitter());
            Add("File", new SimpleMenuBuilder("Exit", ""));
        }

        public static void Rebuild()
        {
            if (_menu == null) return;
            _menu.Items.Clear();
            foreach (var kv in MenuItems)
            {
                var mi = new ToolStripMenuItem(kv.Key);
                mi.DropDownItems.AddRange(kv.Value.SelectMany(x => x.Build()).ToArray());
                _menu.Items.Add(mi);
            }
        }
    }
}
