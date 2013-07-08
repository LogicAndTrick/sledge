using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sledge.Settings
{
    public static class Hotkeys
    {
        private static readonly List<HotkeyDefinition> Definitions; 

        static Hotkeys()
        {
            Definitions = new List<HotkeyDefinition>
                               {
                                    new HotkeyDefinition("Autosize Views", "Reset the position of the 4-view splitter", HotkeysMediator.FourViewAutosize, "Ctrl+A"),
                                    new HotkeyDefinition("Focus View Top Left", "Focus on the 3D View", HotkeysMediator.FourViewFocusTopLeft, "F5"),
                                    new HotkeyDefinition("Focus View Top Right", "Focus on the XY View", HotkeysMediator.FourViewFocusTopRight, "F2"),
                                    new HotkeyDefinition("Focus View Bottom Left", "Focus on the YZ View", HotkeysMediator.FourViewFocusBottomLeft, "F4"),
                                    new HotkeyDefinition("Focus View Bottom Right", "Focus on the XZ View", HotkeysMediator.FourViewFocusBottomRight, "F3"),

                                    new HotkeyDefinition("New File", "Create a new map", HotkeysMediator.FileNew, "Ctrl+N"),
                                    new HotkeyDefinition("Open File", "Open an existing map", HotkeysMediator.FileOpen, "Ctrl+O"),
                                    new HotkeyDefinition("Save File", "Save the currently opened map", HotkeysMediator.FileSave, "Ctrl+S"),
                                    new HotkeyDefinition("Compile Map", "Compile the currently opened map", HotkeysMediator.FileCompile, "F9"),

                                    new HotkeyDefinition("Increase Grid Size", "Increase the current grid size", HotkeysMediator.GridIncrease, "]"),
                                    new HotkeyDefinition("Decrease Grid Size", "Decrease the current grid size", HotkeysMediator.GridDecrease, "["),

                                    new HotkeyDefinition("Undo", "Undo the last action", HotkeysMediator.HistoryUndo, "Ctrl+Z"),
                                    new HotkeyDefinition("Redo", "Redo the last undone action", HotkeysMediator.HistoryRedo, "Ctrl+Y"),

                                    new HotkeyDefinition("Show Object Properties", "Open the object properties dialog for the currently selected items", HotkeysMediator.ObjectProperties, "Alt+Enter"),
                                    
                                    new HotkeyDefinition("Copy", "Copy the current selection", HotkeysMediator.OperationsCopy, "Ctrl+C"),
                                    new HotkeyDefinition("Cut", "Cut the current selection", HotkeysMediator.OperationsCut, "Ctrl+X"),
                                    new HotkeyDefinition("Paste", "Paste the clipboard contents", HotkeysMediator.OperationsPaste, "Ctrl+V"),
                                    new HotkeyDefinition("Paste Special", "Paste special the clipboard contents", HotkeysMediator.OperationsPasteSpecial, "Ctrl+B"),
                                    new HotkeyDefinition("Delete", "Delete the current selection", HotkeysMediator.OperationsDelete, "Del"),
                                    
                                    new HotkeyDefinition("Group", "Group the selected objects", HotkeysMediator.GroupingGroup, "Ctrl+G"),
                                    new HotkeyDefinition("Ungroup", "Ungroup the selected objects", HotkeysMediator.GroupingUngroup, "Ctrl+U"),
                                    
                                    new HotkeyDefinition("Tie To Entity", "Tie the selected objects to an entity", HotkeysMediator.TieToEntity, "Ctrl+T"),
                                    new HotkeyDefinition("Move To World", "Move the selected objects to the world", HotkeysMediator.TieToWorld, "Ctrl+W"),
                               };
        }

        public static HotkeyDefinition GetHotkeyForMessage(String message)
        {
            return Definitions.FirstOrDefault(x => x.Action.ToString() == message);
        }

        public static HotkeyDefinition GetHotkeyFor(string keyCombination)
        {
            return Definitions.FirstOrDefault(x => x.DefaultHotkey == keyCombination);
        }
    }
}
