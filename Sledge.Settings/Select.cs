namespace Sledge.Settings
{
    public static class Select
    {
        public static SnapStyle SnapStyle { get; set; }
        public static RotationStyle RotationStyle { get; set; }

        public static bool ArrowKeysNudgeSelection { get; set; }
        public static decimal NudgeUnits { get; set; }

        public static bool KeepGroupsWhenCloning { get; set; }
        public static bool SelectByCenterHandlesOnly { get; set; }

        static Select()
        {
            SnapStyle = SnapStyle.SnapOffAlt;
            RotationStyle = RotationStyle.SnapOnShift;

            ArrowKeysNudgeSelection = false;
            NudgeUnits = 1;

            KeepGroupsWhenCloning = true;
            SelectByCenterHandlesOnly = false;
        }
    }
}
