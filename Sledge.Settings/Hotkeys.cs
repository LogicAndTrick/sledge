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
                                    new HotkeyDefinition("Focus Current View", "Focus on the currently active view", HotkeysMediator.FourViewFocusCurrent, "Shift+Z"),

                                    new HotkeyDefinition("New File", "Create a new map", HotkeysMediator.FileNew, "Ctrl+N"),
                                    new HotkeyDefinition("Open File", "Open an existing map", HotkeysMediator.FileOpen, "Ctrl+O"),
                                    new HotkeyDefinition("Save File", "Save the currently opened map", HotkeysMediator.FileSave, "Ctrl+S"),
                                    new HotkeyDefinition("Save File As...", "Save the currently opened map", HotkeysMediator.FileSave, "Ctrl+Alt+S"),
                                    new HotkeyDefinition("Compile Map", "Compile the currently opened map", HotkeysMediator.FileCompile, "F9"),

                                    new HotkeyDefinition("Increase Grid Size", "Increase the current grid size", HotkeysMediator.GridIncrease, "]"),
                                    new HotkeyDefinition("Decrease Grid Size", "Decrease the current grid size", HotkeysMediator.GridDecrease, "["),
                                    new HotkeyDefinition("Toggle 2D Grid", "Toggle the 2D viewport grid on and off", HotkeysMediator.ToggleShow2DGrid, "Shift+R"),
                                    new HotkeyDefinition("Toggle 3D Grid", "Toggle the 3D viewport grid on and off", HotkeysMediator.ToggleShow3DGrid, "P"),
                                    new HotkeyDefinition("Toggle Snap to Grid", "Toggle grid snapping on and off", HotkeysMediator.ToggleSnapToGrid, "Shift+W"),
                                    new HotkeyDefinition("Snap Selection to Grid", "Snap current selection to the grid based on the selection bounding box", HotkeysMediator.SnapSelectionToGrid, "Ctrl+B"),
                                    new HotkeyDefinition("Snap Selection to Grid Individually", "Snap each selected object to the grid based on their individual bounding boxes", HotkeysMediator.SnapSelectionToGridIndividually, "Ctrl+Shift+B"),

                                    new HotkeyDefinition("Undo", "Undo the last action", HotkeysMediator.HistoryUndo, "Ctrl+Z"),
                                    new HotkeyDefinition("Redo", "Redo the last undone action", HotkeysMediator.HistoryRedo, "Ctrl+Y"),

                                    new HotkeyDefinition("Show Object Properties", "Open the object properties dialog for the currently selected items", HotkeysMediator.ObjectProperties, "Alt+Enter"),
                                    
                                    new HotkeyDefinition("Copy", "Copy the current selection", HotkeysMediator.OperationsCopy, "Ctrl+C"),
                                    new HotkeyDefinition("Copy", "Copy the current selection", HotkeysMediator.OperationsCopy, "Ctrl+Ins"),
                                    new HotkeyDefinition("Cut", "Cut the current selection", HotkeysMediator.OperationsCut, "Ctrl+X"),
                                    new HotkeyDefinition("Cut", "Cut the current selection", HotkeysMediator.OperationsCut, "Shift+Del"),
                                    new HotkeyDefinition("Paste", "Paste the clipboard contents", HotkeysMediator.OperationsPaste, "Ctrl+V"),
                                    new HotkeyDefinition("Paste", "Paste the clipboard contents", HotkeysMediator.OperationsPaste, "Shift+Ins"),
                                    new HotkeyDefinition("Paste Special", "Paste special the clipboard contents", HotkeysMediator.OperationsPasteSpecial, "Ctrl+B"),
                                    new HotkeyDefinition("Delete", "Delete the current selection", HotkeysMediator.OperationsDelete, "Del"),
                                    
                                    new HotkeyDefinition("Group", "Group the selected objects", HotkeysMediator.GroupingGroup, "Ctrl+G"),
                                    new HotkeyDefinition("Ungroup", "Ungroup the selected objects", HotkeysMediator.GroupingUngroup, "Ctrl+U"),
                                    
                                    new HotkeyDefinition("Hide Selected", "Hide the selected objects", HotkeysMediator.QuickHideSelected, "H"),
                                    new HotkeyDefinition("Hide Unselected", "Hide the unselected objects", HotkeysMediator.QuickHideUnselected, "Ctrl+H"),
                                    new HotkeyDefinition("Unhide All", "Show all hidden objects", HotkeysMediator.QuickHideShowAll, "U"),
                                    
                                    new HotkeyDefinition("Tie To Entity", "Tie the selected objects to an entity", HotkeysMediator.TieToEntity, "Ctrl+T"),
                                    new HotkeyDefinition("Ungroup", "Ungroup the selected objects", HotkeysMediator.GroupingUngroup, "Ctrl+H"),
                                    
                                    new HotkeyDefinition("Center 2D View on Selection", "Center the 2D viewports on the current selection", HotkeysMediator.Center2DViewsOnSelection, "Ctrl+E"),
                                    new HotkeyDefinition("Center 3D View on Selection", "Center the 3D viewport on the current selection", HotkeysMediator.Center3DViewsOnSelection, "Ctrl+Shift+E"),
                                    
                                    new HotkeyDefinition("Deselect All", "Deselect all currently selected objects", HotkeysMediator.SelectionClear, "Shift+Q"),
                                    new HotkeyDefinition("Deselect All", "Deselect all currently selected objects", HotkeysMediator.SelectionClear, "Esc"),
                                    
                                    new HotkeyDefinition("Tie to Entity", "Tie the selected objects to an entity", HotkeysMediator.TieToEntity, "Ctrl+T"),
                                    new HotkeyDefinition("Move to World", "Move the selected entities to the world", HotkeysMediator.TieToWorld, "Ctrl+Shift+W"),

                                    new HotkeyDefinition("Toggle Texture Lock", "Toggles texture locking on and off", HotkeysMediator.ToggleTextureLock, "Shift+L"),
                                    new HotkeyDefinition("Transform", "Open the 'Transform' dialog", HotkeysMediator.Transform, "Ctrl+M"),
                                    new HotkeyDefinition("Check for Problems", "Open the 'Check for Problems' dialog", HotkeysMediator.CheckForProblems, "Alt+P"),
                                    new HotkeyDefinition("Go to Brush ID", "Open the 'Go to Brush ID' dialog", HotkeysMediator.GoToBrushID, "Ctrl+Shift+G"),

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
