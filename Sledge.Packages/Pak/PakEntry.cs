namespace Sledge.Packages.Pak
{
    public class PakEntry
    {
        public string Path { get; private set; }
        public int Offset { get; private set; }
        public int Length { get; private set; }

        public PakEntry(string path, int offset, int length)
        {
            Path = path;
            Offset = offset;
            Length = length;
        }
    };
}