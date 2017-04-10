using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using LogicAndTrick.Gimme;
using LogicAndTrick.Gimme.Providers;
using LogicAndTrick.Oy;
using Sledge.Common.Logging;
using Sledge.Common.Shell.Documents;
using Sledge.Common.Shell.Hooks;

namespace Sledge.Shell.Registers
{
    /// <summary>
    /// The document register handles document loaders
    /// </summary>
    [Export(typeof(IStartupHook))]
    public class DocumentRegister : SyncResourceProvider<IDocumentLoader>, IStartupHook
    {
        [ImportMany] private IEnumerable<Lazy<IDocumentLoader>> _documentLoaders;

        public Task OnStartup()
        {
            // Register exported commands
            foreach (var export in _documentLoaders)
            {
                Log.Debug("Documents", "Loaded: " + export.Value.GetType().FullName);
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

        /// <summary>
        /// Register a document loader
        /// </summary>
        /// <param name="documentLoader">The loader</param>
        private void Add(IDocumentLoader documentLoader)
        {
            _loaders.Add(documentLoader);
        }

        /// <summary>
        /// Unregister a document loader
        /// </summary>
        /// <param name="documentLoader">The loader</param>
        private void Remove(IDocumentLoader documentLoader)
        {
            _loaders.Remove(documentLoader);
        }

        // Document loader resource provider

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