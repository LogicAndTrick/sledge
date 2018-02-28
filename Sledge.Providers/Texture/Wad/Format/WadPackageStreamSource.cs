using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sledge.Common;

namespace Sledge.Providers.Texture.Wad.Format
{
    public class WadPackageStreamSource : IDisposable
    {
        private readonly Stream _stream;
        private readonly Dictionary<string, WadEntry> _entries;

        public WadPackageStreamSource(WadPackage package)
        {
            _stream = package.File.Open();
            _entries = package.GetEntries().ToDictionary(x => x.Name, x => x, StringComparer.InvariantCultureIgnoreCase);
        }

        public bool HasEntry(string name)
        {
            return _entries.ContainsKey(name);
        }

        public IEnumerable<WadEntry> GetEntries()
        {
            return _entries.Values;
        }

        public WadEntry GetEntry(string name)
        {
            return _entries.ContainsKey(name) ? _entries[name] : null;
        }

        public Stream OpenEntry(WadEntry entry)
        {
            if (entry == null) throw new FileNotFoundException();
            return new WadImageStream(entry, new SubStream(_stream, entry.Offset, entry.Length));
        }

        public void Dispose()
        {
            _stream.Dispose();
        }
    }
}