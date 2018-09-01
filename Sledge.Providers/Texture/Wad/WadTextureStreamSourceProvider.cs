using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using Sledge.FileSystem;
using Sledge.Providers.Texture.Wad.Format;

namespace Sledge.Providers.Texture.Wad
{
    public class WadStreamSource : ITextureStreamSource
    {
        private readonly WadPackageStreamSource _stream;
        private readonly WadPackage _package;

        public WadStreamSource(IFile file)
        {
            _package = new WadPackage(file);
            _stream = new WadPackageStreamSource(_package);
        }

        public bool HasImage(string item)
        {
            return _stream.HasEntry(item);
        }

        public async Task<Bitmap> GetImage(string item, int maxWidth, int maxHeight)
        {
            var entry = _stream.GetEntry(item);
            if (entry == null) return null;

            return await Task.Factory.StartNew(() =>
            {
                using (var s = _stream.OpenEntry(entry))
                {
                    return PostProcessBitmap(_package.File.Name, entry.Name, new Bitmap(s));
                }
            });
        }

        private static Bitmap PostProcessBitmap(string packageName, string name, Bitmap bmp)
        {
            // Transparent textures are named like: {Name
            if (!name.StartsWith("{")) return bmp;

            var palette = bmp.Palette;

            // Two transparency types: "blue" transparency and "decal" transparency
            // Decal transparency is all greyscale and doesn't contain any of palette #255 colour
            /*
                var blueTransparency = false;
                for (var i = 0; i < palette.Entries.Length - 1; i++)
                {
                    var color = palette.Entries[i];
                    if (color.R != color.B || color.R != color.G || color.B != color.G)
                    {
                        blueTransparency = true;
                        break;
                    }
                }*/

            // Can't be clever and detect the transparency type automatically - Goldsource is too unpredictable
            // decal.wad is hard-coded in the engine, so this is relatively safe, except for custom engines
            var blueTransparency = packageName.IndexOf("decal", StringComparison.CurrentCultureIgnoreCase) < 0;

            if (blueTransparency)
            {
                // We found the last index, therefore it should be transparent
                palette.Entries[palette.Entries.Length - 1] = Color.Transparent;
            }
            else
            {
                // If we didn't find the last index, we have a decal
                var last = palette.Entries[palette.Entries.Length - 1];
                // If the first item is black, we need to flip the transparency calculation (I think...)
                var isBlack = palette.Entries[0].R == 0 && palette.Entries[0].G == 0 && palette.Entries[0].B == 0;
                for (var i = 0; i < palette.Entries.Length - 1; i++)
                {
                    palette.Entries[i] = Color.FromArgb(isBlack ? palette.Entries[i].R : 255 - palette.Entries[i].R, last);
                }
            }
            bmp.Palette = palette;

            // Copy the bitmap to a new one with a proper alpha channel
            var clone = new Bitmap(bmp.Width, bmp.Height, PixelFormat.Format32bppArgb);
            using (var g = System.Drawing.Graphics.FromImage(clone))
            {
                g.DrawImage(bmp, new Rectangle(0, 0, clone.Width, clone.Height));
            }
            bmp.Dispose();
            return clone;
        }

        public void Dispose()
        {
            _stream.Dispose();
        }
    }
}