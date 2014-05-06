using System.IO;

namespace Sledge.Packages.Pak
{
    public class PakEntry : IPackageEntry
    {
        public PakPackage Package { get; private set; }

        public string Path { get; private set; }
        public int Offset { get; private set; }
        public string Name { get { return GetName(); }}
        public string FullName { get { return Path; } }
        public string ParentPath { get { return GetParent(); } }
        public long Length { get; private set; }

        public PakEntry(PakPackage package, string path, int offset, int length)
        {
            Package = package;
            Path = path;
            Offset = offset;
            Length = length;
        }

        public Stream Open()
        {
            return Package.OpenStream(this);
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