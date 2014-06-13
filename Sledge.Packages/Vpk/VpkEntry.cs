using System.IO;

namespace Sledge.Packages.Vpk
{
    public class VpkEntry : IPackageEntry
    {
        internal const ushort EntryTerminator = 0xffff;

        public VpkDirectory Directory { get; private set; }

        public string Path { get; private set; }
        public uint Crc32 { get; private set; }
        public byte[] PreloadData { get; private set; }
        public ushort ArchiveIndex { get; private set; }
        public uint EntryOffset { get; private set; }
        public uint EntryLength { get; private set; }

        public long Length
        {
            get { return (int) (EntryLength + PreloadData.Length); }
        }

        public string Name { get { return GetName(); } }
        public string FullName { get { return Path; } }
        public string ParentPath { get { return GetParent(); } }

        public VpkEntry(VpkDirectory directory, string path, uint crc32, byte[] preloadBytes, ushort archiveIndex, uint entryOffset, uint entryLength)
        {
            Directory = directory;
            Path = path;
            Crc32 = crc32;
            PreloadData = preloadBytes;
            ArchiveIndex = archiveIndex;
            EntryOffset = entryOffset;
            EntryLength = entryLength;
        }

        public Stream Open()
        {
            if (EntryLength == 0) return new MemoryStream(PreloadData);
            return Directory.OpenStream(this);
        }

        private string GetName()
        {
            var idx = Path.LastIndexOf('/');
            if (idx < 0) return Path;
            return Path.Substring(idx + 1);
        }

        private string GetParent()
        {
            var idx = Path.LastIndexOf('/');
            if (idx < 0) return "";
            return Path.Substring(0, idx);
        }
    };
}