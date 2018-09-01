using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Sledge.FileSystem
{
    public static class FileExtensions
    {
        /// <summary>
        /// Traverses a file path. If the path starts with a /, it will search from the root.
        /// If the path has a root name at the start followed by a colon, it will also search from the root.
        /// If a path contains . or .., they will be treated as the current and parent directories respectively.
        /// Otherwise, files and folders will be traversed until the end of the path.
        /// </summary>
        /// <param name="file">The file to start the traversal from</param>
        /// <param name="path">The path to traverse</param>
        /// <returns>The file at the end of the path. Returns null if the path was not found.</returns>
        public static IFile TraversePath(this IFile file, string path)
        {
            path = (path ?? "").Replace('\\', '/');
            var idx = path.IndexOf(":", StringComparison.InvariantCulture);
            if (idx > 0)
            {
                path = path.Substring(idx + 1);
                if (path.Length == 0 || path[0] != '/') path = '/' + path;
            }
            var f = file;
            var split = path.Split('/');
            for (var i = 0; i < split.Length; i++)
            {
                var name = split[i];
                if (i == 0 && name == "")
                {
                    while (f != null && f.Parent != null) f = f.Parent;
                    continue; // We've moved to the parent, start at the next path component
                }

                if (name == "") throw new FileNotFoundException("Invalid path.");
                if (f == null) return null;

                switch (name)
                {
                    case ".":
                        break;
                    case "..":
                        f = f.Parent;
                        break;
                    default:
                        var c = f.GetChild(name);
                        if (c != null)
                        {
                            f = c;
                            break;
                        }
                        c = f.GetFile(name);
                        if (c != null)
                        {
                            f = c;
                            break;
                        }
                        return null;
                }
            }
            return f;
        }

        private static IEnumerable<IFile> CollectChildren(IFile file)
        {
            var files = new List<IFile> { file };
            files.AddRange(file.GetChildren().SelectMany(CollectChildren));
            return files;
        }

        public static IEnumerable<IFile> GetFiles(this IFile file, bool recursive)
        {
            return !recursive ? file.GetFiles() : CollectChildren(file).SelectMany(x => x.GetFiles());
        }

        public static IEnumerable<IFile> GetFiles(this IFile file, string regex, bool recursive)
        {
            return !recursive ? file.GetFiles(regex) : CollectChildren(file).SelectMany(x => x.GetFiles(regex));
        }

        public static IEnumerable<IFile> GetChildren(this IFile file, bool recursive)
        {
            return !recursive ? file.GetChildren() : CollectChildren(file);
        }

        public static IEnumerable<IFile> GetChildren(this IFile file, string regex, bool recursive)
        {
            return !recursive ? file.GetChildren(regex) : CollectChildren(file).Where(x => Regex.IsMatch(x.Name, regex, RegexOptions.IgnoreCase));
        }

        public static string GetRelativePath(this IFile file, IFile relative)
        {
            var path = file.Name;
            var par = file;
            while (par != null && par.FullPathName != relative.FullPathName)
            {
                if (par.Parent != null) path = par.Parent.Name + "/" + path;
                par = par.Parent;
            }
            if (par == null) return file.FullPathName;
            return path;
        }

        /// <summary>
        /// For a file on disk, return the path to this file. Otherwise, will return null.
        /// </summary>
        public static string GetPathOnDisk(this IFile file)
        {
            if (file is CompositeFile cf) file = cf.FirstFile;
            if (file is NativeFile nf) return nf.FullPathName;
            return null;
        }
    }
}