using System;
using System.Windows.Forms;
using Sledge.Common.Mediator;
using Sledge.Settings;

namespace Sledge.Editor.UI
{
    internal sealed class ViewportContextMenu : ContextMenuStrip
    {
        public static ViewportContextMenu Instance { get; private set; }

        static ViewportContextMenu()
        {
            Instance = new ViewportContextMenu();
        }

        public void AddNonSelectionItems()
        {
            Items.Clear();
            Add("Paste", HotkeysMediator.OperationsPaste);
            Add("Paste Special", HotkeysMediator.OperationsPasteSpecial);
            Items.Add(new ToolStripSeparator());
            Add("Undo", HotkeysMediator.HistoryUndo);
            Add("Redo", HotkeysMediator.HistoryRedo);
        }

        public void AddSelectionItems()
        {
            Items.Clear();
            Add("Cut", HotkeysMediator.OperationsCut);
            Add("Copy", HotkeysMediator.OperationsCopy);
            Add("Delete", HotkeysMediator.OperationsDelete);
            Add("Paste Special", HotkeysMediator.OperationsPasteSpecial);
            Items.Add(new ToolStripSeparator());
            Add("Undo", HotkeysMediator.HistoryUndo);
            Add("Redo", HotkeysMediator.HistoryRedo);
            Items.Add(new ToolStripSeparator());
            Add("Carve", null);
            Add("Hollow", null);
            Items.Add(new ToolStripSeparator());
            Add("Group", HotkeysMediator.GroupingGroup);
            Add("Ungroup", HotkeysMediator.GroupingUngroup);
            Items.Add(new ToolStripSeparator());
            Add("Tie To Entity", HotkeysMediator.TieToEntity);
            Add("Move To World", HotkeysMediator.TieToWorld);
            Items.Add(new ToolStripSeparator());
            Items.Add(new ToolStripMenuItem("Align", null,
                                            CreateMenuItem("Top", null),
                                            CreateMenuItem("Left", null),
                                            CreateMenuItem("Right", null),
                                            CreateMenuItem("Bottom", null)));
            Add("Properties", HotkeysMediator.ObjectProperties);
        }

        private void Add(string name, Enum onclick)
        {
            Items.Add(CreateMenuItem(name, onclick));
        }

        private static ToolStripItem CreateMenuItem(string name, Enum onclick)
        {
            var item = new ToolStripMenuItem(name);
            item.Click += (sender, args) => Mediator.Publish(onclick);
            return item;
        }
    }
}