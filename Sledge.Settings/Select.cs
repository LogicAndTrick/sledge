namespace Sledge.Settings
{
    public static class Select
    {
        public static SnapStyle SnapStyle { get; set; }
        public static RotationStyle RotationStyle { get; set; }

        public static bool ArrowKeysNudgeSelection { get; set; }
        public static decimal NudgeUnits { get; set; }
        public static NudgeStyle NudgeStyle { get; set; }

        public static bool KeepGroupsWhenCloning { get; set; }
        public static bool SelectByCenterHandlesOnly { get; set; }

        static Select()
        {
            SnapStyle = SnapStyle.SnapOffAlt;
            RotationStyle = RotationStyle.SnapOnShift;

            ArrowKeysNudgeSelection = true;
            NudgeUnits = 1;
            NudgeStyle = NudgeStyle.GridOnCtrl;

            KeepGroupsWhenCloning = true;
            SelectByCenterHandlesOnly = false;
        }
    }
}
