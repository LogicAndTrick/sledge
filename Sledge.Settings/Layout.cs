namespace Sledge.Settings
{
    public static class Layout
    {
        public static int SidebarWidth { get; set; }
        public static string ViewportTopLeft { get; set; }
        public static string ViewportTopRight { get; set; }
        public static string ViewportBottomLeft { get; set; }
        public static string ViewportBottomRight { get; set; }

        static Layout()
        {
            SidebarWidth = 250;
        }
    }
}