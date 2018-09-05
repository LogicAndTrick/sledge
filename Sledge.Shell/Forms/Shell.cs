using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Documents;
using Sledge.Common.Shell.Settings;
using Sledge.Common.Translations;
using Sledge.Common.Transport;
using Sledge.Shell.Controls;
using Sledge.Shell.Registers;

namespace Sledge.Shell.Forms
{
    /// <summary>
    /// The application's base window
    /// </summary>
    [Export]
    [Export("Shell", typeof(Form))]
    [Export(typeof(ISettingsContainer))]
    [AutoTranslate]
    internal partial class Shell : BaseForm, ISettingsContainer
    {
        private readonly List<IDocument> _documents;
        private readonly object _lock = new object();

        private Lazy<Bootstrapper> _bootstrapper;
        private IEnumerable<Lazy<IDocumentLoader>> _loaders;
        private Lazy<DocumentRegister> _documentRegister;
        private Lazy<ITranslationStringProvider> _translation;

        public string Title { get; set; } = "Sledge Shell";

        public string UnsavedChanges { get; set; } = "Unsaved changes in file";
        public string SaveChangesToFile { get; set; } = "Save changes to {0}?";

        internal ToolStripPanel ToolStrip => ToolStripContainer.TopToolStripPanel;

        [ImportingConstructor]
        public Shell(
            [Import] Lazy<Bootstrapper> bootstrapper, 
            [ImportMany] IEnumerable<Lazy<IDocumentLoader>> loaders, 
            [Import] Lazy<DocumentRegister> documentRegister, 
            [Import] Lazy<ITranslationStringProvider> translation
        )
        {
            _bootstrapper = bootstrapper;
            _loaders = loaders;
            _documentRegister = documentRegister;
            _translation = translation;

            _documents = new List<IDocument>();

            InitializeComponent();
            InitializeShell();
        }

        /// <summary>
        /// Setup the shell pre-startup
        /// </summary>
        private void InitializeShell()
        {
            DocumentTabs.TabPages.Clear();
            
            Oy.Subscribe<List<string>>("Shell:InstanceOpened", async a => await this.InvokeAsync(() => InstanceOpened(a)));

            Oy.Subscribe<IDocument>("Document:Opened", async d => await this.InvokeAsync(() => OpenDocument(d)));
            Oy.Subscribe<IDocument>("Document:Closed", async d => await this.InvokeLaterAsync(() => CloseDocument(d)));
            Oy.Subscribe<IDocument>("Document:Changed", async d => await this.InvokeAsync(() => DocumentChanged(d)));
            Oy.Subscribe<IDocument>("Document:Switch", async d => await this.InvokeAsync(() => DocumentSwitch(d)));
            Oy.Subscribe<IDocument>("Document:CloseAndPrompt", async d => await this.InvokeAsync(() => DocumentCloseAndPrompt(d)));

            Oy.Subscribe<string>("Shell:OpenCommandBox", async o => await this.InvokeAsync(() => OpenCommandBox(o)));

            Oy.Subscribe<Exception>("Shell:UnhandledException", ShowExceptionDialog);
            Oy.Subscribe<Exception>("Shell:UnhandledExceptionOnce", ShowExceptionDialogOnce);
        }

        private Task ShowExceptionDialog(Exception obj)
        {
            this.InvokeLater(() => {
                var ed = new ExceptionWindow(obj);
                ed.ShowDialog(this);
            });
            return Task.CompletedTask;
        }

        private Task ShowExceptionDialogOnce(Exception obj)
        {
            return ShowException(obj) ? ShowExceptionDialog(obj) : Task.CompletedTask;
        }

        private readonly HashSet<string> _shownExceptions = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

        private string GetExceptionData(Exception exception)
        {
            return exception == null ? "" : $"{exception.Message}|{exception.StackTrace}|{GetExceptionData(exception.InnerException)}";
        }

        private bool ShowException(Exception exception)
        {
            var exData = GetExceptionData(exception);
            if (_shownExceptions.Contains(exData)) return false;

            _shownExceptions.Add(exData);
            return true;
        }

        private async Task InstanceOpened(List<string> args)
        {
            foreach (var arg in args.Skip(1))
            {
                Common.Logging.Log.Debug(nameof(Shell), $"Command line: `{arg}`");
                if (!File.Exists(arg)) continue;

                await Oy.Publish("Command:Run", new CommandMessage("Internal:OpenDocument", new
                {
                    Path = arg
                }));
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            // Bootstrap the shell
            _bootstrapper.Value.UIStartup();
            _bootstrapper.Value.Startup()
                .ContinueWith(_ => _bootstrapper.Value.Initialise())
                .ContinueWith(_ => PostLoad());

            // Set up bootstrapping for shutdown
            Closing += DoClosing;

            Text = Title;
        }

        private async Task PostLoad()
        {
            if (_openDocuments != null)
            {
                foreach (var dp in _openDocuments)
                {
                    if (!File.Exists(dp.FileName)) continue;
                    
                    var loader = _loaders.Select(x => x.Value).FirstOrDefault(x => x.GetType().Name == dp.Loader && x.CanLoad(dp.FileName));
                    if (loader == null) continue;
                    
                    var doc = await loader.Load(dp.ToPointer());
                    if (doc != null)
                    {
                        await Oy.Publish("Document:Opened", doc);
                    }
                }
            }
            await Oy.Publish("Shell:InstanceOpened", Environment.GetCommandLineArgs().ToList());
        }

        private void CancelClose(object sender, CancelEventArgs e) => e.Cancel = true;

        private async void DoClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            Closing -= DoClosing;
            Closing += CancelClose;

            await Task.Yield();

            // Save any unsaved documents
            if (!await SaveUnsavedDocuments())
            {
                Closing += DoClosing;
                Closing -= CancelClose;
                return;
            }

            // Try to close all the open documents
            SaveOpenDocuments();

            // Get the list of currently open documents
            List<IDocument> docs;
            lock (_lock) docs = _documents.ToList();

            foreach (var doc in docs)
            {
                // Request to close the document
                await Oy.Publish("Document:RequestClose", doc);
                lock (_lock)
                {
                    // If the document is still open than we can't continue
                    if (!_documents.Contains(doc)) continue;

                    Closing += DoClosing;
                    Closing -= CancelClose;
                    return;
                }
            }

            // Close anything else
            if (!await _bootstrapper.Value.ShuttingDown())
            {
                Closing += DoClosing;
                Closing -= CancelClose;
                return;
            }

            // Unsubscribe the event (no infinite loops!) and close for good
            Closing -= CancelClose;
            Enabled = false;
            this.InvokeSync(() => { 
                _bootstrapper.Value.UIShutdown();
            });
            await _bootstrapper.Value.Shutdown();
            Close();
        }

        private void SaveOpenDocuments()
        {
            _openDocuments = new List<DocumentPointer>();
            lock (_lock)
            {
                foreach (var d in _documents)
                {
                    var loader = _loaders.Select(x => x.Value).FirstOrDefault(x => x.CanSave(d));
                    var pointer = loader?.GetDocumentPointer(d);
                    if (pointer != null)
                    {
                        var dp = new DocumentPointer(pointer);
                        _openDocuments.Add(dp);
                    }
                }
            }
        }

        private async Task<bool> SaveUnsavedDocuments()
        {
            var unsaved = _documentRegister.Value.OpenDocuments.Where(x => x.HasUnsavedChanges).ToList();
            if (!unsaved.Any()) return true; // nothing unsaved

            DialogResult result;

            using (var d = new SaveChangesForm(unsaved))
            {
                d.Translate(_translation.Value);
                d.Owner = this;
                result = await d.ShowDialogAsync();
            }

            // Don't continue
            if (result == DialogResult.Cancel) return false;

            // Discard changes and continue
            if (result == DialogResult.No) return true;

            // Save changes and continue
            foreach (var doc in unsaved)
            {
                await SaveDocument(doc);
            }

            return true;
        }

        /// <summary>
        /// Get the list of docking panels in the shell
        /// </summary>
        /// <returns>The list of docking panels</returns>
        internal IEnumerable<DockedPanel> GetDockPanels()
        {
            yield return LeftSidebar;
            yield return RightSidebar;
            yield return BottomSidebar;
        }
        
        // Subscriptions

        private Task OpenDocument(IDocument document)
        {
            lock (_lock)
            {
                if (_documents.Contains(document)) return Task.CompletedTask;

                _documents.Add(document);
                var page = new TabPage
                {
                    Text = document.Name,
                    Tag = document
                };
                DocumentTabs.TabPages.Add(page);
                page.ImageKey = document.HasUnsavedChanges ? "Dirty" : "Clean";
                DocumentTabs.SelectedIndex = DocumentTabs.TabPages.Count - 1;
                TabChanged(DocumentTabs, EventArgs.Empty);
            }

            return Task.CompletedTask;
        }

        private void DocumentSwitch(IDocument document)
        {
            var tab = DocumentTabs.TabPages.OfType<TabPage>().FirstOrDefault(x => x.Tag == document);
            if (tab == null) return;

            var idx = DocumentTabs.TabPages.IndexOf(tab);
            if (idx < 0) return;

            DocumentTabs.SelectedIndex = idx;
            TabChanged(DocumentTabs, EventArgs.Empty);

        }

        private Task CloseDocument(IDocument document)
        {
            lock (_lock)
            {
                if (!_documents.Contains(document)) return Task.CompletedTask;

                _documents.Remove(document);
                var page = DocumentTabs.TabPages.OfType<TabPage>().FirstOrDefault(x => x.Tag == document);
                if (page != null) DocumentTabs.TabPages.Remove(page);
                TabChanged(DocumentTabs, EventArgs.Empty);
            }

            return Task.CompletedTask;
        }

        private Task DocumentChanged(IDocument document)
        {
            var page = DocumentTabs.TabPages.OfType<TabPage>().FirstOrDefault(x => x.Tag == document);
            if (page != null && document != null)
            {
                page.Text = document.Name;
                page.ImageKey = document.HasUnsavedChanges ? "Dirty" : "Clean";
            }

            return Task.CompletedTask;
        }

        private async Task DocumentCloseAndPrompt(IDocument doc)
        {
            if (doc.HasUnsavedChanges)
            {
                var r = MessageBox.Show(this, String.Format(SaveChangesToFile, doc.Name), UnsavedChanges, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (r == DialogResult.Cancel)
                {
                    return;
                }

                if (r == DialogResult.Yes)
                {
                    if (!await SaveDocument(doc)) return;
                }
            }
            await Oy.Publish("Document:RequestClose", doc);
        }

        private async Task<bool> SaveDocument(IDocument doc)
        {
            var filename = doc.FileName;

            if (String.IsNullOrWhiteSpace(filename) || !Directory.Exists(Path.GetDirectoryName(filename)))
            {
                var loaders = _loaders.Select(x => x.Value).Where(x => x.CanSave(doc)).ToList();
                var filter = loaders.SelectMany(x => x.SupportedFileExtensions).Select(x => x.Description + "|" + String.Join(";", x.Extensions.Select(ex => "*" + ex))).ToList();

                using (var sfd = new SaveFileDialog {Filter = String.Join("|", filter)})
                {
                    if (sfd.ShowDialog() != DialogResult.OK) return false;
                    filename = sfd.FileName;
                }
            }

            await Oy.Publish("Command:Run", new CommandMessage("Internal:SaveDocument", new
            {
                Document = doc,
                Path = filename
            }));
            return true;
        }

        private Task OpenCommandBox(string obj)
        {
            var cb = new CommandBox();
            cb.Location = new Point(Location.X + (Size.Width - cb.Width) / 2, Location.Y + (Size.Height - cb.Height) / 4);
            cb.StartPosition = FormStartPosition.Manual;
            cb.Show(this);
            return Task.CompletedTask;
        }

        // Form events

        private void TabChanged(object sender, EventArgs e)
        {
            if (DocumentTabs.SelectedTab != null)
            {
                if (DocumentTabs.SelectedTab.Tag is IDocument doc)
                {
                    var currentControl = DocumentContainer.Controls.OfType<Control>().FirstOrDefault();
                    if (currentControl != doc.Control)
                    {
                        DocumentContainer.Controls.Clear();
                        DocumentContainer.Controls.Add((Control) doc.Control);
                        DocumentContainer.Controls[0].Dock = DockStyle.Fill;
                    }
                    Oy.Publish("Document:Activated", doc);
                    Oy.Publish("Context:Add", new ContextInfo("ActiveDocument", doc));

                    Text = Title + @" - " + doc.Name;
                }
                else
                {
                    Oy.Publish<IDocument>("Document:Activated", null);
                    Oy.Publish("Context:Remove", new ContextInfo("ActiveDocument"));
                    Text = Title;
                }
            }
            else
            {
                DocumentContainer.Controls.Clear();
                Oy.Publish<IDocument>("Document:Activated", null);
                Oy.Publish("Context:Remove", new ContextInfo("ActiveDocument"));
                Text = Title;
            }
        }

        private void RequestClose(object sender, int index)
        {
            if (DocumentTabs.TabPages[index].Tag is IDocument doc)
            {
                Oy.Publish("Command:Run", new CommandMessage("Internal:CloseDocument", new
                {
                    Document = doc
                }));
            }
        }

        // settings
        private List<DocumentPointer> _openDocuments;

        string ISettingsContainer.Name => "Sledge.Shell";

        public IEnumerable<SettingKey> GetKeys()
        {
            yield break;
        }

        public void LoadValues(ISettingsStore store)
        {
            _openDocuments = store.Get<DocumentPointer[]>("OpenDocuments")?.ToList();
        }

        public void StoreValues(ISettingsStore store)
        {
            store.Set("OpenDocuments", _openDocuments?.ToArray());
        }

        private class DocumentPointer
        {
            public string Loader { get; set; }
            public string FileName { get; set; }
            public Dictionary<string, string> Metadata { get; set; }

            // ReSharper disable once UnusedMember.Local
            // Don't delete this, it's used for reflection
            public DocumentPointer()
            {
                Metadata = new Dictionary<string, string>();
            }

            public DocumentPointer(SerialisedObject pointer)
            {
                Metadata = new Dictionary<string, string>();
                Loader = pointer.Name;
                foreach (var pp in pointer.Properties) Metadata[pp.Key] = pp.Value;
                FileName = pointer.Get<string>("FileName");
            }

            public SerialisedObject ToPointer()
            {
                var so = new SerialisedObject(Loader);
                foreach (var m in Metadata) so.Set(m.Key, m.Value);
                so.Set("FileName", FileName);
                return so;
            }
        }
    }
}
