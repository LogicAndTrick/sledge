using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Sledge.FileSystem;
using Sledge.Graphics.Helpers;

namespace Sledge.Providers.Texture
{
    public class SprProvider : TextureProvider
    {
        private enum SpriteRenderMode
        {
            Normal = 0,     // No transparency
            Additive = 1,   // R/G/B = R/G/B, A = (R+G+B)/3
            IndexAlpha = 2, // R/G/B = Palette index 255, A = (R+G+B)/3
            AlphaTest = 3   // R/G/B = R/G/B, Palette index 255 = transparent
        }

        public override bool IsValidForPackageFile(IFile package)
        {
            return package.IsContainer && package.Exists;
        }

        private static Bitmap Parse(IFile file)
        {
            // Sprite file spec taken from the spritegen source in the Half-Life SDK.
            using (var br = new BinaryReader(file.Open()))
            {
                var idst = br.ReadFixedLengthString(Encoding.ASCII, 4);
                if (idst != "IDSP") return null;
                var version = br.ReadInt32();
                if (version != 2) return null;
                var type = (SpriteOrientation) br.ReadInt32();
                var texFormat = (SpriteRenderMode) br.ReadInt32();
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
                        var a = (int) ((c.R + c.G + c.B) / 255f);
                        c = Color.FromArgb(a, c);
                    }
                    else if (texFormat == SpriteRenderMode.IndexAlpha && i < 255)
                    {
                        var a = (int) ((c.R + c.G + c.B) / 255f);
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

        public override void LoadTexture(TextureItem item)
        {
            LoadTextures(new[] {item});
        }

        public override void LoadTextures(IEnumerable<TextureItem> items)
        {
            foreach (var item in items)
            {
                var file = item.Package.PackageFile.GetFile(item.Name);
                if (file != null && file.Exists)
                {
                    TextureHelper.Create("sprites/" + item.Name.ToLowerInvariant(), Parse(file), true);
                }
            }
        }

        private TextureItem CreateTextureItem(TexturePackage package, IFile file)
        {
            var bmp = Parse(file);
            return new TextureItem(package, file.Name, bmp.Width, bmp.Height);
        }

        public override TexturePackage CreatePackage(IFile package)
        {
            var tp = new TexturePackage(package, this);
            var items = package.GetFiles(".*\\.spr$", true)
                .Select(x => CreateTextureItem(tp, x));
            foreach (var ti in items)
            {
                tp.AddTexture(ti);
            }
            return tp;
        }

        public override ITextureStreamSource GetStreamSource(IEnumerable<TexturePackage> packages)
        {
            throw new NotImplementedException();
        }
    }
}
