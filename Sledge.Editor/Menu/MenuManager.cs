using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Sledge.Common.Mediator;
using Sledge.Database.Models;
using Sledge.Editor.Documents;
using Sledge.Editor.Properties;
using Sledge.Settings;

namespace Sledge.Editor.Menu
{
    public static class MenuManager
    {
        private static MenuStrip _menu;
        private static ToolStripContainer _container;
        private static readonly Dictionary<string, List<IMenuBuilder>> MenuItems;

        public static List<RecentFile> RecentFiles { get; private set; }

        static MenuManager()
        {
            RecentFiles = new List<RecentFile>();
            MenuItems = new Dictionary<string, List<IMenuBuilder>>();
        }

        public static void Init(MenuStrip menu, ToolStripContainer toolStripContainer)
        {
            _menu = menu;
            _container = toolStripContainer;
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
            Add("File", new SimpleMenuBuilder("New", HotkeysMediator.FileNew) { Image = Resources.Menu_New, ShowInToolStrip = true });
            Add("File", new SimpleMenuBuilder("Open", HotkeysMediator.FileOpen) { Image = Resources.Menu_Open, ShowInToolStrip = true });
            Add("File", new SimpleMenuBuilder("Close", HotkeysMediator.FileClose) {IsVisible = mapOpen, Image = Resources.Menu_Close, ShowInToolStrip = true});
            Add("File", new SimpleMenuBuilder("Save", HotkeysMediator.FileSave) {IsVisible = mapOpen, ShowInToolStrip = true});
            Add("File", new SimpleMenuBuilder("Save As...", HotkeysMediator.FileSaveAs) {IsVisible = mapOpen});
            Add("File", new MenuSplitter { IsVisible = mapOpen, ShowInToolStrip = true });
            Add("File", new SimpleMenuBuilder("Run", HotkeysMediator.FileCompile) { IsVisible = mapOpen, ShowInToolStrip = true });
            Add("File", new RecentFilesMenu());
            Add("File", new MenuSplitter());
            Add("File", new SimpleMenuBuilder("Exit", EditorMediator.Exit));
            
            Func<bool> canUndo = () => DocumentManager.CurrentDocument.History.CanUndo();
            Func<bool> canRedo = () => DocumentManager.CurrentDocument.History.CanRedo();
            Func<string> undoText = () => DocumentManager.CurrentDocument.History.GetUndoString();
            Func<string> redoText = () => DocumentManager.CurrentDocument.History.GetRedoString();
            Func<bool> itemsSelected = () => DocumentManager.CurrentDocument.Selection.GetSelectedObjects().Any();
            Func<bool> canPaste = Clipboard.ClipboardManager.CanPaste;
            Add("Edit", new SimpleMenuBuilder("Undo", HotkeysMediator.HistoryUndo) { IsVisible = mapOpen, IsActive = canUndo, Text = undoText, ShowInToolStrip = true });
            Add("Edit", new SimpleMenuBuilder("Redo", HotkeysMediator.HistoryRedo) { IsVisible = mapOpen, IsActive = canRedo, Text = redoText, ShowInToolStrip = true });
            Add("Edit", new MenuSplitter { IsVisible = mapOpen, ShowInToolStrip = true });
            Add("Edit", new SimpleMenuBuilder("Cut", HotkeysMediator.OperationsCut) { IsVisible = mapOpen, IsActive = itemsSelected, ShowInToolStrip = true });
            Add("Edit", new SimpleMenuBuilder("Copy", HotkeysMediator.OperationsCopy) { IsVisible = mapOpen, IsActive = itemsSelected, ShowInToolStrip = true });
            Add("Edit", new SimpleMenuBuilder("Paste", HotkeysMediator.OperationsPaste) { IsVisible = mapOpen, IsActive = canPaste, ShowInToolStrip = true });
            Add("Edit", new SimpleMenuBuilder("Paste Special...", HotkeysMediator.OperationsPasteSpecial) { IsVisible = mapOpen, IsActive = canPaste, ShowInToolStrip = true });
            Add("Edit", new SimpleMenuBuilder("Delete", HotkeysMediator.OperationsDelete) { IsVisible = mapOpen, IsActive = itemsSelected, ShowInToolStrip = true });
            Add("Edit", new MenuSplitter { IsVisible = mapOpen, ShowInToolStrip = true });
            Add("Edit", new SimpleMenuBuilder("Clear Selection", HotkeysMediator.SelectionClear) { IsVisible = mapOpen, IsActive = itemsSelected });
            Add("Edit", new SimpleMenuBuilder("Select All", HotkeysMediator.SelectAll) { IsVisible = mapOpen });
            Add("Edit", new MenuSplitter { IsVisible = mapOpen });
            Add("Edit", new SimpleMenuBuilder("Object Properties", HotkeysMediator.ObjectProperties) { IsVisible = mapOpen, ShowInToolStrip = true });

            Add("Map", new SimpleMenuBuilder("Snap to Grid", HotkeysMediator.ToggleSnapToGrid) { IsVisible = mapOpen, IsChecked = () => DocumentManager.CurrentDocument.Map.SnapToGrid, ShowInToolStrip = true });
            Add("Map", new SimpleMenuBuilder("Show 2D Grid", HotkeysMediator.ToggleShow2DGrid) { IsVisible = mapOpen, IsChecked = () => DocumentManager.CurrentDocument.Map.Show2DGrid, ShowInToolStrip = true });
            Add("Map", new SimpleMenuBuilder("Show 3D Grid", HotkeysMediator.ToggleShow3DGrid) { IsVisible = mapOpen, IsChecked = () => DocumentManager.CurrentDocument.Map.Show3DGrid, ShowInToolStrip = true });
            Add("Map", new GroupedMenuBuilder("Grid Settings",
                                              new SimpleMenuBuilder("Smaller Grid", HotkeysMediator.GridDecrease) { IsVisible = mapOpen, ShowInToolStrip = true },
                                              new SimpleMenuBuilder("Bigger Grid", HotkeysMediator.GridIncrease) { IsVisible = mapOpen, ShowInToolStrip = true }
                           ) { IsVisible = mapOpen });
            Add("Map", new MenuSplitter { IsVisible = mapOpen });
            Add("Map", new SimpleMenuBuilder("Show Information", HotkeysMediator.ShowMapInformation) { IsVisible = mapOpen });
            Add("Map", new SimpleMenuBuilder("Show Selected Brush ID", HotkeysMediator.ShowSelectedBrushID) { IsVisible = mapOpen, IsActive = itemsSelected });
            Add("Map", new SimpleMenuBuilder("Entity Report...", HotkeysMediator.ShowEntityReport) { IsVisible = mapOpen });
            Add("Map", new SimpleMenuBuilder("Check for Problems", HotkeysMediator.CheckForProblems) { IsVisible = mapOpen });
            Add("Map", new MenuSplitter { IsVisible = mapOpen });
            Add("Map", new SimpleMenuBuilder("Map Properties...", EditorMediator.WorldspawnProperties) { IsVisible = mapOpen });
            Add("Map", new MenuSplitter { IsVisible = mapOpen });
            Add("Map", new SimpleMenuBuilder("Quick Load Pointfile", HotkeysMediator.QuickLoadPointfile) { IsVisible = mapOpen });
            Add("Map", new SimpleMenuBuilder("Load Pointfile...", HotkeysMediator.LoadPointfile) { IsVisible = mapOpen });
            Add("Map", new SimpleMenuBuilder("Unload Pointfile", HotkeysMediator.UnloadPointfile) { IsVisible = mapOpen });

            Add("View", new SimpleMenuBuilder("Autosize 4 Views", HotkeysMediator.FourViewAutosize) { IsVisible = mapOpen });
            Add("View", new SimpleMenuBuilder("Center All Views on Selection", HotkeysMediator.CenterAllViewsOnSelection) { IsVisible = mapOpen, IsActive = itemsSelected });
            Add("View", new SimpleMenuBuilder("Center 2D Views on Selection", HotkeysMediator.Center2DViewsOnSelection) { IsVisible = mapOpen, IsActive = itemsSelected });
            Add("View", new SimpleMenuBuilder("Center 3D View on Selection", HotkeysMediator.Center3DViewsOnSelection) { IsVisible = mapOpen, IsActive = itemsSelected });
            Add("View", new MenuSplitter { IsVisible = mapOpen });
            Add("View", new SimpleMenuBuilder("Go to Brush ID...", HotkeysMediator.GoToBrushID) { IsVisible = mapOpen });
            Add("View", new SimpleMenuBuilder("Go to Coordinates...", HotkeysMediator.GoToCoordinates) { IsVisible = mapOpen });
            Add("View", new MenuSplitter { IsVisible = mapOpen });
            Add("View", new SimpleMenuBuilder("Hide Selected Objects", "") { IsVisible = mapOpen, ShowInToolStrip = true });
            Add("View", new SimpleMenuBuilder("Hide Unselected Objects", "") { IsVisible = mapOpen, ShowInToolStrip = true });
            Add("View", new SimpleMenuBuilder("Show Hidden Objects", "") { IsVisible = mapOpen, ShowInToolStrip = true });

            Add("Tools", new SimpleMenuBuilder("Carve", HotkeysMediator.Carve) { IsVisible = mapOpen, IsActive = itemsSelected, ShowInToolStrip = true });
            Add("Tools", new SimpleMenuBuilder("Make Hollow", HotkeysMediator.MakeHollow) { IsVisible = mapOpen, IsActive = itemsSelected, ShowInToolStrip = true });
            Add("Tools", new MenuSplitter { IsVisible = mapOpen });
            Add("Tools", new SimpleMenuBuilder("Group", HotkeysMediator.GroupingGroup) { IsVisible = mapOpen, IsActive = itemsSelected, ShowInToolStrip = true });
            Add("Tools", new SimpleMenuBuilder("Ungroup", HotkeysMediator.GroupingUngroup) { IsVisible = mapOpen, IsActive = itemsSelected, ShowInToolStrip = true });
            Add("Tools", new MenuSplitter { IsVisible = mapOpen });
            Add("Tools", new SimpleMenuBuilder("Tie to Entity", HotkeysMediator.TieToEntity) { IsVisible = mapOpen, IsActive = itemsSelected });
            Add("Tools", new SimpleMenuBuilder("Move to World", HotkeysMediator.TieToWorld) { IsVisible = mapOpen, IsActive = itemsSelected });
            Add("Tools", new MenuSplitter { IsVisible = mapOpen });
            Add("Tools", new SimpleMenuBuilder("Replace Textures", "") { IsVisible = mapOpen });
            Add("Tools", new MenuSplitter { IsVisible = mapOpen });
            Add("Tools", new SimpleMenuBuilder("Transform...", HotkeysMediator.Transform) { IsVisible = mapOpen, IsActive = itemsSelected });
            Add("Tools", new SimpleMenuBuilder("Snap Selected to Grid", HotkeysMediator.SnapSelectionToGrid) { IsVisible = mapOpen, IsActive = itemsSelected });
            Add("Tools", new SimpleMenuBuilder("Snap Selected to Grid Individually", HotkeysMediator.SnapSelectionToGridIndividually) { IsVisible = mapOpen, IsActive = itemsSelected });
            Add("Tools", new GroupedMenuBuilder("Align Objects",
                                                new SimpleMenuBuilder("To X Axis Min", HotkeysMediator.AlignXMin) { IsVisible = mapOpen, IsActive = itemsSelected },
                                                new SimpleMenuBuilder("To X Axis Max", HotkeysMediator.AlignXMax) { IsVisible = mapOpen, IsActive = itemsSelected },
                                                new SimpleMenuBuilder("To Y Axis Min", HotkeysMediator.AlignYMin) { IsVisible = mapOpen, IsActive = itemsSelected },
                                                new SimpleMenuBuilder("To Y Axis Max", HotkeysMediator.AlignYMax) { IsVisible = mapOpen, IsActive = itemsSelected },
                                                new SimpleMenuBuilder("To Z Axis Min", HotkeysMediator.AlignZMin) { IsVisible = mapOpen, IsActive = itemsSelected },
                                                new SimpleMenuBuilder("To Z Axis Max", HotkeysMediator.AlignZMax) { IsVisible = mapOpen, IsActive = itemsSelected }
                             ) { IsVisible = mapOpen });
            Add("Tools", new GroupedMenuBuilder("Flip Objects",
                                                new SimpleMenuBuilder("X Axis", HotkeysMediator.FlipX) { IsVisible = mapOpen, IsActive = itemsSelected },
                                                new SimpleMenuBuilder("Y Axis", HotkeysMediator.FlipY) { IsVisible = mapOpen, IsActive = itemsSelected },
                                                new SimpleMenuBuilder("Z Axis", HotkeysMediator.FlipZ) { IsVisible = mapOpen, IsActive = itemsSelected }
                             ) { IsVisible = mapOpen });
            Add("Tools", new MenuSplitter { IsVisible = mapOpen });
            Add("Tools", new SimpleMenuBuilder("Options...", EditorMediator.OpenSettings){ ShowInToolStrip = true });

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
            if (_menu == null || _container == null) return;
            foreach (ToolStripMenuItem mi in _menu.Items)
            {
                mi.DropDownOpening -= DropDownOpening;
            }
            _menu.Items.Clear();
            foreach (var kv in MenuItems)
            {
                var mi = new ToolStripMenuItem(kv.Key);
                mi.DropDownItems.AddRange(kv.Value.Where(x => x.ShowInMenu).SelectMany(x => x.Build()).ToArray());
                if (mi.DropDownItems.Count <= 0) continue;
                mi.DropDownOpening += DropDownOpening;
                _menu.Items.Add(mi);
            }
            // Need to remove and re-add tool strips because the ordering is incorrect otherwise
            foreach (var control in _container.Controls.OfType<ToolStripPanel>().SelectMany(x => x.Controls.OfType<ToolStrip>()))
            {
                if (MenuItems.Any(x => x.Key == control.Name))
                {
                    control.Parent.Controls.Remove(control);
                    control.Dispose();
                }
            }
            foreach (var kv in MenuItems.Reverse())
            {
                var ts = new ToolStrip {Name = kv.Key};
                ts.Items.Clear();
                ts.Items.AddRange(kv.Value.Where(x => x.ShowInToolStrip).SelectMany(x => x.BuildToolStrip()).ToArray());
                if (ts.Items.Count > 0) _container.TopToolStripPanel.Join(ts);
            }
        }

        private static void DropDownOpening(object sender, EventArgs e)
        {
            Mediator.Publish(EditorMediator.UpdateMenu);
        }
    }
}
