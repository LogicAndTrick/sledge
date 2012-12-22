namespace Sledge.Settings
{
    public class View
    {
        public static bool CrosshairCursorIn2DViews { get; set; }

        public static int BackClippingPane { get; set; }
        public static int ForwardSpeed { get; set; }
        public static decimal TimeToTopSpeed { get; set; }

        public static bool InvertX { get; set; }
        public static bool InvertY { get; set; }

        static View()
        {
            CrosshairCursorIn2DViews = false;

            BackClippingPane = 4000;
            ForwardSpeed = 1000;
            TimeToTopSpeed = 0.5m;

            InvertX = false;
            InvertY = false;
        }
    }
}