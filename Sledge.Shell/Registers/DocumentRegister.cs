using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Threading.Tasks;
using LogicAndTrick.Gimme;
using LogicAndTrick.Gimme.Providers;
using LogicAndTrick.Oy;
using Sledge.Common.Documents;
using Sledge.Common.Hooks;

namespace Sledge.Shell.Registers
{
    [Export(typeof(IStartupHook))]
    public class DocumentRegister : SyncResourceProvider<IDocumentLoader>, IStartupHook
    {
        public Task OnStartup(CompositionContainer container)
        {
            // Register exported commands
            foreach (var export in container.GetExports<IDocumentLoader>())
            {
                Add(export.Value);
            }

            // Listen for dynamically added/removed document loaders
            Oy.Subscribe<IDocumentLoader>("DocumentLoader:Register", c => Add(c));
            Oy.Subscribe<IDocumentLoader>("DocumentLoader:Unregister", c => Remove(c));

            // Register the resource provider
            Gimme.Register(this);

            return Task.FromResult(0);
        }

        private readonly List<IDocumentLoader> _loaders;

        public DocumentRegister()
        {
            _loaders = new List<IDocumentLoader>();
        }

        private void Add(IDocumentLoader documentLoader)
        {
            _loaders.Add(documentLoader);
        }

        private void Remove(IDocumentLoader documentLoader)
        {
            _loaders.Remove(documentLoader);
        }

        public override bool CanProvide(string location)
        {
            return _loaders.Any(x => x.CanLoad(location));
        }

        public override IEnumerable<IDocumentLoader> Fetch(string location, List<string> resources)
        {
            return _loaders.Where(x => x.CanLoad(location));
        }
    }
}