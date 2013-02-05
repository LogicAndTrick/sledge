using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Sledge.Graphics.Helpers;


namespace Sledge.Providers.Texture
{
    public class SprProvider : TextureProvider
    {
        protected override bool IsValidForPackageFile(string package)
        {
            return Directory.Exists(package);
        }

        private static Bitmap Parse(FileInfo file)
        {
            // Sprite file spec taken from the spritegen source in the Half-Life SDK.
            using (var br = new BinaryReader(file.OpenRead()))
            {
                var idst = br.ReadFixedLengthString(Encoding.ASCII, 4);
                if (idst != "IDSP") return null;
                var version = br.ReadInt32();
                if (version != 2) return null;
                var type = br.ReadInt32();
                var texFormat = br.ReadInt32();
                var boundingRadius = br.ReadSingle();
                var width = br.ReadInt32();
                var height = br.ReadInt32();
                var numframes = br.ReadInt32();
                var beamlength = br.ReadSingle();
                var synctype = br.ReadInt32();
                var paletteSize = br.ReadInt16();
                var palette = br.ReadBytes(paletteSize * 3);
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
                var bitmap = new Bitmap(framewidth, frameheight);
                for (var y = 0; y < frameheight; y++)
                {
                    for (var x = 0; x < framewidth; x++)
                    {
                        var idx = pixels[framewidth * y + x] * 3;
                        var a = 255;
                        var r = palette[idx + 0];
                        var g = palette[idx + 1];
                        var b = palette[idx + 2];
                        if (b == 255 && r == 0 && g == 0) a = b = 0; // blue pixels are transparent
                        var col = Color.FromArgb(a, r, g, b);
                        bitmap.SetPixel(x, y, col);
                    }
                }
                return bitmap;
            }
        }

        protected override void LoadTexture(TexturePackage package, string name)
        {
            var file = new FileInfo(Path.Combine(package.PackageFile, name));
            if (file.Exists)
            {
                TextureHelper.Create("sprites/" + name, Parse(file));
            }
        }

        protected override void LoadTextures(TexturePackage package, IEnumerable<string> names)
        {
            foreach (var name in names)
            {
                LoadTexture(package, name);
            }
        }

        private TextureItem CreateTextureItem(TexturePackage package, string name)
        {
            var bmp = Parse(new FileInfo(name));
            return new TextureItem(package, name.Substring(package.PackageFile.Length).TrimStart(Path.DirectorySeparatorChar), bmp.Width, bmp.Height);
        }

        protected override IEnumerable<TextureItem> GetAllTextureItems(TexturePackage package)
        {
            return Directory.GetFiles(package.PackageFile, "*.spr", SearchOption.AllDirectories)
                .Select(x => CreateTextureItem(package, x));
        }

        protected override TextureStreamSource GetStreamSource(IEnumerable<TexturePackage> packages)
        {
            throw new NotImplementedException();
        }
    }
}
