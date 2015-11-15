using Sledge.DataStructures.GameData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Sledge.Packages.Wad
{
    // http://yuraj.ucoz.com/half-life-formats.pdf
    // https://developer.valvesoftware.com/wiki/WAD
    public class WadPackage : IPackage
    {
        private const string WAD2Signature = "WAD2";
        private const string WAD3Signature = "WAD3";

        public FileInfo PackageFile { get; private set; }
        internal uint NumTextures { get; private set; }
        internal uint LumpOffset { get; private set; }
        internal List<WadEntry> Entries { get; private set; }
        public Palette Palette { get; private set; }

        public WadPackage(FileInfo packageFile, Palette pal)
        {
            PackageFile = packageFile;
            Entries = new List<WadEntry>();
            Palette = pal;

            // Read the data from the wad
            using (var br = new BinaryReader(OpenFile(packageFile)))
            {
                var sig = br.ReadFixedLengthString(Encoding.ASCII, 4);
                if (sig != WAD2Signature && sig != WAD3Signature) throw new PackageException("Unknown package signature: Expected [WAD2,WAD3], got '" + sig + "'.");

                NumTextures = br.ReadUInt32();
                LumpOffset = br.ReadUInt32();

                // Read all the entries from the wad
                ReadTextureEntries(br);
                SetAdditionalEntryData(br);
                RemoveInvalidEntries();
                BuildDirectories();
            }
        }

        private void RemoveInvalidEntries()
        {
            Entries.RemoveAll(e => (e.PaletteDataOffset + e.PaletteSize * 3) - e.Offset > e.Length);
        }

        internal Stream OpenFile(FileInfo file)
        {
            return Stream.Synchronized(new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 4096, FileOptions.RandomAccess));
        }

        private void ReadTextureEntries(BinaryReader br)
        {
            var validTypes = Enum.GetValues(typeof (WadEntryType)).OfType<WadEntryType>().Select(x => (byte) x).ToArray();
            br.BaseStream.Position = LumpOffset;
            for (int i = 0; i < NumTextures; i++)
            {
                var offset = br.ReadUInt32();
                var compressedLength = br.ReadUInt32();
                var fullLength = br.ReadUInt32();
                var type =  br.ReadByte();
                var compressionType = br.ReadByte();
                br.ReadBytes(2); // struct padding
                var name = br.ReadFixedLengthString(Encoding.ASCII, 16).ToLowerInvariant();

                if (!validTypes.Contains(type)) continue; // Skip unsupported types
                if (Entries.Any(x => x.Name == name)) continue; // Don't add duplicates
                Entries.Add(new WadEntry(this, name, (WadEntryType) type, offset, compressionType, compressedLength, fullLength));
            }
        }

        private void SetAdditionalEntryData(BinaryReader br)
        {
            foreach (var wadEntry in Entries)
            {
                br.BaseStream.Position = wadEntry.Offset;
                SetEntryData(wadEntry, br);
            }
        }

        public IEnumerable<IPackageEntry> GetEntries()
        {
            return Entries;
        }

        public IPackageEntry GetEntry(string path)
        {
            return _files.ContainsKey(path) ? _files[path] : null;
        }

        public byte[] ExtractEntry(IPackageEntry entry)
        {
            using (var sr = new BinaryReader(OpenStream(entry)))
            {
                return sr.ReadBytes((int) sr.BaseStream.Length);
            }
        }

        public Stream OpenStream(IPackageEntry entry)
        {
            var pe = entry as WadEntry;
            if (pe == null) throw new ArgumentException("This package is only compatible with WadEntry objects.");
            return new WadImageStream(pe, this);
        }

        public IPackageStreamSource GetStreamSource()
        {
            return new WadPackageStreamSource(this);
        }

        public void Dispose()
        {
            Entries.Clear();
        }

        private void SetEntryData(WadEntry e, BinaryReader br)
        {
            uint width, height, paletteSize;
            long textureDataOffset, paletteDataOffset;
            switch (e.Type)
            {
                case WadEntryType.Image:
                    width = br.ReadUInt32();
                    height = br.ReadUInt32();
                    textureDataOffset = br.BaseStream.Position;
                    br.BaseStream.Position += width * height; // Skip texture data
                    paletteSize = br.ReadUInt16();
                    paletteDataOffset = br.BaseStream.Position;
                    break;
                case WadEntryType.Texture:
                case WadEntryType.QuakeTexture:
                    br.BaseStream.Position += 16; // Skip name
                    width = br.ReadUInt32();
                    height = br.ReadUInt32();
                    textureDataOffset = br.BaseStream.Position + 16;
                    var num = (int)(width * height);
                    var skipMapData = (num / 4) + (num / 16) + (num / 64);
                    br.BaseStream.Position += 16 + num + skipMapData; // Skip mipmap offsets, texture data, mipmap texture data
                    if (e.Type == WadEntryType.QuakeTexture)
                    {
                        paletteSize = 256;
                        paletteDataOffset = 0;
                    }
                    else
                    {
                        paletteSize = br.ReadUInt16();
                        paletteDataOffset = br.BaseStream.Position;
                    }
                    break;
                    /*
                case WadEntryType.Font:
                    width = br.ReadUInt32();
                    height = br.ReadUInt32();
                    textureDataOffset = br.BaseStream.Position + 8 + (256 * 4);
                    br.BaseStream.Position += 8 + (256 * 4) + (width * height); // Skip font data, texture data
                    paletteSize = br.ReadUInt16();
                    paletteDataOffset = br.BaseStream.Position;
                    break;*/
                default:
                    throw new ArgumentOutOfRangeException();
            }
            e.Width = width;
            e.Height = height;
            e.PaletteSize = paletteSize;
            e.TextureDataOffset = textureDataOffset;
            e.PaletteDataOffset = paletteDataOffset;
        }

        private Dictionary<string, WadEntry> _files;

        private void BuildDirectories()
        {
            _files = GetEntries().OfType<WadEntry>().ToDictionary(x => x.Name, x => x);
        }

        public bool HasDirectory(string path)
        {
            return false;
        }

        public bool HasFile(string path)
        {
            path = path.ToLowerInvariant();
            return _files.ContainsKey(path);
        }

        public IEnumerable<string> GetDirectories()
        {
            return _files.Keys;
        }

        public IEnumerable<string> GetFiles()
        {
            return _files.Values.Select(x => x.Name);
        }

        public IEnumerable<string> GetDirectories(string path)
        {
            return new string[0];
        }

        public IEnumerable<string> GetFiles(string path)
        {
            if (path != "") return new string[0];
            return _files.Keys;
        }

        public IEnumerable<string> SearchDirectories(string path, string regex, bool recursive)
        {
            return new string[0];
        }

        public IEnumerable<string> SearchFiles(string path, string regex, bool recursive)
        {
            var files = GetFiles(path);
            return files.Where(x => Regex.IsMatch(x, regex, RegexOptions.IgnoreCase));
        }

        public Stream OpenFile(string path)
        {
            var entry = GetEntry(path);
            if (entry == null) throw new FileNotFoundException();
            return OpenStream(entry);
        }
    }
}
