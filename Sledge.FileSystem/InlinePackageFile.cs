using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Sledge.Packages;
using Sledge.Packages.Pak;
using Sledge.Packages.Vpk;
using Sledge.Packages.Zip;

namespace Sledge.FileSystem
{
    /// <summary>
    /// Represents a PAK/VPK container or sub-file in these file system.
    /// PAK/VPK aren't subfolders, they are a part of the folder itself.
    /// </summary>
    public class InlinePackageFile : IFile
    {
        private IPackage _package;
        private IPackageEntry _entry;
        private string FilePath { get; set; }

        private HashSet<string> _files;
        private HashSet<string> _folders;

        public InlinePackageFile(string fileName)
        {
            switch (Path.GetExtension(fileName))
            {
                case ".pak":
                    _package = new PakPackage(new FileInfo(fileName));
                    break;
                case ".pk3":
                    _package = new ZipPackage(new FileInfo(fileName));
                    break;
                case ".vpk":
                    _package = new VpkDirectory(new FileInfo(fileName));
                    break;
                default:
                    throw new ArgumentException("This file format is not a valid inline package file.");
            }
            FilePath = "";
            _entry = null;
            _files = new HashSet<string>(_package.GetFiles(FilePath));
            _folders = new HashSet<string>(_package.GetDirectories(FilePath));
        }

        public InlinePackageFile(IPackage package, string path)
        {
            _package = package;
            _entry = package.GetEntry(path);
            FilePath = path;
            _files = new HashSet<string>(_package.GetFiles(FilePath));
            _folders = new HashSet<string>(_package.GetDirectories(FilePath));
        }

        public FileSystemType Type
        {
            get { return FileSystemType.Package; }
        }

        private T IfContainer<T>(Func<T> noPath, Func<T> container, Func<T> notContainer)
        {
            if (FilePath == null) return noPath();
            return _entry == null ? container() : notContainer();
        }

        public IFile Parent
        {
            get
            {
                var path = FilePath;
                var idx = path.LastIndexOf('/');
                if (idx < 0) path = "";
                path = path.Substring(0, idx);
                return IfContainer<IFile>(
                    () => new NativeFile(_package.PackageFile.Directory.Parent),
                    () => new InlinePackageFile(_package, path),
                    () => new InlinePackageFile(_package, path)
                    );
            }
        }

        public string FullPathName
        {
            get
            {

                return IfContainer(
                    () => _package.PackageFile.Directory.FullName,
                    () => Path.Combine(_package.PackageFile.FullName, FilePath),
                    () => Path.Combine(_package.PackageFile.FullName, FilePath)
                    );
            }
        }

        public string Name
        {
            get
            {
                return IfContainer(
                    () => _package.PackageFile.Directory.Name,
                    () => Path.GetFileName(FilePath),
                    () => Path.GetFileName(FilePath)
                    );
            }
        }

        public string NameWithoutExtension
        {
            get
            {
                return IfContainer(
                    () => Name,
                    () => Name,
                    () => Path.GetFileNameWithoutExtension(Name)
                    );
            }
        }

        public string Extension
        {
            get
            {
                return IfContainer(
                    () => "",
                    () => "",
                    () => (Path.GetExtension(Name) ?? "").TrimStart('.')
                    );
            }
        }

        public bool Exists
        {
            get
            {
                return _entry != null || _files.Any() || _folders.Any();
            }
        }

        public long Size
        {
            get { return IfContainer(() => 0, () => 0, () => _entry.Length); }
        }

        public int NumFiles
        {
            get { return _files.Count; }
        }

        public bool IsContainer
        {
            get { return IfContainer(() => true, () => true, () => false); }
        }

        public int NumChildren
        {
            get { return _folders.Count; }
        }

        public Stream Open()
        {
            // Allow opening the PAK stream, but not a subfolder stream.
            return IfContainer(
                () => null,
                () => null,
                () => _entry.Open()
                );
        }

        public byte[] ReadAll()
        {
            var stream = Open();
            if (stream == null) return new byte[0];
            using (stream)
            {
                var arr = new byte[stream.Length];
                stream.Read(arr, 0, arr.Length);
                return arr;
            }
        }

        public byte[] Read(long offset, long count)
        {
            var stream = Open();
            if (stream == null) return new byte[0];
            using (stream)
            {
                var arr = new byte[count];
                stream.Read(arr, (int) offset, (int) count);
                return arr;
            }
        }

        public IEnumerable<IFile> GetRelatedFiles()
        {
            // No related files.
            return new List<IFile>();
        }

        public IFile GetRelatedFile(string extension)
        {
            return null;
        }

        public IFile GetChild(string name)
        {
            return GetChildren().FirstOrDefault(x => x.Name.ToLower() == name.ToLower());
        }

        public IEnumerable<IFile> GetChildren()
        {
            return _folders.Select(x => new InlinePackageFile(_package, x));
        }

        public IEnumerable<IFile> GetChildren(string regex)
        {
            return GetChildren().Where(x => Regex.IsMatch(x.Name, regex, RegexOptions.IgnoreCase));
        }

        public IFile GetFile(string name)
        {
            return GetFiles().FirstOrDefault(x => x.Name.ToLower() == name.ToLower());
        }

        public IEnumerable<IFile> GetFiles()
        {
            return _files.Select(x => new InlinePackageFile(_package, x));
        }

        public IEnumerable<IFile> GetFiles(string regex)
        {
            return GetFiles().Where(x => Regex.IsMatch(x.Name, regex, RegexOptions.IgnoreCase));
        }

        public IEnumerable<IFile> GetFilesWithExtension(string extension)
        {
            return GetFiles().Where(x => x.Name.EndsWith("." + extension));
        }
    }
}
