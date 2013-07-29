namespace Sledge.Settings
{
    public static class Select
    {
        public static SnapStyle SnapStyle { get; set; }
        public static RotationStyle RotationStyle { get; set; }

        public static bool ArrowKeysNudgeSelection { get; set; }
        public static decimal NudgeUnits { get; set; }
        public static NudgeStyle NudgeStyle { get; set; }

        public static bool AutoSelectBox { get; set; }
        public static bool SwitchToSelectAfterCreation { get; set; }
        public static bool SelectCreatedBrush { get; set; }
        public static bool KeepGroupsWhenCloning { get; set; }

        public static bool DrawCenterHandles { get; set; }
        public static bool ClickSelectByCenterHandlesOnly { get; set; }
        public static bool BoxSelectByCenterHandlesOnly { get; set; }

        static Select()
        {
            SnapStyle = SnapStyle.SnapOffAlt;
            RotationStyle = RotationStyle.SnapOnShift;

            ArrowKeysNudgeSelection = true;
            NudgeUnits = 1;
            NudgeStyle = NudgeStyle.GridOnCtrl;

            AutoSelectBox = false;
            SwitchToSelectAfterCreation = false;
            SelectCreatedBrush = true;
            KeepGroupsWhenCloning = true;

            DrawCenterHandles = false;
            ClickSelectByCenterHandlesOnly = false;
            BoxSelectByCenterHandlesOnly = false;
        }
    }
}
