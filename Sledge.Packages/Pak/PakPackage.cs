using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Sledge.Common;
using Sledge.Common.Extensions;

namespace Sledge.Packages.Pak
{
    // http://quakewiki.org/wiki/.pak
    public class PakPackage : IPackage
    {
        private const string Signature = "PACK";

        public FileInfo PackageFile { get; private set; }
        internal int TreeOffset { get; private set; }
        internal int TreeLength { get; private set; }
        internal List<PakEntry> Entries { get; private set; }

        public PakPackage(FileInfo packageFile)
        {
            PackageFile = packageFile;
            Entries = new List<PakEntry>();

            // Read the data from the pak
            using (var br = new BinaryReader(OpenFile(packageFile)))
            {
                var sig = br.ReadFixedLengthString(Encoding.ASCII, 4);
                if (sig != Signature) throw new PackageException("Unknown package signature: Expected '" + Signature + "', got '" + sig + "'.");

                TreeOffset = br.ReadInt32();
                TreeLength = br.ReadInt32();

                // Read all the entries from the pak
                ReadPackageEntries(br);
                BuildDirectories();
            }
        }

        internal Stream OpenFile(FileInfo file)
        {
            return Stream.Synchronized(new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 4096, FileOptions.RandomAccess));
        }

        private void ReadPackageEntries(BinaryReader br)
        {
            br.BaseStream.Position = TreeOffset;
            var numEntries = TreeLength / 64;
            for (int i = 0; i < numEntries; i++)
            {
                var path = br.ReadFixedLengthString(Encoding.ASCII, 56).ToLowerInvariant();
                var offset = br.ReadInt32();
                var length = br.ReadInt32();
                Entries.Add(new PakEntry(this, path, offset, length));
            }
        }

        public IEnumerable<IPackageEntry> GetEntries()
        {
            return Entries;
        }

        public IPackageEntry GetEntry(string path)
        {
            path = path.ToLowerInvariant();
            return GetEntries().FirstOrDefault(x => x.FullName == path);
        }

        public byte[] ExtractEntry(IPackageEntry entry)
        {
            using (var sr = new BinaryReader(OpenStream(entry)))
            {
                return sr.ReadBytes((int) sr.BaseStream.Length);
            }
        }

        public Stream OpenStream(IPackageEntry entry)
        {
            var pe = entry as PakEntry;
            if (pe == null) throw new ArgumentException("This package is only compatible with PakEntry objects.");
            return new BufferedStream(new SubStream(OpenFile(PackageFile), pe.Offset, pe.Length) { CloseParentOnDispose = true });
        }

        public IPackageStreamSource GetStreamSource()
        {
            return new PakPackageStreamSource(this);
        }

        public void Dispose()
        {
            Entries.Clear();
        }

        private Dictionary<string, HashSet<string>> _folders;
        private Dictionary<string, HashSet<string>> _files;

        private void BuildDirectories()
        {
            _folders = new Dictionary<string, HashSet<string>>();
            _files = new Dictionary<string, HashSet<string>>();
            foreach (var entry in GetEntries())
            {
                var split = entry.FullName.Split('/');
                var joined = "";
                for (var i = 0; i < split.Length; i++)
                {
                    var sub = split[i];
                    var name = joined.Length == 0 ? sub : joined + '/' + sub;
                    if (i == split.Length - 1)
                    {
                        // File name
                        if (!_files.ContainsKey(joined)) _files.Add(joined, new HashSet<string>());
                        _files[joined].Add(name);
                    }
                    else
                    {
                        // Folder name
                        if (!_folders.ContainsKey(joined)) _folders.Add(joined, new HashSet<string>());
                        if (!_folders[joined].Contains(sub)) _folders[joined].Add(name);
                    }
                    joined = joined.Length == 0 ? sub : joined + '/' + sub;
                }
            }
        }

        public bool HasDirectory(string path)
        {
            return _folders.ContainsKey(path);
        }

        public bool HasFile(string path)
        {
            return _files.ContainsKey(path.ToLowerInvariant());
        }

        public IEnumerable<string> GetDirectories()
        {
            return _files.Keys;
        }

        public IEnumerable<string> GetFiles()
        {
            return _files.Values.SelectMany(x => x);
        }

        public IEnumerable<string> GetDirectories(string path)
        {
            if (!_folders.ContainsKey(path)) return new string[0];
            return _folders[path].Where(x => x.Length > 0);
        }

        public IEnumerable<string> GetFiles(string path)
        {
            if (!_files.ContainsKey(path)) return new string[0];
            return _files[path];
        }

        public IEnumerable<string> SearchDirectories(string path, string regex, bool recursive)
        {
            var files = recursive ? CollectDirectories(path) : GetDirectories(path);
            return files.Where(x => Regex.IsMatch(GetName(x), regex, RegexOptions.IgnoreCase));
        }

        public IEnumerable<string> SearchFiles(string path, string regex, bool recursive)
        {
            var files = recursive ? CollectFiles(path) : GetFiles(path);
            return files.Where(x => Regex.IsMatch(GetName(x), regex, RegexOptions.IgnoreCase));
        }

        private IEnumerable<string> CollectDirectories(string path)
        {
            var files = new List<string>();
            if (_folders.ContainsKey(path))
            {
                files.AddRange(_folders[path].Where(x => x.Length > 0));
                files.AddRange(_folders[path].SelectMany(CollectDirectories));
            }
            return files;
        }

        private IEnumerable<string> CollectFiles(string path)
        {
            var files = new List<string>();
            if (_folders.ContainsKey(path))
            {
                files.AddRange(_folders[path].SelectMany(CollectFiles));
            }
            if (_files.ContainsKey(path))
            {
                files.AddRange(_files[path]);
            }
            return files;
        }

        private string GetName(string path)
        {
            var idx = path.LastIndexOf('/');
            if (idx < 0) return path;
            return path.Substring(idx + 1);
        }

        public Stream OpenFile(string path)
        {
            var entry = GetEntry(path);
            if (entry == null) throw new FileNotFoundException();
            return OpenStream(entry);
        }
    }
}
