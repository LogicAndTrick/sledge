namespace Sledge.Packages.Vpk
{
    public class VpkEntry
    {
        internal const ushort EntryTerminator = 0xffff;

        public string Path { get; private set; }
        public uint Crc32 { get; private set; }
        public byte[] PreloadData { get; private set; }
        public ushort ArchiveIndex { get; private set; }
        public uint EntryOffset { get; private set; }
        public uint EntryLength { get; private set; }

        public int TotalLength
        {
            get { return (int) (EntryLength + PreloadData.Length); }
        }

        public VpkEntry(string path, uint crc32, byte[] preloadBytes, ushort archiveIndex, uint entryOffset, uint entryLength)
        {
            Path = path;
            Crc32 = crc32;
            PreloadData = preloadBytes;
            ArchiveIndex = archiveIndex;
            EntryOffset = entryOffset;
            EntryLength = entryLength;
        }
    };
}