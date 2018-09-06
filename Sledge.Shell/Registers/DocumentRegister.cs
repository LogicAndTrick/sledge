using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LogicAndTrick.Oy;
using Microsoft.Win32;
using Sledge.Common.Logging;
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
        private readonly IEnumerable<Lazy<IDocumentLoader>> _documentLoaders;

        private readonly ThreadSafeList<IDocument> _openDocuments;
        public IReadOnlyCollection<IDocument> OpenDocuments => _openDocuments;

        private readonly List<IDocumentLoader> _loaders;

        private readonly string _programId;
        private readonly string _programIdVer = "1";

        [ImportingConstructor]
        public DocumentRegister([ImportMany] IEnumerable<Lazy<IDocumentLoader>> documentLoaders)
        {
            _documentLoaders = documentLoaders;
            _loaders = new List<IDocumentLoader>();

            var assembly = Assembly.GetEntryAssembly()?.GetName().Name ?? "Sledge.Shell";
            _programId = assembly.Replace(".", "");

            _openDocuments = new ThreadSafeList<IDocument>();
        }

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

            
            Oy.Subscribe<IDocument>("Document:Opened", OpenDocument);
            Oy.Subscribe<IDocument>("Document:Closed", CloseDocument);
            
            RegisterExtensionHandlers();

            return Task.FromResult(0);
        }

        private Task OpenDocument(IDocument doc)
        {
            _openDocuments.Add(doc);
            return Task.CompletedTask;
        }

        private Task CloseDocument(IDocument doc)
        {
            _openDocuments.Remove(doc);
            return Task.CompletedTask;
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