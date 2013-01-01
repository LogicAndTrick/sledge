using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Sledge.FileSystem
{
    public static class FileSystemFactory
    {
        private static readonly List<IFileSystemProvider> Providers;

        static FileSystemFactory()
        {
            Providers = new List<IFileSystemProvider>
                             {
                                 new GenericFileSystemProvider(FileSystemType.Native, File.Exists, Directory.Exists, x => new NativeFile(x)),

                                 //new GenericFileSystemProvider(FileSystemType.Composite, x => false, x => false, x => new CompositeFile(null, x))
                             };
        }

        public static IFile Create(string path)
        {
            var provider = Providers.FirstOrDefault(x => x.IsValidFile(path));
            return provider == null ? null : provider.Create(path);
        }
    }

    public interface IFileSystemProvider
    {
        FileSystemType Type { get; }
        bool IsValidFile(string path);
        bool IsContainer(string path);
        IFile Create(string path);
    }

    public class GenericFileSystemProvider : IFileSystemProvider
    {
        public FileSystemType Type { get; private set; }
        public Func<string, bool> IsValidFileFunc { get; private set; }
        public Func<string, bool> IsContainerFunc { get; private set; }
        public Func<string, IFile> CreateFunc { get; private set; }

        public GenericFileSystemProvider(FileSystemType type, Func<string, bool> isValidFileFunc,
            Func<string, bool> isContainerFunc, Func<string, IFile> createFunc)
        {
            Type = type;
            IsValidFileFunc = isValidFileFunc;
            IsContainerFunc = isContainerFunc;
            CreateFunc = createFunc;
        }

        public bool IsValidFile(string path)
        {
            return IsValidFileFunc(path);
        }

        public bool IsContainer(string path)
        {
            return IsContainerFunc(path);
        }

        public IFile Create(string path)
        {
            return CreateFunc(path);
        }
    }
}
