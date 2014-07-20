using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using Sledge.Common;

namespace Sledge.Providers.Texture
{
    public class NullTextureStreamSource : ITextureStreamSource
    {
        private static readonly Bitmap PlaceholderImage;
        static NullTextureStreamSource()
        {
            PlaceholderImage = new Bitmap(64, 64, PixelFormat.Format32bppArgb);
            using (var g = System.Drawing.Graphics.FromImage(PlaceholderImage))
            {
                g.FillRectangle(Brushes.Black, 0, 0, 64, 64);
                for (var i = 0; i < 64; i++)
                {
                    var x = i % 8;
                    var y = i / 8;
                    if (y % 2 == x % 2) continue;
                    g.FillRectangle(Brushes.Magenta, x * 8, y * 8, 8, 8);
                }
            }
        }

        private readonly int _maxWidth;
        private readonly int _maxHeight;

        public NullTextureStreamSource(int maxWidth, int maxHeight)
        {
            _maxWidth = maxWidth;
            _maxHeight = maxHeight;
        }

        public bool HasImage(TextureItem item)
        {
            return item.Flags.HasFlag(TextureFlags.Missing);
        }

        public Bitmap GetImage(TextureItem item)
        {
            lock (PlaceholderImage)
            {
                return new Bitmap(PlaceholderImage);
            }
        }

        public void Dispose()
        {

        }
    }
}
