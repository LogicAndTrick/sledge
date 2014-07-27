namespace Sledge.Settings
{
    public static class Select
    {
        public static SnapStyle SnapStyle { get; set; }
        public static RotationStyle RotationStyle { get; set; }
        public static DoubleClick3DAction DoubleClick3DAction { get; set; }

        public static bool ArrowKeysNudgeSelection { get; set; }
        public static decimal NudgeUnits { get; set; }
        public static NudgeStyle NudgeStyle { get; set; }

        public static bool SwitchToSelectAfterCreation { get; set; }
        public static bool SelectCreatedBrush { get; set; }
        public static bool SwitchToSelectAfterEntity { get; set; }
        public static bool SelectCreatedEntity { get; set; }
        public static bool DeselectOthersWhenSelectingCreation { get; set; }
        public static bool ResetBrushTypeOnCreation { get; set; }

        public static bool AutoSelectBox { get; set; }
        public static bool KeepVisgroupsWhenCloning { get; set; }

        public static bool DrawCenterHandles { get; set; }
        public static bool CenterHandlesActiveViewportOnly { get; set; }
        public static bool CenterHandlesFollowCursor { get; set; }
        public static bool ClickSelectByCenterHandlesOnly { get; set; }
        public static bool BoxSelectByCenterHandlesOnly { get; set; }

        public static bool Show3DSelectionWidgets { get; set; }

        public static bool ApplyTextureImmediately { get; set; }

        public static bool OpenObjectPropertiesWhenCreatingEntity { get; set; }

        public static bool SkipSelectionInUndoStack { get; set; }
        public static bool SkipVisibilityInUndoStack { get; set; }
        public static int UndoStackSize { get; set; }

        static Select()
        {
            SnapStyle = SnapStyle.SnapOffAlt;
            RotationStyle = RotationStyle.SnapOnShift;
            DoubleClick3DAction = DoubleClick3DAction.ObjectProperties;

            ArrowKeysNudgeSelection = true;
            NudgeUnits = 1;
            NudgeStyle = NudgeStyle.GridOnCtrl;

            SwitchToSelectAfterCreation = false;
            SelectCreatedBrush = true;
            SwitchToSelectAfterEntity = false;
            SelectCreatedEntity = true;
            DeselectOthersWhenSelectingCreation = true;
            ResetBrushTypeOnCreation = true;

            AutoSelectBox = false;
            KeepVisgroupsWhenCloning = true;

            DrawCenterHandles = false;
            CenterHandlesActiveViewportOnly = true;
            CenterHandlesFollowCursor = true;
            ClickSelectByCenterHandlesOnly = false;
            BoxSelectByCenterHandlesOnly = false;

            Show3DSelectionWidgets = true;

            ApplyTextureImmediately = false;

            OpenObjectPropertiesWhenCreatingEntity = true;

            SkipSelectionInUndoStack = false;
            SkipVisibilityInUndoStack = false;
            UndoStackSize = 100;
        }
    }
}
