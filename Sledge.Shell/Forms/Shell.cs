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

        [Import] private Bootstrapper _bootstrapper;
        [ImportMany] private IEnumerable<Lazy<IDocumentLoader>> _loaders;
        [Import] private DocumentRegister _documentRegister;
        [Import] private ITranslationStringProvider _translation;

        public string Title { get; set; } = "Sledge Shell";

        public string UnsavedChanges { get; set; } = "Unsaved changes in file";
        public string SaveChangesToFile { get; set; } = "Save changes to {0}?";

        internal ToolStripPanel ToolStrip => ToolStripContainer.TopToolStripPanel;

        public Shell()
        {
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
        }

        private async Task ShowExceptionDialog(Exception obj)
        {
            this.InvokeLater(() => {
                var ed = new ExceptionWindow(obj);
                ed.ShowDialog(this);
            });
        }

        private async Task InstanceOpened(List<string> args)
        {
            foreach (var arg in args.Skip(1))
            {
                Sledge.Common.Logging.Log.Debug(nameof(Shell), $"Command line: `{arg}`");
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
            _bootstrapper.UIStartup();
            _bootstrapper.Startup()
                .ContinueWith(_ => _bootstrapper.Initialise())
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
            foreach (var doc in _documents.ToArray())
            {
                await Oy.Publish("Document:RequestClose", doc);
                if (_documents.Contains(doc))
                {
                    Closing += DoClosing;
                    Closing -= CancelClose;
                    return;
                }
            }

            // Close anything else
            if (!await _bootstrapper.ShuttingDown())
            {
                Closing += DoClosing;
                Closing -= CancelClose;
                return;
            }

            // Unsubscribe the event (no infinite loops!) and close for good
            Closing -= CancelClose;
            Enabled = false;
            this.InvokeSync(() => { 
                _bootstrapper.UIShutdown();
            });
            await _bootstrapper.Shutdown();
            Close();
        }

        private void SaveOpenDocuments()
        {
            _openDocuments = new List<DocumentPointer>();
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

        private async Task<bool> SaveUnsavedDocuments()
        {
            var unsaved = _documentRegister.OpenDocuments.Where(x => x.HasUnsavedChanges).ToList();
            if (!unsaved.Any()) return true; // nothing unsaved

            DialogResult result;

            using (var d = new SaveChangesForm(unsaved))
            {
                d.Translate(_translation);
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

        private async Task OpenDocument(IDocument document)
        {
            lock (_lock)
            {
                if (_documents.Contains(document)) return;
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

        private async Task CloseDocument(IDocument document)
        {
            lock (_lock)
            {
                if (!_documents.Contains(document)) return;
                _documents.Remove(document);
                var page = DocumentTabs.TabPages.OfType<TabPage>().FirstOrDefault(x => x.Tag == document);
                if (page != null) DocumentTabs.TabPages.Remove(page);
                TabChanged(DocumentTabs, EventArgs.Empty);
            }
        }

        private async Task DocumentChanged(IDocument document)
        {
            var page = DocumentTabs.TabPages.OfType<TabPage>().FirstOrDefault(x => x.Tag == document);
            if (page != null && document != null)
            {
                page.Text = document.Name;
                page.ImageKey = document.HasUnsavedChanges ? "Dirty" : "Clean";
            }
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

        private async Task OpenCommandBox(string obj)
        {
            var cb = new CommandBox();
            cb.Location = new Point(Location.X + (Size.Width - cb.Width) / 2, Location.Y + (Size.Height - cb.Height) / 4);
            cb.StartPosition = FormStartPosition.Manual;
            cb.Show(this);
        }

        // Form events

        private void TabChanged(object sender, EventArgs e)
        {
            if (DocumentTabs.SelectedTab != null)
            {
                var doc = DocumentTabs.SelectedTab.Tag as IDocument;
                if (doc != null)
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
            var doc = DocumentTabs.TabPages[index].Tag as IDocument;
            if (doc != null)
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
