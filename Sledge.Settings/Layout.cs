namespace Sledge.Settings
{
    public static class Layout
    {
        public static int SidebarWidth { get; set; }
        public static string SidebarLayout { get; set; }

        static Layout()
        {
            SidebarWidth = 250;
            SidebarLayout = "";
        }
    }
}