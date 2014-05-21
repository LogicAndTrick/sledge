using System;
using System.IO;

namespace Sledge.Packages
{
    internal class SubStream : Stream
    {
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

        public override long Position { get; set; }
        public override long Length { get { return _length; } }

        public bool CloseParentOnDispose { get; set; }

        private Stream _stream;
        private long _offset;
        private long _length;

        public SubStream(Stream stream, long offset, long length)
        {
            _stream = stream;
            _offset = offset;
            _length = length;
            CloseParentOnDispose = false;
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
                    Position = _length + offset;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("origin");
            }
            return Position;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            lock (_stream)
            {
                var pos = _stream.Position;
                _stream.Position = _offset + Position;
                count = (int) Math.Min(count, _length - Position);
                count = _stream.Read(buffer, offset, count);
                Position += count;
                _stream.Position = pos;
                return count;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && CloseParentOnDispose) _stream.Dispose();
            base.Dispose(disposing);
        }

        //
        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
    }
}