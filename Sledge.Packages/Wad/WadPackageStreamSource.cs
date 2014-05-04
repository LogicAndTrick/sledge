using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Sledge.Packages.Wad
{
    internal class WadPackageStreamSource : IPackageStreamSource
    {
        private readonly Stream _stream;
        private readonly Dictionary<string, WadEntry> _files;

        public WadPackageStreamSource(WadPackage package)
        {
            _stream = package.OpenFile(package.File);
            _files = package.GetEntries().ToDictionary(x => x.Name, x => x);
        }

        public IEnumerable<string> GetDirectories(string path)
        {
            return new String[0];
        }

        public IEnumerable<string> GetFiles(string path)
        {
            if (path != "") return new string[0];
            return _files.Keys;
        }

        public IEnumerable<string> SearchDirectories(string path, string regex, bool recursive)
        {
            return new String[0];
        }

        public IEnumerable<string> SearchFiles(string path, string regex, bool recursive)
        {
            var files = GetFiles(path);
            return files.Where(x => Regex.IsMatch(x, regex, RegexOptions.IgnoreCase));
        }

        private WadEntry GetEntry(string path)
        {
            return _files.ContainsKey(path) ? _files[path] : null;
        }

        public Stream OpenFile(string path)
        {
            var entry = GetEntry(path);
            if (entry == null) throw new FileNotFoundException();
            return new WadImageStream(entry, new SubStream(_stream, entry.Offset, entry.FullLength));
        }

        public void Dispose()
        {
            _stream.Dispose();
        }
    }
}