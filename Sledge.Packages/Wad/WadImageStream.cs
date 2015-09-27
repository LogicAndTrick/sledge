using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace Sledge.Packages.Wad
{
    internal class WadImageStream : Stream
    {
        public override long Position { get; set; }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override long Length
        {
            get { return _length; }
        }

        private readonly WadEntry _entry;

        private uint _length;
        private byte[] _data;

        private BinaryReader OpenQuakePalette()
        {
            if (_entry.Type != WadEntryType.QuakeTexture)
                return null;

            string palettePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/Palettes/quake.lmp";
            return new BinaryReader(new FileInfo(palettePath).OpenRead());
        }

        public WadImageStream(WadEntry entry, WadPackage package)
        {
            _entry = entry;
            using (var br = new BinaryReader(package.OpenFile(package.PackageFile)))
            {
                using (var paletteBr = OpenQuakePalette())
                {
                    br.BaseStream.Position = entry.Offset;
                    PrepareData(br, paletteBr);
                }
            }
        }

        public WadImageStream(WadEntry entry, Stream stream)
        {
            _entry = entry;
            using (var br = new BinaryReader(stream))
            {
                using (var paletteBr = OpenQuakePalette())
                {
                    PrepareData(br, paletteBr);
                }
            }
        }

        private void PrepareData(BinaryReader br, BinaryReader paletteBr)
        {
            var startIndex = br.BaseStream.Position;
            const uint headerSize = 14;
            const uint infoSize = 40;
            _length = headerSize + infoSize + _entry.PaletteSize * 4 + _entry.Width * _entry.Height;
            _data = new byte[_length];
            using (var bw = new BinaryWriter(new MemoryStream(_data, true)))
            {
                // BITMAPFILEHEADER
                bw.WriteFixedLengthString(Encoding.ASCII, 2, "BM"); // Type
                bw.Write(_length); // Size
                bw.Write((short)0); // Reserved 1
                bw.Write((short)0); // Reserved 2
                bw.Write((int) (headerSize + infoSize + _entry.PaletteSize * 4)); // Offset to pixel array

                // BITMAPINFOHEADER
                bw.Write(infoSize); // Header size
                bw.Write(_entry.Width);
                bw.Write(_entry.Height);
                bw.Write((short) 1); // Number of colour planes
                bw.Write((short) 8); // Colour depth
                bw.Write(0); // Compression method
                bw.Write(_entry.Width * _entry.Height); // Image data size
                bw.Write(0); // Horizontal resolution
                bw.Write(0); // Vertical resolution
                bw.Write(_entry.PaletteSize); // Colours used
                bw.Write(_entry.PaletteSize); // "Important" colours used

                byte[] paletteData;
                if (_entry.Type == WadEntryType.QuakeTexture)
                {
                    paletteData = paletteBr.ReadBytes((int)(_entry.PaletteSize * 3));
                }
                else
                {
                    br.BaseStream.Position = startIndex + (_entry.PaletteDataOffset - _entry.Offset);
                    paletteData = br.ReadBytes((int)(_entry.PaletteSize * 3));
                }

                for (var i = 0; i < _entry.PaletteSize; i++)
                {
                    // Wad palettes are RGB, bitmap is BGRX
                    bw.Write(paletteData[i * 3 + 2]);
                    bw.Write(paletteData[i * 3 + 1]);
                    bw.Write(paletteData[i * 3 + 0]);
                    bw.Write((byte)0);
                }

                br.BaseStream.Position = startIndex + (_entry.TextureDataOffset - _entry.Offset);
                var imageData = br.ReadBytes((int)(_entry.Width * _entry.Height));
                for (var y = (int)_entry.Height - 1; y >= 0; y--)
                {
                    // The Y axis is reversed
                    bw.Write(imageData, (int)_entry.Width * y, (int) _entry.Width);
                }
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    Position = offset;
                    break;
                case SeekOrigin.Current:
                    Position += offset;
                    break;
                case SeekOrigin.End:
                    Position = Length + offset;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("origin");
            }
            return Position;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var pos = Math.Min(Position, _length);
            var ret = (int) Math.Min(_length - pos, count);
            Array.Copy(_data, pos, buffer, offset, ret);
            Position += ret;
            return ret;
        }

        // Write: not supported
        public override void Flush()
        {
            //
        }

        public override void SetLength(long value)
        {
            //
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            //
        }
    }
}