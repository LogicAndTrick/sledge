using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Sledge.Libs.HLLib;

namespace Sledge.FileSystem
{
    /// <summary>
    /// Represents a WAD container or sub-file in the native file system
    /// </summary>
    public class WadFile : IFile
    {
        private FileInfo FileInfo { get; set; }
        private string FileName { get; set; }

        public WadFile(string fileName)
        {
            FileInfo = new FileInfo(fileName);
            FileName = null;
        }

        private WadFile(string fileName, string file)
        {
            FileInfo = new FileInfo(fileName);
            FileName = file.EndsWith(".bmp") ? file : file + ".bmp";
        }

        public FileSystemType Type
        {
            get { return FileSystemType.Wad; }
        }

        private T IfContainer<T>(Func<T> container, Func<T> notContainer)
        {
            return FileName == null ? container() : notContainer();
        }

        public IFile Parent
        {
            get { return IfContainer<IFile>(() => new NativeFile(FileInfo.Directory), () => new WadFile(FileInfo.FullName)); }
        }

        public string FullPathName
        {
            get { return IfContainer(() => FileInfo.FullName, () => Path.Combine(FileInfo.FullName, FileName)); }
        }

        public string Name
        {
            get { return IfContainer(() => FileInfo.Name, () => FileName); }
        }

        public string NameWithoutExtension
        {
            get { return IfContainer(() => Name, () => Path.GetFileNameWithoutExtension(Name)); }
        }

        public string Extension
        {
            get { return IfContainer(() => "", () => "bmp"); }
        }

        private int _exists = -1;
        public bool Exists
        {
            get
            {
                if (FileName == null) return FileInfo.Exists;

                if (_exists < 0)
                {
                    HLLib.Initialize();
                    using (var pack = new HLLib.Package(FileInfo.FullName))
                    {
                        var item = pack.GetRootFolder().GetItemByName(FileName, HLLib.FindType.Files);
                        _exists = item.Exists ? 1 : 0;
                    }
                    HLLib.Shutdown();
                }
                return _exists > 0;
            }
        }

        private int _size = -1;
        public long Size
        {
            get
            {
                if (FileName == null) return FileInfo.Length;

                if (_size < 0)
                {
                    HLLib.Initialize();
                    using (var pack = new HLLib.Package(FileInfo.FullName))
                    {
                        var item = pack.GetRootFolder().GetItemByName(FileName, HLLib.FindType.Files);
                        _size = (int) item.Size;
                    }
                    HLLib.Shutdown();
                }
                return _size;
            }
        }

        public bool IsContainer
        {
            get { return IfContainer(() => true, () => false); }
        }

        public int NumChildren
        {
            get { return 0; }
        }

        private int _numFiles = -1;
        public int NumFiles
        {
            get
            {
                if (FileName != null) return 0;

                if (_numFiles < 0)
                {
                    HLLib.Initialize();
                    using (var pack = new HLLib.Package(FileInfo.FullName))
                    {
                        _numFiles = (int) pack.GetRootFolder().ItemCount;
                    }
                    HLLib.Shutdown();
                }
                return _numFiles;
            }
        }

        public Stream Open()
        {
            // Allow this even if the WAD is a container.
            return IfContainer<Stream>(() => FileInfo.OpenRead(), () => new WadFilePackageStream(FileInfo.FullName, FileName));
        }

        public byte[] ReadAll()
        {
            using (var stream = Open())
            {
                var arr = new byte[stream.Length];
                stream.Read(arr, 0, arr.Length);
                return arr;
            }
        }

        public byte[] Read(long offset, long count)
        {
            using (var stream = Open())
            {
                var arr = new byte[count];
                stream.Read(arr, (int) offset, (int) count);
                return arr;
            }
        }

        private List<IFile> _relatedFiles;
        public IEnumerable<IFile> GetRelatedFiles()
        {
            if (_relatedFiles == null)
            {
                _relatedFiles = new List<IFile>();
                // Related WAD files: http://www.slackiller.com/tommy14/hltexture.htm
                // +0name to +9name: animated texture (toggle 1)
                // +Aname (to +Jname?): animated texture (toggle 2, group both toggles together)
                // -0name to -9name: randomly tiled texture
                if (FileName != null && FileName.Length > 2 && (FileName.StartsWith("-") || FileName.StartsWith("-")))
                {
                    var pattern = FileName.Substring(0, 1) + "\\d" + FileName.Substring(2);
                    HLLib.Initialize();
                    using (var pack = new HLLib.Package(FileInfo.FullName))
                    {
                        var root = pack.GetRootFolder();
                        var files = root.GetItems().Where(x => x.Type == HLLib.DirectoryItemType.File);
                        _relatedFiles.AddRange(files.Where(x => Regex.IsMatch(x.Name, pattern)).Select(item => new WadFile(FileInfo.FullName, item.Name)));
                    }
                    HLLib.Shutdown();
                }
            }
            return _relatedFiles;
        }

        public IFile GetRelatedFile(string extension)
        {
            return null;
        }

        public IFile GetChild(string name)
        {
            return null; // WADs don't have folder structure
        }

        public IEnumerable<IFile> GetChildren()
        {
            return new List<IFile>(); // Nope
        }

        public IEnumerable<IFile> GetChildren(string regex)
        {
            return new List<IFile>(); // They still don't
        }

        public IFile GetFile(string name)
        {
            return GetFiles().FirstOrDefault(x => x.Name.ToLower() == name.ToLower());
        }

        private List<IFile> _files;
        public IEnumerable<IFile> GetFiles()
        {
            if (_files == null)
            {
                _files = new List<IFile>();
                if (FileName == null)
                {
                    HLLib.Initialize();
                    using (var pack = new HLLib.Package(FileInfo.FullName))
                    {
                        var root = pack.GetRootFolder();
                        var files = root.GetItems().Where(x => x.Type == HLLib.DirectoryItemType.File);
                        _files.AddRange(files.Select(item => new WadFile(FileInfo.FullName, item.Name)));
                    }
                    HLLib.Shutdown();
                }
            }
            return _files;
        }

        public IEnumerable<IFile> GetFiles(string regex)
        {
            return GetFiles().Where(x => Regex.IsMatch(x.Name, regex));
        }

        public IEnumerable<IFile> GetFilesWithExtension(string extension)
        {
            if (FileName != null) return new List<IFile>();
            return extension.ToLower() == "bmp" ? GetFiles() : new List<IFile>();
        }

        internal class WadFilePackageStream : Stream
        {
            private readonly string _wadPath;
            private readonly string _fileName;
            private bool _open;
            private HLLib.Package _package;
            private HLLib.Stream _stream;

            public WadFilePackageStream(string wadPath, string fileName)
            {
                _wadPath = wadPath;
                _fileName = fileName;
                Open();
            }

            private void Open()
            {
                if (_open) return;
                _open = true;
                HLLib.Initialize();
                _package = new HLLib.Package(_wadPath);
                var item = _package.GetRootFolder().GetItemByName(_fileName, HLLib.FindType.Files);
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
