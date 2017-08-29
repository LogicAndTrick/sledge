using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sledge.BspEditor.Environment;
using Sledge.BspEditor.Environment.Controls;
using Sledge.BspEditor.Environment.Goldsource;
using Sledge.BspEditor.Primitives;
using Sledge.BspEditor.Providers;
using Sledge.Common.Shell.Documents;
using Sledge.Common.Translations;
using Sledge.Shell;

namespace Sledge.BspEditor.Documents
{
    [AutoTranslate]
    [Export(typeof(IDocumentLoader))]
    public class BspSourceDocumentLoader : IDocumentLoader
    {
        [ImportMany] private IEnumerable<Lazy<IBspSourceProvider>> _providers;
        [Import] private EnvironmentRegister _environments;

        public string FileTypeDescription { get; set; }

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

        private async Task<IEnvironment> GetEnvironment()
        {
            var envs = _environments.GetSerialisedEnvironments().ToList();
            SerialisedEnvironment chosenEnvironment = null;
            if (envs.Count == 1)
            {
                chosenEnvironment = envs[0];
            }
            else if (envs.Count > 1)
            {
                using (var esf = new EnvironmentSelectionForm(envs))
                {
                    var result = await esf.ShowDialogAsync();
                    if (result != DialogResult.OK) return null;
                    chosenEnvironment = esf.SelectedEnvironment;
                }
            }

            if (chosenEnvironment != null) return _environments.GetEnvironment(chosenEnvironment.ID);
            return new EmptyEnvironment();
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

        public bool CanSave(IDocument document)
        {
            return document is MapDocument;
        }

        public async Task Save(IDocument document, string location)
        {
            var map = (MapDocument) document;

            await map.Environment.UpdateDocumentData(map);

            using (var stream = new MemoryStream())
            {
                foreach (var provider in _providers.Where(x => CanLoad(x.Value, location)))
                {
                    try
                    {
                        await provider.Value.Save(stream, map.Map);
                        using (var fs = File.Open(location, FileMode.Create, FileAccess.Write, FileShare.Read))
                        {
                            stream.Seek(0, SeekOrigin.Begin);
                            await stream.CopyToAsync(fs);
                            return;
                        }
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
