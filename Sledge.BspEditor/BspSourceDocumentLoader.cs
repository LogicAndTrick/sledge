using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives;
using Sledge.BspEditor.Providers;
using Sledge.Common.Components;
using Sledge.Common.Context;
using Sledge.Common.Documents;
using Sledge.Common.Hooks;

namespace Sledge.BspEditor
{
    [Export(typeof(IDocumentLoader))]
    public class BspSourceDocumentLoader : IDocumentLoader
    {
        [ImportMany] private IEnumerable<Lazy<IBspSourceProvider>> _providers;

        public bool CanLoad(string location)
        {
            return _providers.Any(x => CanLoad(x.Value, location));
        }

        private bool CanLoad(IBspSourceProvider provider, string location)
        {
            return provider.SupportedFileExtensions.Any(x => location.EndsWith(x, true, CultureInfo.InvariantCulture));
        }

        public async Task<IDocument> CreateBlank()
        {
            return new MapDocument(new Map());
        }

        public async Task<IDocument> Load(string location)
        {
            using (var stream = File.Open(location, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                foreach (var provider in _providers.Where(x => CanLoad(x.Value, location)))
                {
                    try
                    {
                        var map = await provider.Value.Load(stream);
                        return new MapDocument(map);
                    }
                    catch (NotSupportedException)
                    {
                        stream.Seek(0, SeekOrigin.Begin);
                    }
                }
            }
            throw new NotSupportedException("This file type is not supported.");
        }
    }

    [Export(typeof(ISidebarComponent))]
    [Export(typeof(IInitialiseHook))]
    public class BspSourceSidebarComponent : ISidebarComponent, IInitialiseHook
    {
        private ListBox _control;
        public string Title => "BSP Source Loaders";
        public object Control => _control;

        [ImportMany] private IEnumerable<Lazy<IBspSourceProvider>> _providers;

        public BspSourceSidebarComponent()
        {
            _control = new ListBox();
        }

        public bool IsInContext(IContext context)
        {
            return true;
        }

        public async Task OnInitialise()
        {
            _control.Invoke((MethodInvoker) delegate
            {
                foreach (var provider in _providers)
                {
                    _control.Items.Add(provider.Value.GetType().Name);
                }
            });
        }
    }
}
