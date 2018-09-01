using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sledge.FileSystem
{
    /// <summary>
    /// Represents a group of files or containers across multiple file systems.
    /// If multiple files are found with the same name, the first one is given priority.
    /// </summary>
    public class CompositeFile : IFile
    {
        private IEnumerable<IFile> Files { get; set; }

        public CompositeFile(IFile parent, IEnumerable<IFile> files)
        {
            Files = new List<IFile>(files.Where(x => x != null));
            Parent = parent;
            if (!Files.Any()) throw new FileNotFoundException("Cannot create a composite file with no files.");
        }

        public FileSystemType Type
        {
            get { return FileSystemType.Composite; }
        }

        public IEnumerable<IFile> GetCompositeFiles()
        {
            return new List<IFile>(Files);
        }

        public IFile Parent { get; set; }

        public virtual string FullPathName
        {
            get { return Parent == null ? "\\" : Path.Combine(Parent.FullPathName, Name); }
        }

        public IFile FirstFile => First(x => x);

        private T First<T>(Func<IFile, T> func)
        {
            var f = Files.FirstOrDefault(x => x.Exists) ?? Files.First();
            return func(f);
        }

        public string Name
        {
            get { return First(x => x.Name); }
        }

        public string NameWithoutExtension
        {
            get { return First(x => x.NameWithoutExtension); }
        }

        public string Extension
        {
            get { return First(x => x.Extension); }
        }

        public bool Exists
        {
            get { return Files.Any(x => x.Exists); }
        }

        public long Size
        {
            get { return First(x => x.Size); }
        }

        public bool IsContainer
        {
            get { return First(x => x.IsContainer); }
        }

        public int NumChildren
        {
            get { return Files.Sum(x => x.NumChildren); }
        }

        public int NumFiles
        {
            get { return Files.Sum(x => x.NumFiles); }
        }

        public Stream Open()
        {
            return First(x => x.Open());
        }

        public byte[] ReadAll()
        {
            return First(x => x.ReadAll());
        }

        public byte[] Read(long offset, long count)
        {
            return First(x => x.Read(offset, count));
        }

        private IEnumerable<IFile> MergeByName(IEnumerable<IFile> files)
        {
            return files.GroupBy(x => new {Name = x.Name.ToLower(), x.IsContainer})
                .Where(x => x.Any())
                .Select(x => new CompositeFile(this, x));
        }

        public IEnumerable<IFile> GetRelatedFiles()
        {
            return MergeByName(Files.SelectMany(x => x.GetRelatedFiles()));
        }

        public IFile GetRelatedFile(string extension)
        {
            return new CompositeFile(this, Files.Select(x => x.GetRelatedFile(extension)));
        }

        public IFile GetChild(string name)
        {
            var children = Files.Select(x => x.GetChild(name)).Where(x => x != null).ToList();
            return !children.Any() ? null : new CompositeFile(this, children);
        }

        public IEnumerable<IFile> GetChildren()
        {
            return MergeByName(Files.SelectMany(x => x.GetChildren()));
        }

        public IEnumerable<IFile> GetChildren(string regex)
        {
            return MergeByName(Files.SelectMany(x => x.GetChildren(regex)));
        }

        public IFile GetFile(string name)
        {
            var files = Files.Select(x => x.GetFile(name)).Where(x => x != null).ToList();
            return !files.Any() ? null : new CompositeFile(this, files);
        }

        public IEnumerable<IFile> GetFiles()
        {
            return MergeByName(Files.SelectMany(x => x.GetFiles()));
        }

        public IEnumerable<IFile> GetFiles(string regex)
        {
            return MergeByName(Files.SelectMany(x => x.GetFiles(regex)));
        }

        public IEnumerable<IFile> GetFilesWithExtension(string extension)
        {
            return MergeByName(Files.SelectMany(x => x.GetFilesWithExtension(extension)));
        }
    }
}
