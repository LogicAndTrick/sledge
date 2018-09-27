using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LogicAndTrick.Oy;
using Microsoft.Win32;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Documents;
using Sledge.Common.Shell.Hooks;
using Sledge.Common.Shell.Settings;
using Sledge.Common.Threading;

namespace Sledge.Shell.Registers
{
    /// <summary>
    /// The document register handles document loaders
    /// </summary>
    [Export(typeof(IStartupHook))]
    [Export(typeof(ISettingsContainer))]
    [Export]
    public class DocumentRegister : IStartupHook, ISettingsContainer
    {
        private readonly ThreadSafeList<IDocument> _openDocuments;
        public IReadOnlyCollection<IDocument> OpenDocuments => _openDocuments;

        private readonly List<IDocumentLoader> _loaders;

        private readonly string _programId;
        private readonly string _programIdVer = "1";

        [ImportingConstructor]
        public DocumentRegister(
            [ImportMany] IEnumerable<Lazy<IDocumentLoader>> documentLoaders
        )
        {
            _loaders = documentLoaders.Select(x => x.Value).ToList();

            var assembly = Assembly.GetEntryAssembly()?.GetName().Name ?? "Sledge.Shell";
            _programId = assembly.Replace(".", "");

            _openDocuments = new ThreadSafeList<IDocument>();
        }

        public Task OnStartup()
        {
            RegisterExtensionHandlers();
            return Task.FromResult(0);
        }

        // Public interface

        public IEnumerable<FileExtensionInfo> GetSupportedFileExtensions(IDocument document)
        {
            return _loaders.Where(x => x.CanSave(document)).SelectMany(x => x.SupportedFileExtensions);
        }

        public DocumentPointer GetDocumentPointer(IDocument document)
        {
            var loader = _loaders.FirstOrDefault(x => x.CanSave(document));
            var pointer = loader?.GetDocumentPointer(document);
            return pointer;
        }

        // Save/load/open documents

        public bool IsOpen(string fileName)
        {
            return _openDocuments.Any(x => string.Equals(x.FileName, fileName, StringComparison.InvariantCultureIgnoreCase));
        }

        public IDocument GetDocumentByFileName(string fileName)
        {
            return _openDocuments.FirstOrDefault(x => string.Equals(x.FileName, fileName, StringComparison.InvariantCultureIgnoreCase));
        }

        public bool IsOpen(IDocument document)
        {
            return _openDocuments.Contains(document);
        }

        public async Task<IDocument> NewDocument(IDocumentLoader loader)
        {
            var doc = await loader.CreateBlank();
            if (doc != null) OpenDocument(doc);
            return doc;
        }

        public async Task<IDocument> OpenDocument(DocumentPointer documentPointer, string loaderName = null)
        {
            var fileName = documentPointer.FileName;
            if (!File.Exists(fileName)) return null;

            if (IsOpen(fileName))
            {
                ActivateDocument(GetDocumentByFileName(fileName));
                return null;
            }

            var loader = _loaders.FirstOrDefault(x => x.GetType().Name == loaderName && x.CanLoad(fileName));
            if (loader == null) return null;

            var doc = await loader.Load(documentPointer);
            if (doc != null) OpenDocument(doc);

            return doc;
        }

        public async Task<IDocument> OpenDocument(string fileName, string loaderHint = "")
        {
            if (!File.Exists(fileName)) return null;

            if (IsOpen(fileName))
            {
                ActivateDocument(GetDocumentByFileName(fileName));
                return null;
            }

            IDocumentLoader loader = null;
            if (!String.IsNullOrWhiteSpace(loaderHint)) loader = _loaders.FirstOrDefault(x => x.GetType().Name == loaderHint);
            if (loader == null) loader = _loaders.FirstOrDefault(x => x.CanLoad(fileName));
            
            if (loader != null)
            {
                var doc = await loader.Load(fileName);
                if (doc != null) OpenDocument(doc);
                return doc;
            }

            return null;
        }

        public async Task ActivateDocument(IDocument document)
        {
            if (document == null)
            {
                await Oy.Publish<IDocument>("Document:Activated", new NoDocument());
                await Oy.Publish("Context:Remove", new ContextInfo("ActiveDocument"));
            }
            else
            {
                await Oy.Publish("Document:Activated", document);
                await Oy.Publish("Context:Add", new ContextInfo("ActiveDocument", document));
            }
        }

        public Task<bool> ExportDocument(IDocument document, string fileName, string loaderHint = "")
        {
            return SaveDocument(document, fileName, loaderHint, false);
        }

        public Task<bool> SaveDocument(IDocument document, string fileName, string loaderHint = "")
        {
            return SaveDocument(document, fileName, loaderHint, true);
        }

        private async Task<bool> SaveDocument(IDocument document, string fileName, string loaderHint, bool switchFileName)
        {
            if (document == null || fileName == null) return false;

            IDocumentLoader loader = null;
            if (!String.IsNullOrWhiteSpace(loaderHint)) loader = _loaders.FirstOrDefault(x => x.GetType().Name == loaderHint);
            if (loader == null) loader = _loaders.FirstOrDefault(x => x.CanSave(document) && x.CanLoad(fileName));

            if (loader == null) return false;

            await Oy.Publish("Document:BeforeSave", document);

            await loader.Save(document, fileName);

            // Only publish document saved when the file name is changed
            // Otherwise we're not actually saving the document's file
            if (switchFileName)
            {
                document.FileName = fileName;
                await Oy.Publish("Document:Saved", document);
            }
            
            return true;
        }

        /// <summary>
        /// Request to close a document. The document will be closed
        /// (if possible) before returning.
        /// </summary>
        /// <param name="document">The document to close</param>
        /// <returns>True if the document was closed</returns>
        public async Task<bool> RequestCloseDocument(IDocument document)
        {
            var canClose = await document.RequestClose();

            var msg = new DocumentCloseMessage(document);
            await Oy.Publish("Document:RequestClose", msg);
            if (msg.Cancelled) canClose = false;

            if (canClose) ForceCloseDocument(document);
            return canClose;
        }

        public async Task ForceCloseDocument(IDocument document)
        {
            await Oy.Publish("Document:BeforeClose", document);

            _openDocuments.Remove(document);

            await Oy.Publish("Document:Closed", document);
        }

        private async Task OpenDocument(IDocument doc)
        {
            _openDocuments.Add(doc);
            await Oy.Publish("Document:Opened", doc);
            await ActivateDocument(doc);
        }
        
        // Settings provider

        public string Name => "Sledge.Shell.Documents";

        public IEnumerable<SettingKey> GetKeys()
        {
            yield return new SettingKey("FileAssociations", "Associations", typeof(FileAssociations));
        }

        public void LoadValues(ISettingsStore store)
        {
            if (!store.Contains("Associations")) return;

            var associations = store.Get("Associations", new FileAssociations());
            AssociateExtensionHandlers(associations.Where(x => x.Value).Select(x => x.Key));
        }

        public void StoreValues(ISettingsStore store)
        {
            var associations = new FileAssociations();
            var reg = GetRegisteredExtensionAssociations().ToList();
            foreach (var ext in _loaders.SelectMany(x => x.SupportedFileExtensions).SelectMany(x => x.Extensions))
            {
                associations[ext] = reg.Contains(ext, StringComparer.InvariantCultureIgnoreCase);
            }
            store.Set("Associations", associations);
        }

        public class FileAssociations : Dictionary<string, bool>
        {
            public FileAssociations Clone()
            {
                var b = new FileAssociations();
                foreach (var kv in this) b.Add(kv.Key, kv.Value);
                return b;
            }
        }

        private static string ExecutableLocation()
        {
            return Assembly.GetEntryAssembly().Location;
        }

        private void RegisterExtensionHandlers()
        {
            try
            {
                using (var root = Registry.CurrentUser.OpenSubKey("Software\\Classes", true))
                {
                    if (root == null) return;

                    foreach (var ext in _loaders.SelectMany(x => x.SupportedFileExtensions))
                    {
                        foreach (var extension in ext.Extensions)
                        {
                            using (var progId = root.CreateSubKey(_programId + extension + "." + _programIdVer))
                            {
                                if (progId == null) continue;

                                progId.SetValue("", ext.Description);

                                using (var di = progId.CreateSubKey("DefaultIcon"))
                                {
                                    di?.SetValue("", ExecutableLocation() + ",-40001");
                                }

                                using (var comm = progId.CreateSubKey("shell\\open\\command"))
                                {
                                    comm?.SetValue("", "\"" + ExecutableLocation() + "\" \"%1\"");
                                }

                                progId.SetValue("AppUserModelID", _programId);
                            }
                        }
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                // security exception or some such
            }
        }

        private void AssociateExtensionHandlers(IEnumerable<string> extensions)
        {
            try
            {
                using (var root = Registry.CurrentUser.OpenSubKey("Software\\Classes", true))
                {
                    if (root == null) return;

                    foreach (var extension in extensions)
                    {
                        using (var ext = root.CreateSubKey(extension))
                        {
                            if (ext == null) return;
                            ext.SetValue("", _programId + extension + "." + _programIdVer);
                            ext.SetValue("PerceivedType", "Document");

                            using (var openWith = ext.CreateSubKey("OpenWithProgIds"))
                            {
                                openWith?.SetValue(_programId + extension + "." + _programIdVer, string.Empty);
                            }
                        }
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                // security exception or some such
            }
        }

        private IEnumerable<string> GetRegisteredExtensionAssociations()
        {
            var associations = new List<string>();
            try
            {
                using (var root = Registry.CurrentUser.OpenSubKey("Software\\Classes"))
                {
                    if (root == null) return Enumerable.Empty<string>();

                    foreach (var ft in _loaders.SelectMany(x => x.SupportedFileExtensions))
                    {
                        foreach (var extension in ft.Extensions)
                        {
                            using (var ext = root.OpenSubKey(extension))
                            {
                                if (ext == null) continue;
                                if (Convert.ToString(ext.GetValue("")) == _programId + extension + "." + _programIdVer)
                                {
                                    associations.Add(extension);
                                }
                            }
                        }
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                // security exception or some such
            }

            return associations;
        }
    }
}