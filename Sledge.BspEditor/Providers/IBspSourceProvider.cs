using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Sledge.BspEditor.Environment;
using Sledge.BspEditor.Primitives;
using Sledge.Common.Shell.Documents;

namespace Sledge.BspEditor.Providers
{
    public interface IBspSourceProvider
    {
        IEnumerable<Type> SupportedDataTypes { get; }
        IEnumerable<FileExtensionInfo> SupportedFileExtensions { get; }
        Task<Map> Load(Stream stream, IEnvironment environment);
        Task Save(Stream stream, Map map);
    }
}