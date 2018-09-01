using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Sledge.Common.Extensions;
using Sledge.FileSystem;
using Sledge.Packages;

namespace Sledge.Providers.Texture.Wad.Format
{
    // http://yuraj.ucoz.com/half-life-formats.pdf
    // https://developer.valvesoftware.com/wiki/WAD
    public class WadPackage
    {
        private const string Signature = "WAD3";

        public IFile File { get; }

        private readonly uint _numTextures;
        private readonly uint _lumpOffset;
        private readonly Dictionary<string, WadEntry> _entries;

        public WadPackage(IFile file)
        {
            File = file;
            _entries = new Dictionary<string, WadEntry>();

            // Read the data from the wad
            using (var br = new BinaryReader(file.Open()))
            {
                var sig = br.ReadFixedLengthString(Encoding.ASCII, 4);
                if (sig != Signature) throw new PackageException("Unknown package signature: Expected '" + Signature + "', got '" + sig + "'.");
                
                _numTextures = br.ReadUInt32();
                _lumpOffset = br.ReadUInt32();

                // Read all the entries from the wad
                ReadTextureEntries(br);
                SetAdditionalEntryData(br);
                RemoveInvalidEntries();
            }
        }

        #region Entry names

        public static IEnumerable<string> GetEntryNames(IFile file)
        {
            using (var s = file.Open())
            {
                return GetEntryNames(s);
            }
        }

        public static IEnumerable<string> GetEntryNames(Stream stream)
        {
            using (var br = new BinaryReader(stream, Encoding.ASCII, true))
            {
                var sig = br.ReadFixedLengthString(Encoding.ASCII, 4);
                if (sig != Signature) throw new PackageException("Unknown package signature: Expected '" + Signature + "', got '" + sig + "'.");

                var numTextures = br.ReadUInt32();
                var lumpOffset = br.ReadUInt32();

                var validTypes = Enum.GetValues(typeof(WadEntryType)).OfType<WadEntryType>().Select(x => (byte)x).ToArray();
                br.BaseStream.Position = lumpOffset;

                for (var i = 0; i < numTextures; i++)
                {
                    br.BaseStream.Seek(12, SeekOrigin.Current);
                    var type = br.ReadByte();

                    if (!validTypes.Contains(type))
                    {
                        // Skip unsupported types
                        br.BaseStream.Seek(19, SeekOrigin.Current);
                    }
                    else
                    {
                        br.BaseStream.Seek(3, SeekOrigin.Current);
                        yield return br.ReadFixedLengthString(Encoding.ASCII, 16).ToLowerInvariant();
                    }
                }
            }
        }
        
        #endregion

        #region Loading

        private void ReadTextureEntries(BinaryReader br)
        {
            var validTypes = Enum.GetValues(typeof (WadEntryType)).OfType<WadEntryType>().Select(x => (byte) x).ToArray();
            br.BaseStream.Position = _lumpOffset;
            for (var i = 0; i < _numTextures; i++)
            {
                var offset = br.ReadUInt32();
                var compressedLength = br.ReadUInt32();
                var fullLength = br.ReadUInt32();
                var type =  br.ReadByte();
                var compressionType = br.ReadByte();
                br.ReadBytes(2); // struct padding
                var name = br.ReadFixedLengthString(Encoding.ASCII, 16).ToLowerInvariant();

                if (!validTypes.Contains(type)) continue; // Skip unsupported types
                if (_entries.ContainsKey(name)) continue; // Don't add duplicates

                _entries[name] = new WadEntry(name, (WadEntryType) type, offset, compressionType, compressedLength, fullLength);
            }
        }

        private void SetAdditionalEntryData(BinaryReader br)
        {
            foreach (var wadEntry in _entries.Values.OrderBy(x => x.Offset))
            {
                br.BaseStream.Position = wadEntry.Offset;
                SetEntryData(wadEntry, br);
            }
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

        private void RemoveInvalidEntries()
        {
            var inv = _entries.Where(e => (e.Value.PaletteDataOffset + e.Value.PaletteSize * 3) - e.Value.Offset > e.Value.Length).ToList();
            foreach (var e in inv) _entries.Remove(e.Key);
        }

        #endregion

        public IEnumerable<WadEntry> GetEntries() => _entries.Values;
        public bool HasEntry(string name) => _entries.ContainsKey(name);
        public WadEntry GetEntry(string name) => _entries.ContainsKey(name) ? _entries[name] : null;
    }
}
