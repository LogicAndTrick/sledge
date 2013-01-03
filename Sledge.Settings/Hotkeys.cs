using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sledge.Settings
{
    public static class Hotkeys
    {
        private static List<HotkeyDefinition> _definitions; 

        static Hotkeys()
        {
            _definitions = new List<HotkeyDefinition>
                               {
                                   new HotkeyDefinition("Autosize Views", "Reset the position of the 4-view splitter", "FourViewAutosize", "Ctrl+A"),
                                    new HotkeyDefinition("Focus View Top Left", "Focus on the 3D View", "FourViewFocusTopLeft", "F5"),
                                    new HotkeyDefinition("Focus View Top Right", "Focus on the XY View", "FourViewFocusTopRight", "F2"),
                                    new HotkeyDefinition("Focus View Bottom Left", "Focus on the YZ View", "FourViewFocusBottomLeft", "F4"),
                                    new HotkeyDefinition("Focus View Bottom Right", "Focus on the XZ View", "FourViewFocusBottomRight", "F3"),

                                    new HotkeyDefinition("New File", "Create a new map", "FileNew", "Ctrl+N"),
                                    new HotkeyDefinition("Open File", "Open an existing map", "FileOpen", "Ctrl+O"),
                                    new HotkeyDefinition("Save File", "Save the currently opened map", "FileSave", "Ctrl+S"),
                                    new HotkeyDefinition("Export File", "Export the currently opened map", "FileExport", "Ctrl+E"),
                                    new HotkeyDefinition("Compile Map", "Compile the currently opened map", "FileCompile", "F9"),

                                    new HotkeyDefinition("Increase Grid Size", "Increase the current grid size", "GridIncrease", "]"),
                                    new HotkeyDefinition("Decrease Grid Size", "Decrease the current grid size", "GridDecrease", "["),

                                    new HotkeyDefinition("Undo", "Undo the last action", "HistoryUndo", "Ctrl+Z"),
                                    new HotkeyDefinition("Redo", "Redo the last undone action", "HistoryRedo", "Ctrl+Y"),

                                    new HotkeyDefinition("Show Object Properties", "Open the object properties dialog for the currently selected items", "ObjectProperties", "Alt+Enter"),
                               };
        }

        public static HotkeyDefinition GetHotkeyFor(string keyCombination)
        {
            return _definitions.FirstOrDefault(x => x.DefaultHotkey == keyCombination);
        }
    }
}
