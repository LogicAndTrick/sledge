using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Sledge.BspEditor.Environment;
using Sledge.BspEditor.Environment.Goldsource;
using Sledge.BspEditor.Primitives;
using Sledge.BspEditor.Providers;
using Sledge.Common.Shell.Documents;

namespace Sledge.BspEditor.Documents
{
    [Export(typeof(IDocumentLoader))]
    public class BspSourceDocumentLoader : IDocumentLoader
    {
        [ImportMany] private IEnumerable<Lazy<IBspSourceProvider>> _providers;

        public IEnumerable<FileExtensionInfo> SupportedFileExtensions
        {
            get { return _providers.SelectMany(x => x.Value.SupportedFileExtensions); }
        }

        public bool CanLoad(string location)
        {
            return location == null || _providers.Any(x => CanLoad(x.Value, location));
        }

        private bool CanLoad(IBspSourceProvider provider, string location)
        {
            return provider.SupportedFileExtensions.Any(x => x.Matches(location));
        }

        public async Task<IEnvironment> GetEnvironment()
        {
            return new GoldsourceEnvironment
            {
                BaseDirectory = @"F:\Steam\SteamApps\common\Half-Life",
                GameDirectory = "valve",
                ModDirectory =  "valve",
                Name = "Half-Life",
                FgdFiles =
                {
                    @"D:\Github\sledge\_Resources\FGD\Half-Life.fgd"
                }
            };
        }

        public async Task<IDocument> CreateBlank()
        {
            var env = await GetEnvironment();
            if (env == null) return null;
            return new MapDocument(new Map(), env);
        }

        public async Task<IDocument> Load(string location)
        {
            var env = await GetEnvironment();
            if (env == null) return null;

            using (var stream = File.Open(location, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                foreach (var provider in _providers.Where(x => CanLoad(x.Value, location)))
                {
                    try
                    {
                        var map = await provider.Value.Load(stream);
                        return new MapDocument(map, env) { FileName = location };
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
}
