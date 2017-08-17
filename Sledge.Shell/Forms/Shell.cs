using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Documents;
using Sledge.Common.Translations;
using Sledge.Shell.Controls;

namespace Sledge.Shell.Forms
{
    /// <summary>
    /// The application's base window
    /// </summary>
    [Export]
    [Export("Shell", typeof(Form))]
    [AutoTranslate]
    internal partial class Shell : BaseForm
    {
        private readonly List<IDocument> _documents;
        private readonly object _lock = new object();

        [Import] private Bootstrapper _bootstrapper;

        public string Title { get; set; } = "Sledge Shell";

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
            
            Oy.Subscribe<IContext>("Context:Changed", ContextChanged);

            Oy.Subscribe<IDocument>("Document:Opened", OpenDocument);
            Oy.Subscribe<IDocument>("Document:Closed", CloseDocument);
            Oy.Subscribe<IDocument>("Document:NameChanged", DocumentNameChanged);

            Oy.Subscribe<string>("Shell:OpenCommandBox", OpenCommandBox);
        }

        protected override void OnLoad(EventArgs e)
        {
            // Bootstrap the shell
            _bootstrapper.Startup().ContinueWith(t => _bootstrapper.Initialise());

            // Set up bootstrapping for shutdown
            Closing += DoClosing;

            Text = Title;
        }

        private async void DoClosing(object sender, CancelEventArgs e)
        {
            // Try to close all the open documents
            foreach (var doc in _documents.ToArray())
            {
                await Oy.Publish("Document:RequestClose", doc);
                if (_documents.Contains(doc))
                {
                    e.Cancel = true;
                    return;
                }
            }

            // Close anything else
            if (!await _bootstrapper.ShuttingDown())
            {
                e.Cancel = true;
                return;
            }

            // Unsubscribe the event (no infinite loops!) and close for good
            Closing -= DoClosing;
            Enabled = false;
            e.Cancel = true;
            await _bootstrapper.Shutdown();
            Close();
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
                DocumentTabs.TabPages.Add(new TabPage { Text = document.Name, Tag = document });
                TabChanged(DocumentTabs, EventArgs.Empty);
            }
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

        private async Task DocumentNameChanged(IDocument document)
        {
            var page = DocumentTabs.TabPages.OfType<TabPage>().FirstOrDefault(x => x.Tag == document);
            if (page != null && document != null) page.Text = document.Name;
        }

        private async Task ContextChanged(IContext context)
        {

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
                }
                else
                {
                    Oy.Publish<IDocument>("Document:Activated", null);
                    Oy.Publish("Context:Remove", new ContextInfo("ActiveDocument"));
                }
            }
            else
            {
                DocumentContainer.Controls.Clear();
                Oy.Publish<IDocument>("Document:Activated", null);
                Oy.Publish("Context:Remove", new ContextInfo("ActiveDocument"));
            }
        }

        private void RequestClose(object sender, int index)
        {
            var doc = DocumentTabs.TabPages[index].Tag as IDocument;
            if (doc != null)
            {
                Oy.Publish("Document:RequestClose", doc);
            }
        }
    }
}
