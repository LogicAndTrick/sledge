using System.IO;

namespace Sledge.Packages.Zip
{
    public class ZipEntry : IPackageEntry
    {
        public ZipPackage Package { get; private set; }

        public string Path { get; private set; }
        public string Name { get { return GetName();  } }
        public string FullName { get { return Path; } }
        public string ParentPath { get { return GetParent(); } }
        public long Length { get; private set; }
        public ZipArchive.FileEntry Entry { get; private set; }

        public ZipEntry(ZipPackage package, ZipArchive.FileEntry entry)
        {
            Package = package;
            Path = entry.FileName.ToLowerInvariant();
            Length = entry.UncompressedSize;
            Entry = entry;
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
    }
}
