using System.Drawing;
using OpenTK.Graphics;
using GL = OpenTK.Graphics.OpenGL.GL;

namespace Sledge.Graphics.Helpers
{
    /// <summary>
    /// The text printer wraps around OpenTK's TextPrinter, which may be replaced in the future
    /// as it has been marked as obsolete.
    /// </summary>
    public class Text
    {
        private static TextPrinter instance;

        static Text()
        {
            instance = new TextPrinter();
        }

        /// <summary>
        /// Print a string. GL drawing must be ended before calling this function.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="font"></param>
        /// <param name="color"></param>
        public static void Print(string text, Font font, Color color)
        {
            instance.Begin();
            instance.Print(text, font, color);
            instance.End();
        }

        public static void Print(string text, Font font, Color color, double x, double y)
        {
            instance.Begin();
            GL.Translate(x, y, 0);
            instance.Print(text, font, color);
            instance.End();
        }

        public static void Dispose()
        {
            instance.Dispose();
        }

    }
}
