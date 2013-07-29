namespace Sledge.Settings
{
    public class View
    {
        public static bool CrosshairCursorIn2DViews { get; set; }

        public static int BackClippingPane { get; set; }
        public static int ModelRenderDistance { get; set; }
        public static int DetailRenderDistance { get; set; }

        public static int ForwardSpeed { get; set; }
        public static decimal TimeToTopSpeed { get; set; }
        public static bool InvertX { get; set; }
        public static bool InvertY { get; set; }

        public static int CameraFOV { get; set; }

        static View()
        {
            CrosshairCursorIn2DViews = false;

            BackClippingPane = 6000;
            ModelRenderDistance = 4000;
            DetailRenderDistance = 4000;

            ForwardSpeed = 1000;
            TimeToTopSpeed = 0.5m;

            InvertX = false;
            InvertY = false;

            CameraFOV = 60;
        }
    }
}