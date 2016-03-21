using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;

namespace Sledge.Packages.Zip
{
    public class ZipPackage : IPackage
    {
        public FileInfo PackageFile { get; private set; }
        internal List<ZipEntry> Entries { get; private set; }

        public ZipPackage(FileInfo packageFile)
        {
            PackageFile = packageFile;
            Entries = new List<ZipEntry>();

			// Read the data from the .zip
			using (ZipArchive br = new ZipArchive(OpenFile(packageFile)))
            {
                // Read all the entries from the .zip
	            IEnumerable<string> entries = br.GetFiles();
                foreach (string filePath in entries)
                {
	                string entryFixed = filePath.ToLowerInvariant();
	                ZipEntry pe;
                    switch (Path.GetExtension(entryFixed))
					{
						case ".png":
						case ".jpg":
						//case ".tga":
						case ".md3":
							MemoryStream fileStream = br.GetFileStream(filePath);
                            pe = new ZipEntry(this, filePath, fileStream);
							break;
							//break;
						default:
							continue;
					}

					Entries.Add(pe);
				}

                BuildDirectories();
            }
        }

        internal MemoryStream OpenFile(FileInfo file)
        {
			return new MemoryStream(File.ReadAllBytes(file.FullName));
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
            throw new Exception("Don't do this.");
        }

        public Stream OpenStream(IPackageEntry entry)
        {
            var pe = entry as ZipEntry;
            if (pe == null) throw new ArgumentException("This package is only compatible with ZipEntry objects.");
            return pe.GetStream();
        }

        public IPackageStreamSource GetStreamSource()
        {
            return new ZipPackageStreamSource(this);
        }

        public void Dispose()
        {
            Entries.Clear();
        }
        
        private HashSet<string> _files;

        private void BuildDirectories()
        {
            _files = new HashSet<string>();
            foreach (var entry in GetEntries())
            {
                _files.Add(entry.Name);
            }
        }

        public bool HasDirectory(string path)
        {
            return false;
        }

        public bool HasFile(string path)
        {
            return _files.Contains(path.ToLowerInvariant());
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
            if (path != "")
            {
                return new List<string>();
            }
            return _files;
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
            return new List<string>();
        }

        private IEnumerable<string> CollectFiles(string path)
        {
            var files = new List<string>();
            if (_files.Contains(path))
            {
                files.AddRange(_files);
            }
            return files;
        }

        private string GetName(string path)
        {
            return path;
        }

        public Stream OpenFile(string path)
        {
            var entry = GetEntry(path);
            if (entry == null) throw new FileNotFoundException();
            return OpenStream(entry);
        }
    }
}
