using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Sledge.Libs.HLLib;

namespace Sledge.FileSystem
{
    /// <summary>
    /// Represents a PAK container or sub-file in the PAK file system.
    /// PAK isn't a subfolder, it's a part of the folder itself.
    /// This makes it slightly different to the GCF file where they only need to be treated as roots.
    /// </summary>
    public class PakFile : IFile
    {
        private FileInfo FileInfo { get; set; }
        private string FilePath { get; set; }

        public PakFile(string fileName)
        {
            FileInfo = new FileInfo(fileName);
            FilePath = null;
        }

        public PakFile(string fileName, string path)
        {
            FileInfo = new FileInfo(fileName);
            FilePath = path;
        }

        public FileSystemType Type
        {
            get { return FileSystemType.Pak; }
        }

        private bool _checkedIsContainer;
        private bool _isContainer;
        private void CheckIsContainer()
        {
            if (!_checkedIsContainer)
            {
                if (FilePath == null)
                {
                    _isContainer = true;
                }
                else
                {
                    HLLib.Initialize();
                    using (var pack = new HLLib.Package(FileInfo.FullName))
                    {
                        var subFolder = new HLLib.Folder(pack.GetRootFolder().GetItemByPath(FilePath, HLLib.FindType.Folders));
                        _isContainer = subFolder.Exists;
                    }
                    HLLib.Shutdown();
                }
            }
            _checkedIsContainer = true;
        }

        private T IfContainer<T>(Func<T> noPath, Func<T> container, Func<T> notContainer)
        {
            CheckIsContainer();
            if (FilePath == null) return noPath();
            return _isContainer ? container() : notContainer();
        }

        public IFile Parent
        {
            get
            {
                return IfContainer<IFile>(
                    () => new NativeFile(FileInfo.Directory.Parent), // The PAK pretends to be part of the native file system, so the parent is the PAK directory parent.
                    () => new PakFile(FileInfo.FullName, Path.GetDirectoryName(FilePath)),
                    () => new PakFile(FileInfo.FullName, Path.GetDirectoryName(FilePath))
                    );
            }
        }

        public string FullPathName
        {
            get
            {
                return IfContainer(
                    () => FileInfo.Directory.FullName,
                    () => Path.Combine(FileInfo.FullName, FilePath),
                    () => Path.Combine(FileInfo.FullName, FilePath)
                    );
            }
        }

        public string Name
        {
            get
            {
                return IfContainer(
                    () => FileInfo.Directory.Name,
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

        private bool _propertiesSet = false;
        private bool _exists = false;
        private int _size = 0;
        private int _numFiles = 0;

        private void SetProperties()
        {
            if (!_propertiesSet)
            {
                CheckIsContainer();
                HLLib.Initialize();
                using (var pack = new HLLib.Package(FileInfo.FullName))
                {
                    var root = pack.GetRootFolder();
                    if (IsContainer)
                    {
                        if (FilePath != null)
                        {
                            var item = new HLLib.Folder(root.GetItemByPath(FilePath, HLLib.FindType.Folders));
                            _exists = item.Exists;
                            _size = 0;
                            _numFiles = (int) item.ItemCount;
                        }
                        else
                        {
                            _exists = FileInfo.Exists;
                            _size = (int) FileInfo.Length;
                            _numFiles = (int) root.ItemCount;
                        }
                    }
                    else
                    {
                        var item = root.GetItemByPath(FilePath, HLLib.FindType.Files);
                        _exists = item.Exists;
                        _size = (int) item.Size;
                        _numFiles = 0;
                    }
                }
                HLLib.Shutdown();
            }
            _propertiesSet = true;
        }

        public bool Exists
        {
            get
            {
                SetProperties();
                return _exists;
            }
        }

        public long Size
        {
            get
            {
                SetProperties();
                return _size;
            }
        }

        public int NumFiles
        {
            get
            {

                SetProperties();
                return _numFiles;
            }
        }

        public bool IsContainer
        {
            get { return IfContainer(() => true, () => true, () => false); }
        }

        public int NumChildren
        {
            get { return 0; }
        }

        public Stream Open()
        {
            // Allow opening the PAK stream, but not a subfolder stream.
            return IfContainer<Stream>(
                () => FileInfo.OpenRead(),
                () => null,
                () => new PakFilePackageStream(FileInfo.FullName, FilePath)
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
            // PAKs do not have related files.
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

        private bool _contentsSet;
        private void SetContents()
        {
            if (!_contentsSet)
            {
                _children = new List<IFile>();
                _files = new List<IFile>();
                if (IsContainer)
                {
                    HLLib.Initialize();
                    using (var pack = new HLLib.Package(FileInfo.FullName))
                    {
                        var root = pack.GetRootFolder();
                        if (FilePath != null)
                        {
                            root = new HLLib.Folder(root.GetItemByPath(FilePath, HLLib.FindType.Folders));
                        }
                        var files = root.GetItems().ToList();
                        _children.AddRange(files.Where(x => x.Type == HLLib.DirectoryItemType.Folder)
                            .Select(item => new PakFile(FileInfo.FullName, Path.Combine(FilePath ?? "", item.Name))));
                        _files.AddRange(files.Where(x => x.Type == HLLib.DirectoryItemType.File)
                            .Select(item => new PakFile(FileInfo.FullName, Path.Combine(FilePath ?? "", item.Name))));
                    }
                    HLLib.Shutdown();
                }
            }
            _contentsSet = true;
        }

        private List<IFile> _children;
        public IEnumerable<IFile> GetChildren()
        {
            SetContents();
            return _children;
        }

        public IEnumerable<IFile> GetChildren(string regex)
        {
            return GetChildren().Where(x => Regex.IsMatch(x.Name, regex));
        }

        public IFile GetFile(string name)
        {
            return GetFiles().FirstOrDefault(x => x.Name.ToLower() == name.ToLower());
        }

        private List<IFile> _files;
        public IEnumerable<IFile> GetFiles()
        {
            SetContents();
            return _files;
        }

        public IEnumerable<IFile> GetFiles(string regex)
        {
            return GetFiles().Where(x => Regex.IsMatch(x.Name, regex));
        }

        public IEnumerable<IFile> GetFilesWithExtension(string extension)
        {
            return GetFiles().Where(x => x.Name.EndsWith("." + extension));
        }

        internal class PakFilePackageStream : Stream
        {
            private readonly string _pakPath;
            private readonly string _filePath;
            private bool _open;
            private HLLib.Package _package;
            private HLLib.Stream _stream;

            public PakFilePackageStream(string pakPath, string filePath)
            {
                _pakPath = pakPath;
                _filePath = filePath;
                Open();
            }

            private void Open()
            {
                if (_open) return;
                _open = true;
                HLLib.Initialize();
                _package = new HLLib.Package(_pakPath);
                var item = _package.GetRootFolder().GetItemByPath(_filePath, HLLib.FindType.Files);
                _stream = _package.CreateStream(item);
            }

            protected override void Dispose(bool disposing)
            {
                if (_open)
                {
                    _stream.Dispose();
                    _package.Dispose();
                    HLLib.Shutdown();
                }
                base.Dispose(disposing);
            }

            public override void Flush()
            {
                Open();
                _stream.Flush();
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                Open();
                return _stream.Seek(offset, origin);
            }

            public override void SetLength(long value)
            {
                Open();
                _stream.SetLength(value);
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                Open();
                return _stream.Read(buffer, offset, count);
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                Open();
                _stream.Write(buffer, offset, count);
            }

            public override bool CanRead
            {
                get { Open(); return _stream.CanRead; }
            }

            public override bool CanSeek
            {
                get { Open(); return _stream.CanSeek; }
            }

            public override bool CanWrite
            {
                get { Open(); return _stream.CanWrite; }
            }

            public override long Length
            {
                get { Open(); return _stream.Length; }
            }

            public override long Position
            {
                get { Open(); return _stream.Position; }
                set { Open(); _stream.Position = value; }
            }
        }
    }
}
