using System;
using System.Collections.Generic;
using System.IO;

namespace Sledge.Packages
{
    public interface IPackageStreamSource : IDisposable
    {
        bool HasDirectory(string path);
        bool HasFile(string path);
        IEnumerable<string> GetDirectories();
        IEnumerable<string> GetFiles();
        IEnumerable<string> GetDirectories(string path);
        IEnumerable<string> GetFiles(string path);
        IEnumerable<string> SearchDirectories(string path, string regex, bool recursive);
        IEnumerable<string> SearchFiles(string path, string regex, bool recursive);
        Stream OpenFile(string path);
    }
}