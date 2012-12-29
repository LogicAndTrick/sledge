using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Sledge.FileSystem
{
    /// <summary>
    /// An implementation of the native windows file system using the IFile interface.
    /// </summary>
    public class NativeFile : IFile
    {
        protected FileInfo FileInfo { get; set; }
        protected DirectoryInfo DirectoryInfo { get; set; }

        public NativeFile(FileInfo fileInfo)
        {
            FileInfo = fileInfo;
        }

        public NativeFile(DirectoryInfo directoryInfo)
        {
            DirectoryInfo = directoryInfo;
        }

        public NativeFile(string filePath)
        {
            if (Directory.Exists(filePath)) DirectoryInfo = new DirectoryInfo(filePath);
            else FileInfo = new FileInfo(filePath);
        }

        public FileSystemType Type
        {
            get { return FileSystemType.Native; }
        }

        public IFile Parent
        {
            get { return new NativeFile(IsContainer ? DirectoryInfo.Parent : FileInfo.Directory); }
        }

        public string FullPathName
        {
            get { return IsContainer ? DirectoryInfo.FullName : FileInfo.FullName; }
        }

        public string Name
        {
            get { return IsContainer ? DirectoryInfo.Name : FileInfo.Name; }
        }

        public string NameWithoutExtension
        {
            get { return IsContainer ? "" : Path.GetFileNameWithoutExtension(FileInfo.FullName); }
        }

        public string Extension
        {
            get { return IsContainer ? "" : FileInfo.Extension.Any() ? FileInfo.Extension.Substring(1) : ""; }
        }

        public bool Exists
        {
            get { return IsContainer ? DirectoryInfo.Exists : FileInfo.Exists; }
        }

        public long Size
        {
            get { return IsContainer ? 0 : FileInfo.Length; }
        }

        public bool IsContainer
        {
            get { return DirectoryInfo != null; }
        }

        public int NumChildren
        {
            get { return IsContainer ? DirectoryInfo.GetDirectories().Length : 0; }
        }

        public int NumFiles
        {
            get { return IsContainer ? DirectoryInfo.GetFiles().Length : 0; }
        }

        public Stream Open()
        {
            if (IsContainer) throw new FileNotFoundException("Unable to open a container.");
            return FileInfo.Open(FileMode.Open);
        }

        public byte[] ReadAll()
        {
            return File.ReadAllBytes(FileInfo.FullName);
        }

        public byte[] Read(long offset, long count)
        {
            // Just read the whole damn file because I'm lazy
            var barr = new byte[count];
            Array.Copy(ReadAll(), offset, barr, 0, count);
            return barr;
        }

        public IEnumerable<IFile> GetRelatedFiles()
        {
            if (IsContainer || Parent == null) return new List<IFile>();
            var thisName = NameWithoutExtension.ToLower();
            return Parent.GetFiles().Where(file => file.Name.Split('.')[0].ToLower() == thisName);
        }

        public IFile GetRelatedFile(string extension)
        {
            return GetRelatedFiles().FirstOrDefault(x => x.Extension.ToLower() == extension.ToLower());
        }

        public IFile GetChild(string name)
        {
            return IsContainer ? GetChildren().FirstOrDefault(x => x.Name.ToLower() == name.ToLower()) : null;
        }

        public IEnumerable<IFile> GetChildren()
        {
            return !IsContainer
                       ? (IEnumerable<IFile>) new List<IFile>()
                       : DirectoryInfo.GetDirectories().Select(directoryInfo => new NativeFile(directoryInfo));
        }

        public IEnumerable<IFile> GetChildren(string regex)
        {
            return !IsContainer
                       ? (IEnumerable<IFile>) new List<IFile>()
                       : DirectoryInfo.GetDirectories().Where(x => Regex.IsMatch(x.Name, regex))
                             .Select(directoryInfo => new NativeFile(directoryInfo));
        }

        public IFile GetFile(string name)
        {
            return IsContainer ? GetFiles().FirstOrDefault(x => x.Name.ToLower() == name.ToLower()) : null;
        }

        public IEnumerable<IFile> GetFiles()
        {
            return !IsContainer
                       ? (IEnumerable<IFile>)new List<IFile>()
                       : DirectoryInfo.GetFiles().Select(fileInfo => new NativeFile(fileInfo));
        }

        public IEnumerable<IFile> GetFiles(string regex)
        {
            return !IsContainer
                       ? (IEnumerable<IFile>)new List<IFile>()
                       : DirectoryInfo.GetFiles().Where(x => Regex.IsMatch(x.Name, regex))
                             .Select(fileInfo => new NativeFile(fileInfo));
        }

        public IEnumerable<IFile> GetFilesWithExtension(string extension)
        {
            return !IsContainer
                       ? (IEnumerable<IFile>)new List<IFile>()
                       : DirectoryInfo.GetFiles().Where(x => x.Extension.ToLower() == extension.ToLower())
                             .Select(fileInfo => new NativeFile(fileInfo));
        }
    }
}
