using System;
using System.Collections.Generic;
using System.IO;

namespace Sledge.Packages
{
    public interface IPackage : IDisposable
    {
        IEnumerable<IPackageEntry> GetEntries();
        byte[] ExtractEntry(IPackageEntry entry);
        Stream OpenStream(IPackageEntry entry);
        IPackageStreamSource GetStreamSource();
    }
}