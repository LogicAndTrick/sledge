using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using Sledge.Rendering.Scenes.Elements;

namespace Sledge.Rendering.Internal
{
    internal static class InternalTextures
    {
        private static Bitmap WhitePixel { get; set; }
        private static Bitmap DebugTexture { get; set; }
        private static Bitmap SquareHandleTexture { get; set; }

        static InternalTextures()
        {
            WhitePixel = new Bitmap(1, 1);
            using (var g = Graphics.FromImage(WhitePixel))
            {
                g.Clear(Color.White);
            }

            DebugTexture = new Bitmap(100, 100);
            using (var g = Graphics.FromImage(DebugTexture))
            {
                g.Clear(Color.White);
                g.FillRectangle(Brushes.Blue, 10, 10, 80, 80);
                g.FillRectangle(Brushes.Orange, 20, 20, 60, 60);
                g.FillRectangle(Brushes.Purple, 30, 30, 40, 40);
                g.FillRectangle(Brushes.Yellow, 40, 40, 20, 20);
            }

            SquareHandleTexture = new Bitmap(8, 8);
            using (var g = Graphics.FromImage(SquareHandleTexture))
            {
                g.SmoothingMode = SmoothingMode.HighSpeed;
                g.Clear(Color.White);
                g.DrawRectangle(Pens.Black, 0, 0, 7, 7);
            }
        }

        public static Dictionary<string, Bitmap> GetInternalTextures()
        {
            return new Dictionary<string, Bitmap>
            {
                { "Internal::White", WhitePixel  },
                { "Internal::Debug", DebugTexture  },
                { HandleElement.SquareHandleTextureName, SquareHandleTexture },
            };
        }
    }
}
