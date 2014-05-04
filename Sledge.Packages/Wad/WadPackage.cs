using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Sledge.Packages.Wad
{
    // http://yuraj.ucoz.com/half-life-formats.pdf
    // https://developer.valvesoftware.com/wiki/WAD
    public class WadPackage : IDisposable
    {
        private const string Signature = "WAD3";

        public FileInfo File { get; private set; }
        internal uint NumTextures { get; private set; }
        internal uint LumpOffset { get; private set; }
        internal List<WadEntry> Entries { get; private set; }

        public WadPackage(FileInfo file)
        {
            File = file;
            Entries = new List<WadEntry>();

            // Read the data from the wad
            using (var br = new BinaryReader(OpenFile(file)))
            {
                var sig = br.ReadFixedLengthString(Encoding.ASCII, 4);
                if (sig != Signature) throw new PackageException("Unknown package signature: Expected '" + Signature + "', got '" + sig + "'.");

                NumTextures = br.ReadUInt32();
                LumpOffset = br.ReadUInt32();

                // Read all the entries from the wad
                ReadTextureEntries(br);
                SetAdditionalEntryData(br);
            }
        }
        
        internal FileStream OpenFile(FileInfo file)
        {
            return new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 4096, FileOptions.RandomAccess);
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
                var name = br.ReadFixedLengthString(Encoding.ASCII, 16);

                if (!validTypes.Contains(type)) continue; // Skip unsupported types
                Entries.Add(new WadEntry(name, (WadEntryType) type, offset, compressionType, compressedLength, fullLength));
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

        public IEnumerable<WadEntry> GetEntries()
        {
            return Entries;
        }

        public byte[] ExtractEntry(WadEntry entry)
        {
            using (var sr = new BinaryReader(OpenStream(entry)))
            {
                return sr.ReadBytes((int) entry.CompressedLength);
            }
        }

        public Stream OpenStream(WadEntry entry)
        {
            return new WadImageStream(entry, this);
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
                    br.BaseStream.Position += 16; // Skip name
                    width = br.ReadUInt32();
                    height = br.ReadUInt32();
                    textureDataOffset = br.BaseStream.Position + 16;
                    var num = (int)(width * height);
                    var skipMapData = (num / 4) + (num / 16) + (num / 64);
                    br.BaseStream.Position += 16 + num + skipMapData; // Skip mipmap offsets, texture data, mipmap texture data
                    paletteSize = br.ReadUInt16();
                    paletteDataOffset = br.BaseStream.Position;
                    break;
                case WadEntryType.Font:
                    width = br.ReadUInt32();
                    height = br.ReadUInt32();
                    textureDataOffset = br.BaseStream.Position + 8 + (256 * 4);
                    br.BaseStream.Position += 8 + (256 * 4) + (width * height); // Skip font data, texture data
                    paletteSize = br.ReadUInt16();
                    paletteDataOffset = br.BaseStream.Position;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            e.Width = width;
            e.Height = height;
            e.PaletteSize = paletteSize;
            e.TextureDataOffset = textureDataOffset;
            e.PaletteDataOffset = paletteDataOffset;
        }
    }
}
