using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Sledge.FileSystem
{
    public class VirtualFile : IFile
    {
        public FileSystemType Type { get { return FileSystemType.Virtual; } }
        public bool IsContainer { get; private set; }
        public IFile Parent { get; private set; }
        public string FullPathName { get; private set; }
        public string Name { get; private set; }
        public string NameWithoutExtension { get; private set; }
        public string Extension { get; private set; }
        public bool Exists { get; private set; }
        public long Size { get; private set; }
        public int NumChildren { get { return Children.Count(x => x.IsContainer); } }
        public int NumFiles { get { return Children.Count(x => !x.IsContainer); } }

        public List<IFile> Children { get; private set; }

        public VirtualFile(IFile parent, string name)
        {
            Parent = parent;
            Name = name;
            IsContainer = false;
            FullPathName = parent != null ? Path.Combine(parent.FullPathName, name) : name;
            NameWithoutExtension = Path.GetFileNameWithoutExtension(name);
            Extension = (Path.GetExtension(Name) ?? "").TrimStart('.');
            Exists = true;
            Size = 0;
            Children = new List<IFile>();
        }

        public VirtualFile(IFile parent, string name, IEnumerable<IFile> children)
        {
            Parent = parent;
            Name = name;
            IsContainer = true;
            FullPathName = parent == null ? name : Path.Combine(parent.FullPathName, name);
            NameWithoutExtension = name;
            Extension = "";
            Exists = true;
            Size = 0;
            Children = children.ToList();
        }

        public Stream Open()
        {
            throw new NotImplementedException();
        }

        public byte[] ReadAll()
        {
            throw new NotImplementedException();
        }

        public byte[] Read(long offset, long count)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IFile> GetRelatedFiles()
        {
            return new List<IFile>();
        }

        public IFile GetRelatedFile(string extension)
        {
            return null;
        }

        public IFile GetChild(string name)
        {
            return Children.FirstOrDefault(x => x.Name == name);
        }

        public IEnumerable<IFile> GetChildren()
        {
            return Children.Where(x => x.IsContainer);
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
            return Children.Where(x => !x.IsContainer);
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
