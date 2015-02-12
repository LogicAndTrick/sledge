using System.Drawing;

namespace Sledge.Rendering
{
    public static class Extensions
    {
        public static int ToAbgr(this Color color)
        {
            return (color.A << 24) | (color.B << 16) | (color.G << 8) | color.R;
        }
    }
}