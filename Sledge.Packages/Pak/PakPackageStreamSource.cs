using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Sledge.Common;

namespace Sledge.Packages.Pak
{
    internal class PakPackageStreamSource : IPackageStreamSource
    {
        private readonly PakPackage _package;
        private readonly Stream _stream;
        private readonly Dictionary<string, HashSet<string>> _folders;
        private readonly Dictionary<string, HashSet<string>> _files;

        public PakPackageStreamSource(PakPackage package)
        {
            _package = package;
            _stream = package.OpenFile(package.PackageFile);
            _folders = new Dictionary<string, HashSet<string>>();
            _files = new Dictionary<string, HashSet<string>>();
            foreach (var entry in package.GetEntries())
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

        private string GetName(string path)
        {
            var idx = path.LastIndexOf('/');
            if (idx < 0) return path;
            return path.Substring(idx + 1);
        }

        private string GetParent(string path)
        {
            var idx = path.LastIndexOf('/');
            if (idx < 0) return "";
            return path.Substring(0, idx);
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

        private PakEntry GetEntry(string path)
        {
            path = path.ToLowerInvariant();
            return _package.GetEntries().FirstOrDefault(x => x.FullName == path) as PakEntry;
        }

        public Stream OpenFile(string path)
        {
            var entry = GetEntry(path);
            if (entry == null) throw new FileNotFoundException();
            return new BufferedStream(new SubStream(_stream, entry.Offset, entry.Length));
        }

        public void Dispose()
        {
            _stream.Dispose();
        }
    }
}