using System;
using System.Collections.Generic;
using System.Linq;
using Sledge.Settings.Models;

namespace Sledge.Settings
{
    public static class Hotkeys
    {
        private static readonly List<HotkeyDefinition> Definitions;
        private static readonly List<HotkeyImplementation> Implementations;

        static Hotkeys()
        {
            Definitions = new List<HotkeyDefinition>
                               {
                                    new HotkeyDefinition("Autosize Views", "Reset the position of the 4-view splitter", HotkeysMediator.ViewportAutosize, "Ctrl+A"),
                                    new HotkeyDefinition("Focus View Top Left", "Focus on the 3D View", HotkeysMediator.FourViewFocusTopLeft, "F5"),
                                    new HotkeyDefinition("Focus View Top Right", "Focus on the XY View", HotkeysMediator.FourViewFocusTopRight, "F2"),
                                    new HotkeyDefinition("Focus View Bottom Left", "Focus on the YZ View", HotkeysMediator.FourViewFocusBottomLeft, "F4"),
                                    new HotkeyDefinition("Focus View Bottom Right", "Focus on the XZ View", HotkeysMediator.FourViewFocusBottomRight, "F3"),
                                    new HotkeyDefinition("Focus Current View", "Focus on the currently active view", HotkeysMediator.FourViewFocusCurrent, "Shift+Z"),

                                    new HotkeyDefinition("New File", "Create a new map", HotkeysMediator.FileNew, "Ctrl+N"),
                                    new HotkeyDefinition("Open File", "Open an existing map", HotkeysMediator.FileOpen, "Ctrl+O"),
                                    new HotkeyDefinition("Save File", "Save the currently opened map", HotkeysMediator.FileSave, "Ctrl+S"),
                                    new HotkeyDefinition("Save File As...", "Save the currently opened map", HotkeysMediator.FileSaveAs, "Ctrl+Alt+S"),
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
                                    
                                    new HotkeyDefinition("Copy", "Copy the current selection", HotkeysMediator.OperationsCopy, "Ctrl+C", "Ctrl+Ins"),
                                    new HotkeyDefinition("Cut", "Cut the current selection", HotkeysMediator.OperationsCut, "Ctrl+X", "Shift+Del"),
                                    new HotkeyDefinition("Paste", "Paste the clipboard contents", HotkeysMediator.OperationsPaste, "Ctrl+V", "Shift+Ins"),
                                    new HotkeyDefinition("Paste Special", "Paste special the clipboard contents", HotkeysMediator.OperationsPasteSpecial, "Ctrl+Shift+V"),
                                    new HotkeyDefinition("Delete", "Delete the current selection", HotkeysMediator.OperationsDelete, "Del"),
                                    
                                    new HotkeyDefinition("Group", "Group the selected objects", HotkeysMediator.GroupingGroup, "Ctrl+G"),
                                    new HotkeyDefinition("Ungroup", "Ungroup the selected objects", HotkeysMediator.GroupingUngroup, "Ctrl+U"),
                                    new HotkeyDefinition("Toggle Ignore Grouping", "Toggle ignore grouping on and off", HotkeysMediator.ToggleIgnoreGrouping, "Ctrl+W"),
                                    
                                    new HotkeyDefinition("Hide Selected", "Hide the selected objects", HotkeysMediator.QuickHideSelected, "H"),
                                    new HotkeyDefinition("Hide Unselected", "Hide the unselected objects", HotkeysMediator.QuickHideUnselected, "Ctrl+H"),
                                    new HotkeyDefinition("Unhide All", "Show all hidden objects", HotkeysMediator.QuickHideShowAll, "U"),
                                    
                                    new HotkeyDefinition("Rotate Selection Clockwise", "Rotate the selected objects 90 degrees clockwise", HotkeysMediator.RotateClockwise, "N"),
                                    new HotkeyDefinition("Rotate Selection Counter-Clockwise", "Rotate the selected objects 90 degrees counter-clockwise", HotkeysMediator.RotateCounterClockwise, "M"),

                                    new HotkeyDefinition("Create New Visgroup", "Create a new visgroup", HotkeysMediator.VisgroupCreateNew, "Alt+V"),
                                    
                                    new HotkeyDefinition("Center 2D View on Selection", "Center the 2D viewports on the current selection", HotkeysMediator.Center2DViewsOnSelection, "Ctrl+E"),
                                    new HotkeyDefinition("Center 3D View on Selection", "Center the 3D viewport on the current selection", HotkeysMediator.Center3DViewsOnSelection, "Ctrl+Shift+E"),
                                    
                                    new HotkeyDefinition("Deselect All", "Deselect all currently selected objects", HotkeysMediator.SelectionClear, "Shift+Q", "Escape"),
                                    
                                    new HotkeyDefinition("Tie to Entity", "Tie the selected objects to an entity", HotkeysMediator.TieToEntity, "Ctrl+T"),
                                    new HotkeyDefinition("Move to World", "Move the selected entities to the world", HotkeysMediator.TieToWorld, "Ctrl+Shift+W"),

                                    new HotkeyDefinition("Carve", "Carve the selected objects", HotkeysMediator.Carve, "Ctrl+Shift+C"),
                                    new HotkeyDefinition("Make Hollow", "Make the selected object hollow", HotkeysMediator.MakeHollow, "Ctrl+Shift+H"),

                                    new HotkeyDefinition("Toggle Texture Lock", "Toggles texture locking on and off", HotkeysMediator.ToggleTextureLock, "Shift+L"),
                                    new HotkeyDefinition("Transform", "Open the 'Transform' dialog", HotkeysMediator.Transform, "Ctrl+M"),
                                    new HotkeyDefinition("Check for Problems", "Open the 'Check for Problems' dialog", HotkeysMediator.CheckForProblems, "Alt+P"),
                                    new HotkeyDefinition("Go to Brush ID", "Open the 'Go to Brush ID' dialog", HotkeysMediator.GoToBrushID, "Ctrl+Shift+G"),
                                    
                                    new HotkeyDefinition("Flip X", "Flip selection along the X axis", HotkeysMediator.FlipX, "Ctrl+L"),
                                    new HotkeyDefinition("Flip Y", "Flip selection along the Y axis", HotkeysMediator.FlipY, "Ctrl+I"),
                                    new HotkeyDefinition("Flip Z", "Flip selection along the Z axis", HotkeysMediator.FlipZ, "Ctrl+K"),
                                    
                                    new HotkeyDefinition("Selection Tool", "Switch to the selection tool", HotkeysMediator.SwitchTool, HotkeyTool.Selection, "Shift+S"),
                                    new HotkeyDefinition("Camera Tool", "Switch to the camera tool", HotkeysMediator.SwitchTool, HotkeyTool.Camera, "Shift+C"),
                                    new HotkeyDefinition("Entity Tool", "Switch to the entity tool", HotkeysMediator.SwitchTool, HotkeyTool.Entity, "Shift+E"),
                                    new HotkeyDefinition("Brush Tool", "Switch to the brush tool", HotkeysMediator.SwitchTool, HotkeyTool.Brush, "Shift+B"),
                                    new HotkeyDefinition("Texture Tool", "Switch to the texture application tool", HotkeysMediator.SwitchTool, HotkeyTool.Texture, "Shift+A"),
                                    new HotkeyDefinition("Decal Tool", "Switch to the decal tool", HotkeysMediator.SwitchTool, HotkeyTool.Decal, "Shift+D"),
                                    new HotkeyDefinition("Clip Tool", "Switch to the clipping tool", HotkeysMediator.SwitchTool, HotkeyTool.Clip, "Shift+X"),
                                    new HotkeyDefinition("Vertex Manipulation Tool", "Switch to the vertex manipulation tool", HotkeysMediator.SwitchTool, HotkeyTool.VM, "Shift+V"),
                                    new HotkeyDefinition("Cordon Tool", "Switch to the cordon tool", HotkeysMediator.SwitchTool, HotkeyTool.Cordon, "Shift+K"),
                                    
                                    new HotkeyDefinition("Apply Current Texture", "Apply the current texture to the selection.", HotkeysMediator.ApplyCurrentTextureToSelection, "Shift+T"),
                                    
                                    new HotkeyDefinition("Vertex Manipulation Standard Mode", "Switch to the standard mode while in the VM tool.", HotkeysMediator.VMStandardMode, "Alt+W"),
                                    new HotkeyDefinition("Vertex Manipulation Scaling Mode", "Switch to the scaling mode while in the VM tool.", HotkeysMediator.VMScalingMode, "Alt+E"),
                                    new HotkeyDefinition("Vertex Manipulation Face Edit Mode", "Switch to the face edit mode while in the VM tool.", HotkeysMediator.VMFaceEditMode, "Alt+R"),
                                    new HotkeyDefinition("Vertex Manipulation Split Face", "Perform a split operation in standard mode while in the VM tool.", HotkeysMediator.VMSplitFace, "Ctrl+F"),

                                    new HotkeyDefinition("Next Camera", "Switch to the next camera in the camera tool.", HotkeysMediator.CameraNext, "Tab", "PgDn"),
                                    new HotkeyDefinition("Previous Camera", "Switch to the previous camera in the camera tool.", HotkeysMediator.CameraPrevious, "PgUp"),
                                    
                                    new HotkeyDefinition("Previous Tab", "Move to the previous open tab", HotkeysMediator.PreviousTab, "Ctrl+Shift+Tab"),
                                    new HotkeyDefinition("Next Tab", "Move to the next open tab", HotkeysMediator.NextTab, "Ctrl+Tab"),
                               };
            Implementations = new List<HotkeyImplementation>();
            SetupHotkeys(new List<Hotkey>());
        }

        public static void SetupHotkeys(List<Hotkey> overrides)
        {
            Implementations.Clear();
            foreach (var def in Definitions)
            {
                var overridden = false;
                foreach (var hk in overrides.Where(x => x.ID == def.ID).ToList())
                {
                    overridden = true;
                    if (!String.IsNullOrWhiteSpace(hk.HotkeyString))
                    {
                        Implementations.Add(new HotkeyImplementation(def, hk.HotkeyString));
                    }
                }
                if (!overridden)
                {
                    foreach (var hk in def.DefaultHotkeys)
                    {
                        Implementations.Add(new HotkeyImplementation(def, hk));
                    }
                }
            }
        }

        public static IEnumerable<Hotkey> GetHotkeys()
        {
            foreach (var def in Definitions)
            {
                var impls = Implementations.Where(x => x.Definition.ID == def.ID).ToList();
                if (!impls.Any())
                {
                    yield return new Hotkey {ID = def.ID, HotkeyString = ""};
                }
                else
                {
                    foreach (var impl in impls)
                    {
                        yield return new Hotkey {ID = def.ID, HotkeyString = impl.Hotkey};
                    }
                }
            }
        }

        public static HotkeyImplementation GetHotkeyForMessage(object message, object parameter)
        {
            var def = Definitions.FirstOrDefault(x => x.Action.ToString() == message.ToString() && Equals(x.Parameter, parameter));
            if (def == null) return null;
            return Implementations.FirstOrDefault(x => x.Definition.ID == def.ID);
        }

        public static HotkeyImplementation GetHotkeyFor(string keyCombination)
        {
            return Implementations.FirstOrDefault(x => x.Hotkey == keyCombination);
        }

        public static HotkeyDefinition GetHotkeyDefinition(string id)
        {
            return Definitions.FirstOrDefault(x => x.ID == id);
        }

        public static IEnumerable<HotkeyDefinition> GetHotkeyDefinitions()
        {
            return Definitions;
        }
    }
}
