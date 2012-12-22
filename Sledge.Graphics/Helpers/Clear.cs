using System.Drawing;
using OpenTK.Graphics.OpenGL;

namespace Sledge.Graphics.Helpers
{
    public static class Clear
    {
        public static Color Current { get; set; }

        static Clear()
        {
            Current = Color.Black;
        }

        public static void Wipe()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }

        public static void Wipe(Color color)
        {
            GL.ClearColor(color);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor(Current);
        }
    }
}
