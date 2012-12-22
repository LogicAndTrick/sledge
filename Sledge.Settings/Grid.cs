using System.Drawing;

namespace Sledge.Settings
{
    public static class Grid
    {
        public static Color Background { get; set; }
        public static Color GridLines { get; set; }
        public static Color ZeroLines { get; set; }
        public static Color BoundaryLines { get; set; }

        public static bool Highlight1On { get; set; }
        public static int Highlight1LineNum { get; set; }
        public static Color Highlight1 { get; set; }

        public static bool Highlight2On { get; set; }
        public static int Highlight2UnitNum { get; set; }
        public static Color Highlight2 { get; set; }

        public static int DefaultSize { get; set; }

        public static bool HideSmallerOn { get; set; }
        public static int HideSmallerThan { get; set; }
        public static int HideFactor { get; set; }

        static Grid()
        {
            Background = Color.Black;
            GridLines = Color.FromArgb(75,75,75);
            ZeroLines = Color.FromArgb(0,100,100);
            BoundaryLines = Color.Red;

            Highlight1On = true;
            Highlight1LineNum = 8;
            Highlight1 = Color.FromArgb(115,115,115);

            Highlight2On = true;
            Highlight2UnitNum = 1024;
            Highlight2 = Color.FromArgb(100,46,0);

            DefaultSize = 4;

            HideSmallerOn = true;
            HideSmallerThan = 4;
            HideFactor = 8;
        }
    }
}
