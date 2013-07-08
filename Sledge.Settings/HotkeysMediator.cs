namespace Sledge.Settings
{
    public enum HotkeysMediator
    {
        FourViewAutosize,
        FourViewFocusTopLeft,
        FourViewFocusTopRight,
        FourViewFocusBottomLeft,
        FourViewFocusBottomRight,

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

        ShowMapInformation,
        ShowEntityReport,
        CheckForProblems,

        QuickLoadPointfile,
        LoadPointfile,
        UnloadPointfile,
    }
}