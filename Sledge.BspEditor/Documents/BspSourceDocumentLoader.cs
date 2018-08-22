using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sledge.BspEditor.Environment;
using Sledge.BspEditor.Environment.Controls;
using Sledge.BspEditor.Environment.Empty;
using Sledge.BspEditor.Primitives;
using Sledge.BspEditor.Providers;
using Sledge.BspEditor.Providers.Processors;
using Sledge.Common.Shell.Documents;
using Sledge.Common.Translations;
using Sledge.Common.Transport;
using Sledge.Shell;

namespace Sledge.BspEditor.Documents
{
    [AutoTranslate]
    [Export(typeof(IDocumentLoader))]
    public class BspSourceDocumentLoader : IDocumentLoader
    {
        [ImportMany] private IEnumerable<Lazy<IBspSourceProvider>> _providers;
        [ImportMany] private IEnumerable<Lazy<IBspSourceProcessor>> _processors;
        [Import] private EnvironmentRegister _environments;
        [Import("Shell", typeof(Form))] private Lazy<Form> _shell;

        public string FileTypeDescription { get; set; }
        public string UntitledDocumentName { get; set; }

        private static int _untitled = 1;

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
            return location == null || provider.SupportedFileExtensions.Any(x => x.Matches(location));
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
                DialogResult result = DialogResult.Cancel;
                await _shell.Value.InvokeAsync(() =>
                {
                    using (var esf = new EnvironmentSelectionForm(envs))
                    {
                        result = esf.ShowDialog();
                        if (result == DialogResult.OK) chosenEnvironment = esf.SelectedEnvironment;
                    }
                });
                if (result != DialogResult.OK) return null;
            }

            if (chosenEnvironment != null) return _environments.GetEnvironment(chosenEnvironment.ID);
            return new EmptyEnvironment();
        }

        public async Task<IDocument> CreateBlank()
        {
            var env = await GetEnvironment();
            if (env == null) return null;

            var md =  new MapDocument(new Map(), env)
            {
                Name = string.Format(UntitledDocumentName, _untitled++),
                HasUnsavedChanges = true
            };
            await ProcessAfterLoad(md);
            return md;
        }

        public async Task<IDocument> Load(string location)
        {
            var env = await GetEnvironment();
            if (env == null) return null;

            NotSupportedException ex = null;
            using (var stream = File.Open(location, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                foreach (var provider in _providers.Where(x => CanLoad(x.Value, location)))
                {
                    try
                    {
                        var map = await provider.Value.Load(stream, env);
                        var md = new MapDocument(map, env) { FileName = location };
                        await ProcessAfterLoad(md);
                        return md;
                    }
                    catch (NotSupportedException e)
                    {
                        ex = e;
                        stream.Seek(0, SeekOrigin.Begin);
                    }
                }
            }

            if (ex != null)
            {
                // This file type is explicitly supported, but the provider rejected it.
                MessageBox.Show(ex.Message, "Unsupported file format", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            throw new NotSupportedException("This file type is not supported.");
        }

        private async Task ProcessAfterLoad(MapDocument document)
        {
            foreach (var p in _processors.Select(x => x.Value).OrderBy(x => x.OrderHint))
            {
                await p.AfterLoad(document);
            }
        }

        public bool CanSave(IDocument document)
        {
            return document is MapDocument;
        }

        public async Task Save(IDocument document, string location)
        {
            var map = (MapDocument) document;

            await map.Environment.UpdateDocumentData(map);

            await ProcessBeforeSave(map);

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

        private async Task ProcessBeforeSave(MapDocument document)
        {
            foreach (var p in _processors.Select(x => x.Value).OrderBy(x => x.OrderHint))
            {
                await p.BeforeSave(document);
            }
        }

        public SerialisedObject GetDocumentPointer(IDocument document)
        {
            if (!(document is MapDocument doc)) return null;
            var so = new SerialisedObject(nameof(BspSourceDocumentLoader));
            so.Set("FileName", doc.FileName);
            so.Set("Environment", doc.Environment.ID);
            return so;
        }

        public async Task<IDocument> Load(SerialisedObject documentPointer)
        {
            var fileName = documentPointer.Get<string>("FileName");
            var envId = documentPointer.Get<string>("Environment");
            if (String.IsNullOrWhiteSpace(fileName) || String.IsNullOrWhiteSpace(envId)) return null;
            
            var env = _environments.GetEnvironment(envId);
            if (env == null) return null;

            if (!File.Exists(fileName)) return null;

            using (var stream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                foreach (var provider in _providers.Where(x => CanLoad(x.Value, fileName)))
                {
                    try
                    {
                        var map = await provider.Value.Load(stream, env);
                        var md = new MapDocument(map, env) { FileName = fileName };
                        await ProcessAfterLoad(md);
                        return md;
                    }
                    catch (NotSupportedException)
                    {
                        stream.Seek(0, SeekOrigin.Begin);
                    }
                }
            }

            return null;
        }
    }
}
