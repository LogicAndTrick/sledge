using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Sledge.Libs.HLLib;

namespace Sledge.FileSystem
{
    public class WadInsideGcfFile : IFile
    {
        private string GcfFileName { get; set; }
        private string WadFileName { get; set; }
        private string FileName { get; set; }

        public WadInsideGcfFile(string gcfFileName, string wadFileName)
        {
            GcfFileName = gcfFileName;
            WadFileName = wadFileName;
        }

        public WadInsideGcfFile(string gcfFileName, string wadFileName, string fileName)
        {
            GcfFileName = gcfFileName;
            WadFileName = wadFileName;
            FileName = fileName;
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
            get
            {
                return IfContainer<IFile>(
                    () => new GcfFile(GcfFileName, Path.GetDirectoryName(WadFileName)),
                    () => new WadInsideGcfFile(GcfFileName, WadFileName)
                    );
            }
        }

        public string FullPathName
        {
            get
            {
                return IfContainer(
                    () => GcfFileName + @"\" + WadFileName,
                    () => GcfFileName + @"\" + WadFileName + @"\" + FileName
                    );
            }
        }

        public string Name
        {
            get { return IfContainer(() => Path.GetFileName(WadFileName), () => FileName); }
        }

        public string NameWithoutExtension
        {
            get { return IfContainer(() => Name, () => Path.GetFileNameWithoutExtension(Name)); }
        }

        public string Extension
        {
            get { return IfContainer(() => "", () => "bmp"); }
        }

        private bool _propertiesSet = false;
        private bool _exists = false;
        private int _size = 0;
        private int _numFiles = 0;

        private void SetProperties()
        {
            if (!_propertiesSet)
            {
                HLLib.Initialize();
                using (var pack = new HLLib.Package(GcfFileName))
                {
                    var item = pack.GetRootFolder().GetItemByPath(WadFileName, HLLib.FindType.Files);
                    var wad = new HLLib.File(item);
                    _exists = wad.Exists;
                    if (wad.Exists)
                    {
                        using (var wadPack = wad.OpenPackage())
                        {
                            if (IsContainer)
                            {
                                _size = (int) wad.Size;
                                _numFiles = (int) wadPack.GetRootFolder().ItemCount;
                            }
                            else
                            {
                                var wadItem = wadPack.GetRootFolder().GetItemByName(FileName, HLLib.FindType.Files);
                                _exists = wadItem.Exists;
                                _size = (int) wadItem.Size;
                            }
                        }
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
            get { return IfContainer(() => true, () => false); }
        }

        public int NumChildren
        {
            get { return 0; }
        }

        public Stream Open()
        {
            return IfContainer<Stream>(
                () => new GcfFile.GcfFilePackageStream(GcfFileName, WadFileName),
                () => new WadFileInsideGcfPackageStream(GcfFileName, WadFileName, FileName));
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
                stream.Read(arr, (int)offset, (int)count);
                return arr;
            }
        }

        private List<IFile> _relatedFiles;
        public IEnumerable<IFile> GetRelatedFiles()
        {
            if (_relatedFiles == null)
            {
                _relatedFiles = new List<IFile>();
                // See same method in WadFile.cs
                if (FileName != null && FileName.Length > 2 && (FileName.StartsWith("-") || FileName.StartsWith("-")))
                {
                    var pattern = FileName.Substring(0, 1) + "\\d" + FileName.Substring(2);
                    HLLib.Initialize();
                    using (var pack = new HLLib.Package(GcfFileName))
                    {
                        var item = pack.GetRootFolder().GetItemByPath(WadFileName, HLLib.FindType.Files);
                        var wad = new HLLib.File(item);
                        if (wad.Exists)
                        {
                            using (var wadPack = wad.OpenPackage())
                            {
                                var root = wadPack.GetRootFolder();
                                var files = root.GetItems().Where(x => x.Type == HLLib.DirectoryItemType.File);
                                _relatedFiles.AddRange(files.Where(x => Regex.IsMatch(x.Name, pattern)).Select(i => new WadInsideGcfFile(GcfFileName, WadFileName, i.Name)));
                            }
                        }
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
            return null;
        }

        public IEnumerable<IFile> GetChildren()
        {
            return new List<IFile>();
        }

        public IEnumerable<IFile> GetChildren(string regex)
        {
            return new List<IFile>();
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
                    using (var pack = new HLLib.Package(GcfFileName))
                    {
                        var item = pack.GetRootFolder().GetItemByPath(WadFileName, HLLib.FindType.Files);
                        var wad = new HLLib.File(item);
                        if (wad.Exists)
                        {
                            using (var wadPack = wad.OpenPackage())
                            {
                                var root = wadPack.GetRootFolder();
                                var files = root.GetItems().Where(x => x.Type == HLLib.DirectoryItemType.File);
                                _files.AddRange(files.Select(i => new WadInsideGcfFile(GcfFileName, WadFileName, i.Name)));
                            }
                        }
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

        internal class WadFileInsideGcfPackageStream : Stream
        {
            private readonly string _gcfPath;
            private readonly string _wadPath;
            private readonly string _fileName;
            private bool _open;
            private HLLib.Package _gcfPackage;
            private HLLib.Package _wadPackage;
            private HLLib.Stream _stream;

            public WadFileInsideGcfPackageStream(string gcfPath, string wadPath, string fileName)
            {
                _gcfPath = gcfPath;
                _wadPath = wadPath;
                _fileName = fileName;
                Open();
            }

            private void Open()
            {
                if (_open) return;
                _open = true;
                HLLib.Initialize();
                _gcfPackage = new HLLib.Package(_gcfPath);
                var wad = _gcfPackage.GetRootFolder().GetItemByPath(_wadPath, HLLib.FindType.Files);
                _wadPackage = new HLLib.File(wad).OpenPackage();
                var item = _wadPackage.GetRootFolder().GetItemByName(_fileName, HLLib.FindType.Files);
                _stream = _wadPackage.CreateStream(item);
            }

            protected override void Dispose(bool disposing)
            {
                if (_open)
                {
                    _stream.Dispose();
                    _wadPackage.Dispose();
                    _gcfPackage.Dispose();
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
