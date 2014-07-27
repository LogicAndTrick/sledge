using System;
using System.ComponentModel;

namespace Sledge.Settings
{
    public class MenuLocationAttribute : Attribute
    {
        
    }

    public enum HotkeysMediator
    {

        [Description("New")]
        FileNew,
        [Description("Open")]
        FileOpen,
        [Description("Close")]
        FileClose,
        [Description("Save")]
        FileSave,
        [Description("Save As...")]
        FileSaveAs,
        [Description("Export...")]
        FileExport,
        [Description("Run")]
        FileCompile,

        [Description("Undo")]
        HistoryUndo,
        [Description("Redo")]
        HistoryRedo,

        [Description("Copy")]
        OperationsCopy,
        [Description("Cut")]
        OperationsCut,
        [Description("Paste")]
        OperationsPaste,
        [Description("Paste Special...")]
        OperationsPasteSpecial,
        [Description("Delete")]
        OperationsDelete,

        [Description("Select All")]
        SelectAll,
        [Description("Clear Selection")]
        SelectionClear,

        [Description("Object Properties")]
        ObjectProperties,

        [Description("Apply Current Texture to Selection")]
        ApplyCurrentTextureToSelection,

        [Description("Toggle Snap to Grid")]
        ToggleSnapToGrid,
        [Description("Toggle Show 2D Grid")]
        ToggleShow2DGrid,
        [Description("Toggle Show 3D Grid")]
        ToggleShow3DGrid,
        [Description("Bigger Grid")]
        GridIncrease,
        [Description("Smaller Grid")]
        GridDecrease,

        [Description("Toggle Ignore Grouping")]
        ToggleIgnoreGrouping,
        [Description("Toggle Texture Lock")]
        ToggleTextureLock,
        [Description("Toggle Texture Scaling Lock")]
        ToggleTextureScalingLock,
        [Description("Toggle Cordon")]
        ToggleCordon,
        [Description("Toggle Hide Face Mask")]
        ToggleHideFaceMask,
        [Description("Toggle Hide Displacement Solids")]
        ToggleHideDisplacementSolids,
        [Description("Toggle Hide Null Textures")]
        ToggleHideNullTextures,

        [Description("Show Logical Tree")]
        ShowLogicalTree,
        [Description("Show Map Information")]
        ShowMapInformation,
        [Description("Show Entity Report")]
        ShowEntityReport,
        [Description("Check For Problems")]
        CheckForProblems,

        [Description("Quick Load Pointfile")]
        QuickLoadPointfile,
        [Description("Load Pointfile...")]
        LoadPointfile,
        [Description("Unload Pointfile")]
        UnloadPointfile,

        [Description("Autosize 4 Viewports")]
        ViewportAutosize,
        [Description("Maximize Top Left Viewport")]
        FourViewFocusTopLeft,
        [Description("Maximize Top Right Viewport")]
        FourViewFocusTopRight,
        [Description("Maximize Bottom Left Viewport")]
        FourViewFocusBottomLeft,
        [Description("Maximize Bottom Right Viewport")]
        FourViewFocusBottomRight,
        [Description("Maximize Current Viewport")]
        FourViewFocusCurrent,
        
        [Description("Screenshot Current Viewport")]
        ScreenshotViewport,

        [Description("Center All Views on Selection")]
        CenterAllViewsOnSelection,
        [Description("Center 3D Views on Selection")]
        Center3DViewsOnSelection,
        [Description("Center 2D Views on Selection")]
        Center2DViewsOnSelection,

        [Description("Go to Coordinates...")]
        GoToCoordinates,
        [Description("Go to Brush ID...")]
        GoToBrushID,
        [Description("Show Selected Brush ID")]
        ShowSelectedBrushID,

        [Description("Hide Selected Objects")]
        QuickHideSelected,
        [Description("Hide Unselected Objects")]
        QuickHideUnselected,
        [Description("Show Hidden Objects")]
        QuickHideShowAll,

        [Description("Rotate Selected Objects Clockwise")]
        RotateClockwise,
        [Description("Rotate Selected Objects Counter-Clockwise")]
        RotateCounterClockwise,

        [Description("Create New Visgroup")]
        VisgroupCreateNew,

        [Description("Carve")]
        Carve,
        [Description("Make Hollow")]
        MakeHollow,

        [Description("Group")]
        GroupingGroup,
        [Description("Ungroup")]
        GroupingUngroup,

        [Description("Tie to Entity")]
        TieToEntity,
        [Description("Move to World")]
        TieToWorld,

        [Description("Transform...")]
        Transform,

        [Description("Replace Textures")]
        ReplaceTextures,

        [Description("Snap Selected to Grid")]
        SnapSelectionToGrid,
        [Description("Snap Selected to Grid Individually")]
        SnapSelectionToGridIndividually,

        [Description("Align to X Axis Min")]
        AlignXMin,
        [Description("Align to X Axis Max")]
        AlignXMax,
        [Description("Align to Y Axis Min")]
        AlignYMin,
        [Description("Align to Y Axis Max")]
        AlignYMax,
        [Description("Align to Z Axis Min")]
        AlignZMin,
        [Description("Align to Z Axis Max")]
        AlignZMax,

        [Description("Flip on X Axis")]
        FlipX,
        [Description("Flip on Y Axis")]
        FlipY,
        [Description("Flip on Z Axis")]
        FlipZ,


        SwitchTool,

        // Tool specific

        [Description("Vertex Manipulation | Standard Mode")]
        VMStandardMode,
        [Description("Vertex Manipulation | Scaling Mode")]
        VMScalingMode,
        [Description("Vertex Manipulation | Face Edit Mode")]
        VMFaceEditMode,
        [Description("Vertex Manipulation | Split Face")]
        VMSplitFace,

        [Description("Camera | Next Camera")]
        CameraNext,
        [Description("Camera | Previous Camera")]
        CameraPrevious,

        // Tabs

        [Description("Go to Previous Tab")]
        PreviousTab,
        [Description("Go to Next Tab")]
        NextTab
    }
}