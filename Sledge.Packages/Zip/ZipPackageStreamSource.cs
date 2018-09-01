using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Sledge.Packages.Zip
{
    internal class ZipPackageStreamSource : IPackageStreamSource
    {
        private readonly ZipPackage _package;
        private readonly Stream _stream;
        private readonly HashSet<string> _files;

        public ZipPackageStreamSource(ZipPackage package)
        {
            _package = package;
            _stream = package.OpenFile(package.PackageFile);
            _files = new HashSet<string>();
            foreach (var entry in package.GetEntries())
            {
                // File name
                _files.Add(entry.FullName);
            }
        }

        private string GetName(string path)
        {
            return path;
        }

        public bool HasDirectory(string path)
        {
            if (path == "") return true;
            return false;
        }

        public bool HasFile(string path)
        {
            var entry = GetEntry(path);
            return entry != null;
        }

        public IEnumerable<string> GetDirectories()
        {
            return new List<string>();
        }

        public IEnumerable<string> GetFiles()
        {
            return _files;
        }

        public IEnumerable<string> GetDirectories(string path)
        {
            return new string[0];
        }

        public IEnumerable<string> GetFiles(string path)
        {
            return _files;
        }

        public IEnumerable<string> SearchDirectories(string path, string regex, bool recursive)
        {
            return _files.Where(x => Regex.IsMatch(GetName(x), regex, RegexOptions.IgnoreCase));
        }

        public IEnumerable<string> SearchFiles(string path, string regex, bool recursive)
        {
            return _files.Where(x => Regex.IsMatch(GetName(x), regex, RegexOptions.IgnoreCase));
        }

        private IEnumerable<string> CollectDirectories(string path)
        {
            return new string[0];
        }

        private IEnumerable<string> CollectFiles(string path)
        {
            return _files;
        }

        private ZipEntry GetEntry(string path)
        {
            path = path.ToLowerInvariant();
            var pe = _package.GetEntries().FirstOrDefault(x => x.FullName == path) as ZipEntry;
            return pe;
        }

        public Stream OpenFile(string path)
        {
            var entry = GetEntry(path);
            if (entry == null) throw new FileNotFoundException();
            return new BufferedStream(entry.Entry.GetStream(_stream));
        }

        public void Dispose()
        {
            _stream.Dispose();
        }
    }
}
