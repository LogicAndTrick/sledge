using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.IO;
using Sledge.Common;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;
using Sledge.Packages;
using Sledge.Packages.Zip;
using Sledge.Rendering.Materials;

namespace Sledge.Providers.Texture
{
    public class Pk3Provider : TextureProvider
    {
        public static bool ReplaceTransparentPixels = true;

        private static Bitmap PostProcessBitmap(string packageName, string name, Bitmap bmp, out bool hasTransparency)
        {
            hasTransparency = false;
            return bmp;
        }

        private const char NullCharacter = (char)0;

        private bool LoadFromCache(TexturePackage package)
        {
            if (CachePath == null || !Directory.Exists(CachePath)) return false;

            var fi = new FileInfo(package.PackageRoot);
            var cacheFile = Path.Combine(CachePath, fi.Name + "_" + (fi.LastWriteTime.Ticks));
            if (!File.Exists(cacheFile)) return false;

            var lines = File.ReadAllLines(cacheFile);
            if (lines.Length < 3) return false;
            if (lines[0] != fi.FullName) return false;
            if (lines[1] != fi.LastWriteTime.ToFileTime().ToString(CultureInfo.InvariantCulture)) return false;
            if (lines[2] != fi.Length.ToString(CultureInfo.InvariantCulture)) return false;

            try
            {
                var items = new List<TextureItem>();
                foreach (var line in lines.Skip(3))
                {
                    var spl = line.Split(NullCharacter);
                    var reference = spl[0];
                    var name = reference.Substring("textures/".Length);
                    name = name.Substring(0, name.Length - 4);
                    items.Add(new TextureItem(package, name, GetFlags(spl[0]), int.Parse(spl[1], CultureInfo.InvariantCulture), int.Parse(spl[2], CultureInfo.InvariantCulture)) { Reference = reference });
                }
                items.ForEach(package.AddTexture);
            }
            catch
            {
                // Cache file is no good...
                return false;
            }
            return true;
        }

        // TODO: Figure out how id Tech 3 handles this stuff.
        private TextureFlags GetFlags(string name)
        {
            var flags = TextureFlags.None;
            //var tp = ReplaceTransparentPixels && name.StartsWith("{");
            //if (tp) flags |= TextureFlags.Transparent;
            return flags;
        }

        private void SaveToCache(TexturePackage package)
        {
            if (CachePath == null || !Directory.Exists(CachePath)) return;
            var fi = new FileInfo(package.PackageRoot);
            var cacheFile = Path.Combine(CachePath, fi.Name + "_" + (fi.LastWriteTime.Ticks));
            var lines = new List<string>();
            lines.Add(fi.FullName);
            lines.Add(fi.LastWriteTime.ToFileTime().ToString(CultureInfo.InvariantCulture));
            lines.Add(fi.Length.ToString(CultureInfo.InvariantCulture));
            foreach (var ti in package.Items.Values)
            {
                lines.Add(ti.Reference + NullCharacter + ti.Width.ToString(CultureInfo.InvariantCulture) + NullCharacter + ti.Height.ToString(CultureInfo.InvariantCulture));
            }
            File.WriteAllLines(cacheFile, lines);
        }

        private readonly Dictionary<TexturePackage, Pk3Stream> _roots = new Dictionary<TexturePackage, Pk3Stream>();

        private TexturePackage CreatePackage(string package)
        {
            try
            {
                var fi = new FileInfo(package);
                if (!fi.Exists) return null;

                var tp = new TexturePackage(package, Path.GetFileNameWithoutExtension(package), this);
                if (LoadFromCache(tp)) return tp;

                var pack = _roots.Values.FirstOrDefault(x => x.Package.PackageFile.FullName == fi.FullName);
                if (pack == null) _roots.Add(tp, pack = new Pk3Stream(new ZipPackage(fi)));
                
                foreach (var file in pack.Package.SearchFiles("textures", @"\.(jpg|png|tga)$", true))
                {
                    var size = GetImageSize(pack.StreamSource, file);
                    var fname = file.Substring("textures/".Length);
                    fname = fname.Substring(0, fname.Length - 4);
                    var ti = new TextureItem(tp, fname, GetFlags(fname), size.Width, size.Height) {Reference = file};
                    tp.AddTexture(ti);
                }
                SaveToCache(tp);
                return tp;
            }
            catch
            {
                return null;
            }
        }

        private Size GetImageSize(IPackageStreamSource source, string file)
        {
            var len = 0;
            try
            {
                using (var str = source.OpenFile(file))
                {
                    len = (int) str.Length;
                    if (file.EndsWith(".tga"))
                    {
                        return TgaFile.GetTgaFileSize(str);
                    }
                    else
                    {
                        using (var bmp = Image.FromStream(str))
                        {
                            return new Size(bmp.Width, bmp.Height);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                int i = 1;
                throw;
            }
        }

        internal static Bitmap OpenImage(IPackageStreamSource source, string file)
        {
            var open = source.OpenFile(file.ToLowerInvariant());
            if (open == null) return null;

            if (file.EndsWith(".tga"))
            {
                return TgaFile.LoadTgaFile(open);
            }
            else
            {
                return new Bitmap(open);
            }
        }

        public override IEnumerable<TexturePackage> CreatePackages(IEnumerable<string> sourceRoots, IEnumerable<string> additionalPackages, IEnumerable<string> blacklist, IEnumerable<string> whitelist)
        {
            var blist = blacklist.Select(x => x.EndsWith(".pk3") ? x.Substring(0, x.Length - 4) : x).Where(x => !String.IsNullOrWhiteSpace(x)).ToList();
            var wlist = whitelist.Select(x => x.EndsWith(".pk3") ? x.Substring(0, x.Length - 4) : x).Where(x => !String.IsNullOrWhiteSpace(x)).ToList();
            var pk3s = sourceRoots.Union(additionalPackages)
                .Where(Directory.Exists)
                .SelectMany(x => Directory.GetFiles(x, "*.pk3", SearchOption.TopDirectoryOnly))
                .Union(additionalPackages.Where(x => x.EndsWith(".pk3") && File.Exists(x)))
                .GroupBy(Path.GetFileNameWithoutExtension)
                .Select(x => x.First())
                .Where(x => !blist.Any(b => String.Equals(Path.GetFileNameWithoutExtension(x) ?? x, b, StringComparison.InvariantCultureIgnoreCase)));
            if (wlist.Any())
            {
                pk3s = pk3s.Where(x => wlist.Contains(Path.GetFileNameWithoutExtension(x) ?? x, StringComparer.InvariantCultureIgnoreCase));
            }
            return pk3s.AsParallel().Select(CreatePackage).Where(x => x != null);
        }

        public override void DeletePackages(IEnumerable<TexturePackage> packages)
        {
            var packs = packages.ToList();
            var roots = _roots.Where(x => packs.Contains(x.Key)).Select(x => x.Value).ToList();
            foreach (var tp in packs)
            {
                _roots.Remove(tp);
            }
            foreach (var root in roots.Where(x => !_roots.ContainsValue(x)))
            {
                root.Dispose();
            }
        }

        private static bool PackageHasTexture(IPackageStreamSource package, string name)
	    {
	        return package.HasFile((name ?? "").ToLowerInvariant());
	    }

	    public override ITextureStreamSource GetStreamSource(int maxWidth, int maxHeight, IEnumerable<TexturePackage> packages)
        {
            var packs = packages.Select(x =>
            {
                if (!_roots.ContainsKey(x))
                {
                    var wp = new Pk3Stream(new ZipPackage(new FileInfo(x.PackageRoot)));
                    _roots.Add(x, wp);
                }
                return _roots[x];

            }).ToList();
            var streams = packs.Select(x => x.StreamSource).ToList();
            return new Pk3StreamSource(streams);
        }

        private class Pk3StreamSource : ITextureStreamSource
        {
            private readonly List<IPackageStreamSource> _streams;

            public Pk3StreamSource(IEnumerable<IPackageStreamSource> streams)
            {
                _streams = streams.ToList();
            }

            public bool HasImage(TextureItem item)
            {
                return _streams.Any(x => PackageHasTexture(x, item.Reference));
            }

            public Bitmap GetImage(TextureItem item)
            {
                var stream = _streams.First(x => PackageHasTexture(x, item.Reference));
                var bmp = Pk3Provider.OpenImage(stream, item.Reference);
                bool hasTransparency;
                return PostProcessBitmap(item.Package.PackageRelativePath, item.Name.ToLowerInvariant(), bmp, out hasTransparency);
            }

            public void Dispose()
            {

            }
        }

        private class Pk3Stream : IDisposable
        {
            public ZipPackage Package { get; set; }
            public IPackageStreamSource StreamSource { get; private set; }

            public Pk3Stream(ZipPackage package)
            {
                Package = package;
                StreamSource = package.GetStreamSource();
            }

            public void Dispose()
            {
                StreamSource.Dispose();
                Package.Dispose();
            }
        }
    }

    static class TgaFile
    {
        public static Size GetTgaFileSize(Stream stream)
        {
            using (var br = new BinaryReader(stream, Encoding.UTF8))
            {
                const int offset = 1 + 1 + 1 + 2 + 2 + 1 + 2 + 2;
                br.BaseStream.Seek(offset, SeekOrigin.Current);

                var width = br.ReadUInt16();
                var height = br.ReadUInt16();

                return new Size(width, height);
            }
        }

        public static Bitmap LoadTgaFile(Stream stream)
        {
            using (var br = new BinaryReader(stream, Encoding.UTF8))
            {
                br.BaseStream.Seek(-26, SeekOrigin.End);
                var extensionOffset = br.ReadUInt32();
                var developerOffset = br.ReadUInt32();
                var sig = new string(br.ReadChars(16));
                var isVersion2Format = sig == "TRUEVISION-XFILE";

                byte footerAttributes = 0;
                if (isVersion2Format)
                {
                    var skip = 2 + 41 + 324 + 12 + 41 + 6 + 41 + 3 + 4 + 4 + 4 + 4 + 4 + 4;
                    br.BaseStream.Seek(extensionOffset + skip, SeekOrigin.Begin);
                    footerAttributes = br.ReadByte();
                }

                br.BaseStream.Seek(0, SeekOrigin.Begin);

                var idLength = br.ReadByte();
                var colourMapType = br.ReadByte();
                var imageType = br.ReadByte();
                var firstColourMapEntry = br.ReadUInt16();
                var colourMapLength = br.ReadUInt16();
                var colourMapEntrySize = br.ReadByte();
                var colourMapBytesPerPixel = (int)Math.Ceiling(colourMapEntrySize / 8f);
                var xOrigin = br.ReadUInt16();
                var yOrigin = br.ReadUInt16();
                var width = br.ReadUInt16();
                var height = br.ReadUInt16();
                var bitsPerPixel = br.ReadByte();
                var bytesPerPixel = (int)Math.Ceiling(bitsPerPixel / 8f);
                var imageDescriptor = br.ReadByte();
                var imageDescriptorAlpha = (isVersion2Format ? footerAttributes : imageDescriptor) & 0x07;
                var directionTop = (imageDescriptor & 0x20) == 0x20;
                var directionRight = (imageDescriptor & 0x10) == 0x10;

                var hasMap = colourMapType == 0x01;
                var compressed = (imageType & 0x08) == 0x08;
                var typeColourMapped = (imageType & 0x07) == 0x01;
                var typeTrueColour = (imageType & 0x07) == 0x02;
                var typeBlackAndWhite = (imageType & 0x07) == 0x03;

                br.BaseStream.Seek(idLength, SeekOrigin.Current);

                var colourMap = br.ReadBytes(colourMapLength * colourMapBytesPerPixel);

                byte[] imageData;

                if (compressed)
                {
                    imageData = new byte[width * height * bytesPerPixel];
                    int count = 0;
                    while (count < width * height)
                    {
                        var rleHeader = br.ReadByte();
                        var rlEncoded = (rleHeader & 0x80) == 0x80;
                        var length = (rleHeader & 0x7F) + 1;
                        if (rlEncoded)
                        {
                            var bytes = br.ReadBytes(bytesPerPixel);
                            for (int i = 0; i < length && count + i < width * height; i++)
                            {
                                Array.Copy(bytes, 0, imageData, (count + i) * bytesPerPixel, bytesPerPixel);
                            }
                        }
                        else
                        {
                            var bytes = br.ReadBytes(length * bytesPerPixel);
                            Array.Copy(bytes, 0, imageData, count * bytesPerPixel, bytes.Length);
                        }
                        count += length;
                    }
                }
                else
                {
                    imageData = br.ReadBytes(width * height * bytesPerPixel);
                }

                if (hasMap)
                {
                    var newImageData = new byte[width * height * colourMapBytesPerPixel];
                    for (int i = 0; i < width * height; i++)
                    {
                        var idx = imageData[i];
                        Array.Copy(colourMap, idx * colourMapBytesPerPixel, newImageData, i * colourMapBytesPerPixel, colourMapBytesPerPixel);
                    }
                    bitsPerPixel = colourMapEntrySize;
                    bytesPerPixel = colourMapBytesPerPixel;
                    imageData = newImageData;
                }

                byte[] buffer = new byte[width * height * 4];
                for (int i = 0; i < width * height; i++)
                {
                    var bufferPos = i * 4;
                    var imagePos = i * bytesPerPixel;
                    if (bitsPerPixel == 32)
                    {
                        buffer[bufferPos + 0] = imageData[imagePos + 0];
                        buffer[bufferPos + 1] = imageData[imagePos + 1];
                        buffer[bufferPos + 2] = imageData[imagePos + 2];
                        buffer[bufferPos + 3] = imageDescriptorAlpha != 3 ? (byte)255 : imageData[imagePos + 3];
                    }
                    else if (bitsPerPixel == 24)
                    {
                        buffer[bufferPos + 0] = imageData[imagePos + 0];
                        buffer[bufferPos + 1] = imageData[imagePos + 1];
                        buffer[bufferPos + 2] = imageData[imagePos + 2];
                        buffer[bufferPos + 3] = 255;
                    }
                    else if (bitsPerPixel == 16 || bitsPerPixel == 15)
                    {
                        // 0x ARRRRRGG GGGBBBBB
                        // It seems the best way to convert 5 bytes to 8 is to copy the upper bits to the lower like this:
                        // 0x000ABCDE -> 0xABCDEABC
                        var bit1 = (uint)imageData[imagePos + 1];
                        var bit2 = (uint)imageData[imagePos + 0];
                        var pixel = bit1 << 8 | bit2;
                        var redValue = (pixel & 0x7C00) >> 10;
                        var greenValue = (pixel & 0x03E0) >> 5;
                        var blueValue = (pixel & 0x001F);
                        buffer[bufferPos + 0] = (byte)(blueValue << 3 | blueValue >> 2);
                        buffer[bufferPos + 1] = (byte)(greenValue << 3 | greenValue >> 2);
                        buffer[bufferPos + 2] = (byte)(redValue << 3 | redValue >> 2);
                        buffer[bufferPos + 3] = imageDescriptorAlpha != 3 || bitsPerPixel == 15 ? (byte)255 : (byte)((bit1 & 0x80) == 1 ? 255 : 0);
                    }
                    else if (bitsPerPixel == 8)
                    {
                        buffer[bufferPos + 0] = imageData[imagePos];
                        buffer[bufferPos + 1] = imageData[imagePos];
                        buffer[bufferPos + 2] = imageData[imagePos];
                        buffer[bufferPos + 3] = 255;
                    }
                }

                // flip based on image orientation
                if (!directionTop)
                {
                    var newBuffer = new byte[width * height * 4];
                    for (int i = 0; i < height; i++)
                    {
                        Array.Copy(buffer, i * width * 4, newBuffer, (height - i - 1) * width * 4, width * 4);
                    }
                    buffer = newBuffer;
                }

                if (directionRight)
                {
                    var newBuffer = new byte[width * height * 4];
                    for (int i = 0; i < width; i++)
                    {
                        Array.Copy(buffer, i * height * 4, newBuffer, (width - i - 1) * height * 4, height * 4);
                    }
                    buffer = newBuffer;
                }

                var bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
                var bits = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                Marshal.Copy(buffer, 0, bits.Scan0, buffer.Length);
                bmp.UnlockBits(bits);

                return bmp;
            }
        }
    }
}
