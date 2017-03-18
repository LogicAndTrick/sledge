using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace Sledge.Providers.Texture
{
    public class NullTextureStreamSource : ITextureStreamSource
    {
        private static readonly Bitmap PlaceholderImage;
        static NullTextureStreamSource()
        {
            PlaceholderImage = new Bitmap(16, 16, PixelFormat.Format32bppArgb);
            using (var g = Graphics.FromImage(PlaceholderImage))
            {
                g.FillRectangle(Brushes.Black, 0, 0, 16, 16);
                g.FillRectangle(Brushes.Magenta, 8, 0, 8, 8);
                g.FillRectangle(Brushes.Magenta, 0, 8, 8, 8);
            }
        }

        public bool HasImage(string item)
        {
            return true;
        }

        public Task<Bitmap> GetImage(string item, int maxWidth, int maxHeight)
        {
            return Task.Factory.StartNew(() =>
            {
                lock (PlaceholderImage)
                {
                    return new Bitmap(PlaceholderImage);
                }
            });
        }

        public void Dispose()
        {

        }
    }
}
