using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sledge.FileSystem
{
    /// <summary>
    /// Represents a group of files or containers across multiple file systems.
    /// If multiple files are found with the same name, the last one is given priority.
    /// </summary>
    internal class CompositeFile : IFile
    {
        private IEnumerable<IFile> Files { get; set; }

        public CompositeFile(CompositeFile parent, IEnumerable<IFile> files)
        {
            Files = new List<IFile>(files.Where(x => x != null));
            Parent = parent;
            if (!Files.Any()) throw new FileNotFoundException("Cannot create a composite file with no files.");
            if (Files.Select(x => x.Type).Distinct().Count() > 0) throw new Exception("Cannot create a composite file with multiple different file types.");
        }

        public CompositeFile(CompositeFile parent, string filePaths)
        {
            var files = filePaths.Split(';').Select(FileSystemFactory.Create);
            Files = new List<IFile>(files.Where(x => x != null));
            Parent = parent;
            if (!Files.Any()) throw new FileNotFoundException("Cannot create a composite file with no files.");
            if (Files.Select(x => x.Type).Distinct().Count() > 0) throw new Exception("Cannot create a composite file with multiple different file types.");
        }

        public FileSystemType Type
        {
            get { return Files.Last(x => x.Exists).Type; }
        }

        public IFile Parent { get; set; }

        public string FullPathName
        {
            get { return String.Join(";", Files.Select(x => x.FullPathName)); }
        }

        public string Name
        {
            get { return Files.Last(x => x.Exists).Name; }
        }

        public string NameWithoutExtension
        {
            get { return Files.Last(x => x.Exists).NameWithoutExtension; }
        }

        public string Extension
        {
            get { return Files.Last(x => x.Exists).Extension; }
        }

        public bool Exists
        {
            get { return Files.Any(x => x.Exists); }
        }

        public long Size
        {
            get { return Files.Last(x => x.Exists).Size; }
        }

        public bool IsContainer
        {
            get { return Files.Last(x => x.Exists).IsContainer; }
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
            return Files.Last(x => x.Exists).Open();
        }

        public byte[] ReadAll()
        {
            return Files.Last(x => x.Exists).ReadAll();
        }

        public byte[] Read(long offset, long count)
        {
            return Files.Last(x => x.Exists).Read(offset, count);
        }

        private IEnumerable<IFile> MergeByName(IEnumerable<IFile> files)
        {
            return files.GroupBy(x => new { Name = x.Name.ToLower(), x.IsContainer }).Select(x => new CompositeFile(this, x));
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
            return new CompositeFile(this, Files.Select(x => x.GetChild(name)));
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
            return new CompositeFile(this, Files.Select(x => x.GetFile(name)));
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
