using System.IO;

namespace Sledge.FileSystem
{
    public static class FileExtensions
    {
        /// <summary>
        /// Traverses a file path. If the path starts with a /, it will search from the root.
        /// If a path contains . or .., they will be treated as the current and parent directories respectively.
        /// Otherwise, files and folders will be traversed until the end of the path.
        /// </summary>
        /// <param name="file">The file to start the traversal from</param>
        /// <param name="path">The path to traverse</param>
        /// <returns>The file at the end of the path. Returns null if the path was not found.</returns>
        public static IFile TraversePath(this IFile file, string path)
        {
            var f = file;
            for (var i = 0; i < path.Split('/', '\\').Length; i++)
            {
                var name = path.Split('/', '\\')[i];
                if (i == 0 && name == "") while (f != null && f.Parent != null) f = f.Parent;
                else if (name == "") throw new FileNotFoundException("Invalid path.");

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
    }
}