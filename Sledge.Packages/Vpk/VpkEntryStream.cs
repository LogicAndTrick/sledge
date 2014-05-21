using System;
using System.IO;

namespace Sledge.Packages.Vpk
{
    internal class VpkEntryStream : Stream
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
            get { return _entry.EntryLength + _entry.PreloadData.Length; }
        }

        private readonly VpkEntry _entry;
        private readonly Stream _stream;
        private readonly long _streamStart;

        public VpkEntryStream(VpkEntry entry, VpkDirectory directory)
        {
            _entry = entry;
            _stream = directory.OpenChunk(_entry);
            _streamStart = _stream.Position;
        }

        public VpkEntryStream(VpkEntry entry, Stream stream)
        {
            _entry = entry;
            _stream = stream;
            _streamStart = stream.Position;
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
            var ret = 0;
            if (Position < _entry.PreloadData.Length)
            {
                // Get the preload data
                var remainingPreload = _entry.PreloadData.Length - Position;
                var copyLength = Math.Min(count, remainingPreload);
                Array.Copy(_entry.PreloadData, Position, buffer, offset, copyLength);
                count -= (int) copyLength;
                offset += (int) copyLength;
                ret += (int) copyLength;
                Position += ret;
            }
            if (Position >= _entry.PreloadData.Length && _entry.EntryLength > 0)
            {
                var currentEntry = Position - _entry.PreloadData.Length;
                var remainingEntry = _entry.EntryLength - currentEntry;
                var copyLength = (int) Math.Min(count, remainingEntry);
                if (copyLength > 0)
                {
                    lock (_stream)
                    {
                        _stream.Position = _streamStart + currentEntry;
                        var read = _stream.Read(buffer, offset, copyLength);
                        ret += read;
                        Position += read;
                    }
                }
            }
            return ret;
        }

        protected override void Dispose(bool disposing)
        {
            _stream.Dispose();
            base.Dispose(disposing);
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