namespace Sledge.Settings
{
    public static class Select
    {
        public static bool SnapRotationTo15DegreesByDefault { get; set; }
        public static bool KeepGroupsWhenCloning { get; set; }
        public static bool ArrowKeysNudgeSelection { get; set; }
        public static bool SelectByCenterHandlesOnly { get; set; }

        static Select()
        {
            SnapRotationTo15DegreesByDefault = false;
            KeepGroupsWhenCloning = true;
            ArrowKeysNudgeSelection = false;
            SelectByCenterHandlesOnly = false;
        }
    }
}
