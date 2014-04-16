using System.Drawing;

namespace Sledge.Settings
{
    public class View
    {
        public static bool CrosshairCursorIn2DViews { get; set; }
        public static decimal ScrollWheelZoomMultiplier { get; set; }
        public static int SelectionBoxBackgroundOpacity { get; set; }
        public static bool DrawBoxText { get; set; }
        public static bool DrawBoxDashedLines { get; set; }
        public static bool DrawEntityNames { get; set; }
        
        public static Color ViewportBackground { get; set; }

        public static int BackClippingPane { get; set; }
        public static int ModelRenderDistance { get; set; }
        public static int DetailRenderDistance { get; set; }

        public static bool Camera2DPanRequiresMouseClick { get; set; }
        public static bool Camera3DPanRequiresMouseClick { get; set; }

        public static int ForwardSpeed { get; set; }
        public static decimal TimeToTopSpeed { get; set; }
        public static decimal MouseWheelMoveDistance { get; set; }
        public static bool InvertX { get; set; }
        public static bool InvertY { get; set; }

        public static int CameraFOV { get; set; }

        public static bool LoadSession { get; set; }
        public static bool KeepCameraPositions { get; set; }
        public static bool KeepSelectedTool { get; set; }

        public static RenderMode Renderer { get; set; }
        public static bool DisableWadTransparency { get; set; }
        public static bool DisableToolTextureTransparency { get; set; }
        public static bool GloballyDisableTransparency { get; set; }
        public static bool DisableModelRendering { get; set; }
        public static bool DisableTextureFiltering { get; set; }

        public static bool CompileOpenOutput { get; set; }
        public static bool CompileDefaultAdvanced { get; set; }

        static View()
        {
            CrosshairCursorIn2DViews = false;
            ScrollWheelZoomMultiplier = 1.2m;
            SelectionBoxBackgroundOpacity = 64;
            DrawBoxText = true;
            DrawBoxDashedLines = false;
            DrawEntityNames = true;

            ViewportBackground = Color.Black;

            Camera2DPanRequiresMouseClick = false;
            Camera3DPanRequiresMouseClick = false;

            BackClippingPane = 6000;
            ModelRenderDistance = 2000;
            DetailRenderDistance = 2000;

            ForwardSpeed = 1000;
            TimeToTopSpeed = 0.5m;
            MouseWheelMoveDistance = 500;

            InvertX = false;
            InvertY = false;

            CameraFOV = 60;

            LoadSession = true;
            KeepCameraPositions = false;
            KeepSelectedTool = false;

            Renderer = RenderMode.OpenGL3;
            DisableWadTransparency = false;
            DisableToolTextureTransparency = false;
            GloballyDisableTransparency = false;
            DisableModelRendering = false;
            DisableTextureFiltering = false;

            CompileOpenOutput = true;
            CompileDefaultAdvanced = false;
        }
    }
}