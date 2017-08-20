using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Sledge.Common.Transport;

namespace Sledge.BspEditor.Editing.Components.Compile.Specification
{
    [Export(typeof(ICompileSpecificationProvider))]
    public class ApplicationEnvironmentSpecificationProvider : ICompileSpecificationProvider
    {
        [Import] private SerialisedObjectFormatter _parser;

        public async Task<IEnumerable<CompileSpecification>> GetSpecifications()
        {
            var specs = new List<CompileSpecification>();

            var specFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Specifications");
            if (!Directory.Exists(specFolder)) return specs;

            foreach (var file in Directory.GetFiles(specFolder, "*.vdf"))
            {
                try
                {
                    using (var f = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        var gs = _parser.Deserialize(f).ToList();
                        specs.AddRange(gs.Where(x => x.Name == "Specification").Select(CompileSpecification.Parse));
                    }
                }
                catch
                {
                    // Not a valid GS
                }
            }

            return specs;
        }
    }
}