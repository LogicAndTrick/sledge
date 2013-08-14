namespace Sledge.Settings
{
    public enum HotkeysMediator
    {
        FourViewAutosize,
        FourViewFocusTopLeft,
        FourViewFocusTopRight,
        FourViewFocusBottomLeft,
        FourViewFocusBottomRight,
        FourViewFocusCurrent,

        FileNew,
        FileOpen,
        FileClose,
        FileSave,
        FileSaveAs,
        FileCompile,

        HistoryUndo,
        HistoryRedo,

        OperationsCopy,
        OperationsCut,
        OperationsPaste,
        OperationsPasteSpecial,
        OperationsDelete,

        SelectAll,
        SelectionClear,

        ObjectProperties,

        QuickHideSelected,
        QuickHideUnselected,
        QuickHideShowAll,

        SwitchTool,
        ApplyCurrentTextureToSelection,

        Carve,
        MakeHollow,

        GroupingGroup,
        GroupingUngroup,

        TieToEntity,
        TieToWorld,

        Transform,

        ReplaceTextures,

        SnapSelectionToGrid,
        SnapSelectionToGridIndividually,

        AlignXMin,
        AlignXMax,
        AlignYMin,
        AlignYMax,
        AlignZMin,
        AlignZMax,

        FlipX,
        FlipY,
        FlipZ,

        GridIncrease,
        GridDecrease,

        CenterAllViewsOnSelection,
        Center3DViewsOnSelection,
        Center2DViewsOnSelection,

        GoToCoordinates,
        GoToBrushID,
        ShowSelectedBrushID,

        ToggleSnapToGrid,
        ToggleShow2DGrid,
        ToggleShow3DGrid,
        ToggleIgnoreGrouping,
        ToggleTextureLock,
        ToggleTextureScalingLock,
        ToggleCordon,
        ToggleHideFaceMask,
        ToggleHideDisplacementSolids,
        ToggleHideNullTextures,

        ShowMapInformation,
        ShowEntityReport,
        CheckForProblems,

        QuickLoadPointfile,
        LoadPointfile,
        UnloadPointfile,

        // Tool specific

        VMStandardMode,
        VMScalingMode,
        VMFaceEditMode,
        VMSplitFace,

        // Tabs

        PreviousTab,
        NextTab,
    }
}