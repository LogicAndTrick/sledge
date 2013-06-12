using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Sledge.Common.Mediator;
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
            Add("File", new SimpleMenuBuilder("Close", HotkeysMediator.FileClose) {IsVisible = mapOpen});
            Add("File", new SimpleMenuBuilder("Save", HotkeysMediator.FileSave) {IsVisible = mapOpen});
            Add("File", new SimpleMenuBuilder("Save As...", HotkeysMediator.FileSaveAs) {IsVisible = mapOpen});
            Add("File", new MenuSplitter {IsVisible = mapOpen});
            Add("File", new SimpleMenuBuilder("Run", HotkeysMediator.FileCompile) {IsVisible = mapOpen});
            Add("File", new RecentFilesMenu());
            Add("File", new MenuSplitter());
            Add("File", new SimpleMenuBuilder("Exit", EditorMediator.Exit));
            
            Func<bool> canUndo = () => DocumentManager.CurrentDocument.History.CanUndo();
            Func<bool> canRedo = () => DocumentManager.CurrentDocument.History.CanRedo();
            Func<bool> itemsSelected = () => DocumentManager.CurrentDocument.Selection.GetSelectedObjects().Any();
            Func<bool> canPaste = Clipboard.ClipboardManager.CanPaste;
            Add("Edit", new SimpleMenuBuilder("Undo", HotkeysMediator.HistoryUndo) { IsVisible = mapOpen, IsActive = canUndo });
            Add("Edit", new SimpleMenuBuilder("Redo", HotkeysMediator.HistoryRedo) { IsVisible = mapOpen, IsActive = canRedo });
            Add("Edit", new MenuSplitter { IsVisible = mapOpen });
            Add("Edit", new SimpleMenuBuilder("Cut", HotkeysMediator.OperationsCut) { IsVisible = mapOpen, IsActive = itemsSelected });
            Add("Edit", new SimpleMenuBuilder("Copy", HotkeysMediator.OperationsCopy) { IsVisible = mapOpen, IsActive = itemsSelected });
            Add("Edit", new SimpleMenuBuilder("Paste", HotkeysMediator.OperationsPaste) { IsVisible = mapOpen, IsActive = canPaste });
            Add("Edit", new SimpleMenuBuilder("Paste Special...", HotkeysMediator.OperationsPasteSpecial) { IsVisible = mapOpen, IsActive = canPaste });
            Add("Edit", new SimpleMenuBuilder("Delete", HotkeysMediator.OperationsDelete) { IsVisible = mapOpen, IsActive = itemsSelected });
            Add("Edit", new MenuSplitter { IsVisible = mapOpen });
            Add("Edit", new SimpleMenuBuilder("Clear Selection", HotkeysMediator.SelectionClear) { IsVisible = mapOpen, IsActive = itemsSelected });
            Add("Edit", new SimpleMenuBuilder("Select All", HotkeysMediator.SelectAll) { IsVisible = mapOpen });
            Add("Edit", new MenuSplitter { IsVisible = mapOpen });
            Add("Edit", new SimpleMenuBuilder("Object Properties", HotkeysMediator.ObjectProperties) { IsVisible = mapOpen });

            Add("Map", new SimpleMenuBuilder("Snap to Grid", "") { IsVisible = mapOpen });
            Add("Map", new SimpleMenuBuilder("Show Grid", "") { IsVisible = mapOpen });
            Add("Map", new GroupedMenuBuilder("Grid Settings",
                                              new SimpleMenuBuilder("Smaller Grid", HotkeysMediator.GridDecrease) { IsVisible = mapOpen },
                                              new SimpleMenuBuilder("Bigger Grid", HotkeysMediator.GridIncrease) { IsVisible = mapOpen }
                           ) { IsVisible = mapOpen });
            Add("Map", new MenuSplitter { IsVisible = mapOpen });
            Add("Map", new SimpleMenuBuilder("Show Information", "") { IsVisible = mapOpen });
            Add("Map", new SimpleMenuBuilder("Show Selected Brush ID", "") { IsVisible = mapOpen, IsActive = itemsSelected });
            Add("Map", new SimpleMenuBuilder("Entity Report...", "") { IsVisible = mapOpen });
            Add("Map", new SimpleMenuBuilder("Check for Problems", "") { IsVisible = mapOpen });
            Add("Map", new MenuSplitter { IsVisible = mapOpen });
            Add("Map", new SimpleMenuBuilder("Map Properties...", EditorMediator.WorldspawnProperties) { IsVisible = mapOpen });
            Add("Map", new MenuSplitter { IsVisible = mapOpen });
            Add("Map", new SimpleMenuBuilder("Load Pointfile...", "") { IsVisible = mapOpen });
            Add("Map", new SimpleMenuBuilder("Unload Pointfile", "") { IsVisible = mapOpen });

            Add("View", new SimpleMenuBuilder("Autosize 4 Views", HotkeysMediator.FourViewAutosize) { IsVisible = mapOpen });
            Add("View", new SimpleMenuBuilder("Center 2D Views on Selection", "") { IsVisible = mapOpen, IsActive = itemsSelected });
            Add("View", new SimpleMenuBuilder("Center 3D View on Selection", "") { IsVisible = mapOpen, IsActive = itemsSelected });
            Add("View", new MenuSplitter { IsVisible = mapOpen });
            Add("Map", new SimpleMenuBuilder("Go to Brush ID...", "") { IsVisible = mapOpen });
            Add("Map", new SimpleMenuBuilder("Go to Coordinates...", "") { IsVisible = mapOpen });
            Add("View", new MenuSplitter { IsVisible = mapOpen });
            Add("View", new SimpleMenuBuilder("Hide Selected Objects", "") { IsVisible = mapOpen });
            Add("View", new SimpleMenuBuilder("Hide Unselected Objects", "") { IsVisible = mapOpen });
            Add("View", new SimpleMenuBuilder("Show Hidden Objects", "") { IsVisible = mapOpen });

            Add("Tools", new SimpleMenuBuilder("Carve", HotkeysMediator.Carve) { IsVisible = mapOpen, IsActive = itemsSelected });
            Add("Tools", new SimpleMenuBuilder("Make Hollow", HotkeysMediator.MakeHollow) { IsVisible = mapOpen, IsActive = itemsSelected });
            Add("Tools", new MenuSplitter { IsVisible = mapOpen });
            Add("Tools", new SimpleMenuBuilder("Group", HotkeysMediator.GroupingGroup) { IsVisible = mapOpen, IsActive = itemsSelected });
            Add("Tools", new SimpleMenuBuilder("Ungroup", HotkeysMediator.GroupingUngroup) { IsVisible = mapOpen, IsActive = itemsSelected });
            Add("Tools", new MenuSplitter { IsVisible = mapOpen });
            Add("Tools", new SimpleMenuBuilder("Tie to Entity", HotkeysMediator.TieToEntity) { IsVisible = mapOpen, IsActive = itemsSelected });
            Add("Tools", new SimpleMenuBuilder("Move to World", HotkeysMediator.TieToWorld) { IsVisible = mapOpen, IsActive = itemsSelected });
            Add("Tools", new MenuSplitter { IsVisible = mapOpen });
            Add("Tools", new SimpleMenuBuilder("Replace Textures", "") { IsVisible = mapOpen });
            Add("Tools", new MenuSplitter { IsVisible = mapOpen });
            Add("Tools", new SimpleMenuBuilder("Transform...", "") { IsVisible = mapOpen, IsActive = itemsSelected });
            Add("Tools", new SimpleMenuBuilder("Snap Selected to Grid", "") { IsVisible = mapOpen, IsActive = itemsSelected });
            Add("Tools", new SimpleMenuBuilder("Snap Selected to Grid Individually", "") { IsVisible = mapOpen, IsActive = itemsSelected });
            Add("Tools", new GroupedMenuBuilder("Align Objects",
                                                new SimpleMenuBuilder("To Left", "") { IsVisible = mapOpen, IsActive = itemsSelected },
                                                new SimpleMenuBuilder("To Right", "") { IsVisible = mapOpen, IsActive = itemsSelected },
                                                new SimpleMenuBuilder("To Top", "") { IsVisible = mapOpen, IsActive = itemsSelected },
                                                new SimpleMenuBuilder("To Bottom", "") { IsVisible = mapOpen, IsActive = itemsSelected }
                             ) { IsVisible = mapOpen });
            Add("Tools", new GroupedMenuBuilder("Flip Objects",
                                                new SimpleMenuBuilder("X Axis", "") { IsVisible = mapOpen, IsActive = itemsSelected },
                                                new SimpleMenuBuilder("Y Axis", "") { IsVisible = mapOpen, IsActive = itemsSelected },
                                                new SimpleMenuBuilder("Z Axis", "") { IsVisible = mapOpen, IsActive = itemsSelected }
                             ) { IsVisible = mapOpen });
            Add("Tools", new MenuSplitter { IsVisible = mapOpen });
            Add("Tools", new SimpleMenuBuilder("Options...", EditorMediator.OpenSettings));

            Add("Help", new SimpleMenuBuilder("Online Help", EditorMediator.OpenWebsite, "http://example.com/"));
            Add("Help", new SimpleMenuBuilder("Sledge Website", EditorMediator.OpenWebsite, "http://example.com/"));
            Add("Help", new SimpleMenuBuilder("Sledge Community", EditorMediator.OpenWebsite, "http://example.com/"));
            Add("Help", new MenuSplitter());
            Add("Help", new SimpleMenuBuilder("Check for Updates", EditorMediator.CheckForUpdates));
            Add("Help", new MenuSplitter());
            Add("Help", new SimpleMenuBuilder("About...", EditorMediator.About));
        }

        public static void Rebuild()
        {
            if (_menu == null) return;
            foreach (ToolStripMenuItem mi in _menu.Items)
            {
                mi.DropDownOpening -= DropDownOpening;
            }
            _menu.Items.Clear();
            foreach (var kv in MenuItems)
            {
                var mi = new ToolStripMenuItem(kv.Key);
                mi.DropDownItems.AddRange(kv.Value.SelectMany(x => x.Build()).ToArray());
                if (mi.DropDownItems.Count <= 0) continue;
                mi.DropDownOpening += DropDownOpening;
                _menu.Items.Add(mi);
            }
        }

        private static void DropDownOpening(object sender, EventArgs e)
        {
            Mediator.Publish(EditorMediator.UpdateMenu);
        }
    }
}
