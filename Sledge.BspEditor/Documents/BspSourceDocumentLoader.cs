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
    /// <summary>
    /// A document loader for BSP source files.
    /// </summary>
    [AutoTranslate]
    [Export(typeof(IDocumentLoader))]
    public class BspSourceDocumentLoader : IDocumentLoader
    {
        private readonly IEnumerable<Lazy<IBspSourceProvider>> _providers;
        private readonly IEnumerable<Lazy<IBspSourceProcessor>> _processors;
        private readonly Lazy<EnvironmentRegister> _environments;
        private readonly Lazy<Form> _shell;

        /// <inheritdoc />
        public string FileTypeDescription { get; set; }
        public string UntitledDocumentName { get; set; }
        public string UnsupportedFileFormat { get; set; }
        public string FileTypeNotSupported { get; set; }
        public string LoadResultTitle { get; set; }
        public string InvalidObjectsWereDiscarded { get; set; }
        public string OK { get; set; }

        private static int _untitled = 1;

        [ImportingConstructor]
        public BspSourceDocumentLoader(
            [ImportMany] IEnumerable<Lazy<IBspSourceProvider>> providers,
            [ImportMany] IEnumerable<Lazy<IBspSourceProcessor>> processors,
            [Import] Lazy<EnvironmentRegister> environments,
            [Import("Shell")] Lazy<Form> shell
        )
        {
            _providers = providers;
            _processors = processors;
            _environments = environments;
            _shell = shell;
        }

        /// <inheritdoc />
        public IEnumerable<FileExtensionInfo> SupportedFileExtensions
        {
            get { return _providers.SelectMany(x => x.Value.SupportedFileExtensions); }
        }

        /// <inheritdoc />
        public bool CanLoad(string location)
        {
            return location == null || _providers.Any(x => CanLoad(x.Value, location));
        }

        /// <summary>
        /// See if a given provider can load a file from a given location.
        /// </summary>
        /// <param name="provider">The provider to test</param>
        /// <param name="location">The location of the file</param>
        private bool CanLoad(IBspSourceProvider provider, string location)
        {
            return location == null || provider.SupportedFileExtensions.Any(x => x.Matches(location));
        }

        /// <summary>
        /// Get an environment for the document.
        /// If more than one environment exists, then the user will be prompted.
        /// If the user chooses to cancel, null will be returned.
        /// If no environment is configured, <see cref="EmptyEnvironment"/> will be returned.
        /// </summary>
        private async Task<IEnvironment> GetEnvironment()
        {
            var envs = _environments.Value.GetSerialisedEnvironments().ToList();
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

            if (chosenEnvironment?.ID != null) return _environments.Value.GetEnvironment(chosenEnvironment.ID);
            return new EmptyEnvironment();
        }

        /// <inheritdoc />
        public async Task<IDocument> CreateBlank()
        {
            var env = await GetEnvironment();
            if (env == null) return null;

            var md =  new MapDocument(new Map(), env)
            {
                Name = string.Format(UntitledDocumentName, _untitled++),
                HasUnsavedChanges = true
            };
            await ProcessAfterLoad(env, md, new BspFileLoadResult { Map = md.Map });
            return md;
        }

        /// <inheritdoc />
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
                        var result = await provider.Value.Load(stream, env);
                        if (result.Map == null)
                        {
                            stream.Seek(0, SeekOrigin.Begin);
                            continue;
                        }

                        var md = new MapDocument(result.Map, env) { FileName = location };
                        await ProcessAfterLoad(env, md, result);
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
                MessageBox.Show(ex.Message, UnsupportedFileFormat, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            throw new NotSupportedException(FileTypeNotSupported);
        }

        /// <summary>
        /// Process a document after it has been loaded from a file.
        /// </summary>
        /// <param name="env">The document's environment</param>
        /// <param name="document">The document to process</param>
        /// <param name="result">The result of the load operation</param>
        private async Task ProcessAfterLoad(IEnvironment env, MapDocument document, BspFileLoadResult result)
        {
            await env.UpdateDocumentData(document);

            foreach (var p in _processors.Select(x => x.Value).OrderBy(x => x.OrderHint))
            {
                await p.AfterLoad(document);
            }

            if (result.InvalidObjects.Any() || result.Messages.Any())
            {
                var messages = new List<string>();
                if (result.InvalidObjects.Any()) messages.Add(String.Format(InvalidObjectsWereDiscarded, result.InvalidObjects.Count));
                foreach (var m in result.Messages) messages.Add(m);
                _shell.Value.InvokeSync(() =>
                {
                    MessageBox.Show(String.Join(System.Environment.NewLine, messages), LoadResultTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                });
            }
        }

        /// <inheritdoc />
        public bool CanSave(IDocument document)
        {
            return document is MapDocument;
        }

        /// <inheritdoc />
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
            throw new NotSupportedException(FileTypeNotSupported);
        }

        /// <summary>
        /// Process a document before it is saved to a file.
        /// This method will be run at any time, and may not be followed by a change to the document's underlying file.
        /// For example, when exporting a document for compile, this method will be called.
        /// </summary>
        /// <param name="document">The document to process</param>
        private async Task ProcessBeforeSave(MapDocument document)
        {
            foreach (var p in _processors.Select(x => x.Value).OrderBy(x => x.OrderHint))
            {
                await p.BeforeSave(document);
            }
        }

        /// <inheritdoc />
        public DocumentPointer GetDocumentPointer(IDocument document)
        {
            if (!(document is MapDocument doc)) return null;
            var so = new DocumentPointer(nameof(BspSourceDocumentLoader))
            {
                FileName = doc.FileName
            };
            so.Set("Environment", doc.Environment.ID);
            return so;
        }

        /// <inheritdoc />
        public async Task<IDocument> Load(DocumentPointer documentPointer)
        {
            var fileName = documentPointer.FileName;
            var envId = documentPointer.Get<string>("Environment");
            if (String.IsNullOrWhiteSpace(fileName) || String.IsNullOrWhiteSpace(envId)) return null;
            
            var env = _environments.Value.GetEnvironment(envId);
            if (env?.ID == null) return null;

            if (!File.Exists(fileName)) return null;

            using (var stream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                foreach (var provider in _providers.Where(x => CanLoad(x.Value, fileName)))
                {
                    try
                    {
                        var result = await provider.Value.Load(stream, env);
                        if (result.Map == null)
                        {
                            stream.Seek(0, SeekOrigin.Begin);
                            continue;
                        }

                        var md = new MapDocument(result.Map, env) { FileName = fileName };
                        await ProcessAfterLoad(env, md, result);
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
