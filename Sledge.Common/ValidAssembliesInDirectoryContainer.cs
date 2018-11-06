using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Sledge.Common
{
    public class ValidAssembliesInDirectoryContainer : ComposablePartCatalog
    {
        private readonly string _directory;
        private readonly Lazy<AggregateCatalog> _innerCatalog;

        public ValidAssembliesInDirectoryContainer(string directory)
        {
            _directory = directory;
            _innerCatalog = new Lazy<AggregateCatalog>(CreateInnerCatalog);
        }

        private AggregateCatalog CreateInnerCatalog()
        {
            var parts = new List<ComposablePartCatalog>();

            var dir = _directory;
            foreach (var dll in Directory.GetFiles(dir, "*.dll").Union(Directory.GetFiles(dir, "*.exe")))
            {
                try
                {
                    // See if the assembly is managed
                    AssemblyName.GetAssemblyName(dll);
                }
                catch
                {
                    // Invalid assembly; Skip
                    continue;
                }

                parts.Add(new AssemblyCatalog(dll));
            }

            return new AggregateCatalog(parts);
        }
        
        public override IEnumerator<ComposablePartDefinition> GetEnumerator()
        {
            return _innerCatalog.Value.GetEnumerator();
        }
    }
}