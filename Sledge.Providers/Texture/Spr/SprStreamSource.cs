using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Sledge.Common.Extensions;
using Sledge.FileSystem;

namespace Sledge.Providers.Texture.Spr
{
    public class SprStreamSource : ITextureStreamSource
    {
        private readonly IFile _file;

        public SprStreamSource(IFile file)
        {
            _file = file;
        }

        public bool HasImage(string item)
        {
            return _file.TraversePath(item) != null;
        }

        private static Bitmap Parse(Stream stream)
        {
            // Sprite file spec taken from the spritegen source in the Half-Life SDK.
            using (var br = new BinaryReader(stream))
            {
                var idst = br.ReadFixedLengthString(Encoding.ASCII, 4);
                if (idst != "IDSP") return null;
                var version = br.ReadInt32();
                if (version != 2) return null;
                var type = (SpriteOrientation)br.ReadInt32();
                var texFormat = (SpriteRenderMode)br.ReadInt32();
                var boundingRadius = br.ReadSingle();
                var width = br.ReadInt32();
                var height = br.ReadInt32();
                var numframes = br.ReadInt32();
                var beamlength = br.ReadSingle();
                var synctype = br.ReadInt32();
                var paletteSize = br.ReadInt16();
                var palette = br.ReadBytes(paletteSize * 3);

                if (paletteSize > 256) paletteSize = 256; // Don't accept anything higher
                var colours = new Color[256];
                for (var i = 0; i < paletteSize; i++)
                {
                    var r = palette[i * 3 + 0];
                    var g = palette[i * 3 + 1];
                    var b = palette[i * 3 + 2];
                    colours[i] = Color.FromArgb(255, r, g, b);
                }

                // Only read the first frame.
                var frametype = br.ReadInt32();
                if (frametype != 0)
                {
                    var num = br.ReadInt32();
                    var intervals = br.ReadSingleArray(num);
                }
                var originX = br.ReadInt32();
                var originY = br.ReadInt32();
                var framewidth = br.ReadInt32();
                var frameheight = br.ReadInt32();
                var pixels = br.ReadBytes(framewidth * frameheight);

                var bitmap = new Bitmap(framewidth, frameheight, PixelFormat.Format8bppIndexed);

                // Pre-process the palette
                var pal = bitmap.Palette;
                var last = colours[255];
                for (var i = 0; i < paletteSize; i++)
                {
                    var c = colours[i];
                    if (texFormat == SpriteRenderMode.Additive)
                    {
                        var a = (int)((c.R + c.G + c.B) / 3f);
                        c = Color.FromArgb(a, c);
                    }
                    else if (texFormat == SpriteRenderMode.IndexAlpha && i < 255)
                    {
                        var a = (int)((c.R + c.G + c.B) / 3f);
                        c = Color.FromArgb(a, last);
                    }
                    pal.Entries[i] = c;
                }
                if (texFormat == SpriteRenderMode.AlphaTest)
                {
                    pal.Entries[255] = Color.FromArgb(0, 0, 0, 0);
                }
                bitmap.Palette = pal;

                // Set the pixel data
                var data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
                Marshal.Copy(pixels, 0, data.Scan0, data.Width * data.Height);
                bitmap.UnlockBits(data);

                return bitmap;
            }
        }

        public async Task<Bitmap> GetImage(string item, int maxWidth, int maxHeight)
        {
            var file = _file.TraversePath(item);
            if (file == null || !file.Exists) return null;

            return await Task.Factory.StartNew(() =>
            {
                using (var s = file.Open())
                {
                    return Parse(s);
                }
            });
        }
        
        public void Dispose()
        {
            //
        }
    }
}