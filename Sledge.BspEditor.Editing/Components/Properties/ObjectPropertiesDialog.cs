using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Hooks;
using Sledge.Common.Translations;
using Sledge.Shell;

namespace Sledge.BspEditor.Editing.Components.Properties
{
    /// <summary>
    /// This is the main way to edit properties of an object, including
    /// entity data, flags, visgroups, outputs, and anything else.
    /// 
    /// Each tab is a standalone panel that has its own state and context.
    /// If any tab is changed, they are all saved together when the user
    /// saves the dialog.
    /// </summary>
    [Export(typeof(IDialog))]
    [Export(typeof(IInitialiseHook))]
    [AutoTranslate]
    public sealed partial class ObjectPropertiesDialog : Form, IInitialiseHook, IDialog
    {
        [Import("Shell", typeof(Form))] private Lazy<Form> _parent;
        [ImportMany] private IEnumerable<Lazy<IObjectPropertyEditorTab>> _tabs;
        [Import] private IContext _context;

        private List<Subscription> _subscriptions;
        private Dictionary<IObjectPropertyEditorTab, TabPage> _pages;
        private MapDocument _currentDocument;

        public Task OnInitialise()
        {
            _pages = new Dictionary<IObjectPropertyEditorTab, TabPage>();
            this.Invoke(() =>
            {
                foreach (var tab in _tabs.Select(x => x.Value).OrderBy(x => x.OrderHint))
                {
                    var page = new TabPage(tab.Name) {Tag = tab};
                    tab.Control.Dock = DockStyle.Fill;
                    page.Controls.Add(tab.Control);
                    _pages[tab] = page;
                }
            });
            return Task.FromResult(0);
        }

        public string Apply
        {
            get => btnApply.Text;
            set => this.Invoke(() => btnApply.Text = value);
        }

        public string OK
        {
            get => btnOk.Text;
            set => this.Invoke(() => btnOk.Text = value);
        }

        public string Cancel
        {
            get => btnCancel.Text;
            set => this.Invoke(() => btnCancel.Text = value);
        }

        public string ResetUnsavedChanges
        {
            get => btnReset.Text;
            set => this.Invoke(() => btnReset.Text = value);
        }

        public string UnsavedChanges { get; set; }
        public string DoYouWantToSaveFirst { get; set; }

        public ObjectPropertiesDialog()
        {
            InitializeComponent();
            CreateHandle();
        }

        /// <summary>
        /// Update the visibility of all the loaded tabs based on the current selection and context.
        /// </summary>
        private void UpdateTabVisibility(IContext context)
        {
            // todo: to avoid UI flashing, only add/remove tabs when they need to, rather than always
            tabPanel.SuspendLayout();
            var sel = tabPanel.SelectedIndex < tabPanel.TabCount ? tabPanel.SelectedTab : null;
            tabPanel.TabPages.Clear();
            foreach (var tp in _tabs.OrderBy(x => x.Value.OrderHint))
            {
                var tab = tp.Value;
                var inContext = tab.IsInContext(context);
                var page = _pages[tab];
                if (inContext)
                {
                    page.Text = tab.Name;
                    tabPanel.TabPages.Add(page);
                }
                else if (sel == page)
                {
                    sel = null;
                }
            }
            if (sel != null) tabPanel.SelectedTab = sel;
            tabPanel.ResumeLayout(true);
        }

        /// <summary>
        /// Don't close the dialog, but check if changes are made and hide it if the check passes
        /// </summary>
        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            ConfirmIfChanged().ContinueWith(Close);
        }

        /// <summary>
        /// Conditionally close the dialog. Doesn't perform any change detection.
        /// </summary>
        private void Close(Task<bool> actuallyClose)
        {
            if (actuallyClose.Result) Oy.Publish("Context:Remove", new ContextInfo("BspEditor:ObjectProperties"));
        }

        public bool IsInContext(IContext context)
        {
            return context.HasAny("BspEditor:ObjectProperties");
        }

        public void SetVisible(IContext context, bool visible)
        {
            this.Invoke(() =>
            {
                if (visible)
                {
                    UpdateTabVisibility(context);
                    var doc = context.Get<MapDocument>("ActiveDocument");

                    #pragma warning disable 4014 // Intentionally unawaited
                    DocumentActivated(doc);
                    #pragma warning restore 4014

                    Show(_parent.Value);
                    Subscribe();
                }
                else
                {
                    Hide();
                    Unsubscribe();
                }
            });
        }

        private void Subscribe()
        {
            if (_subscriptions != null) return;
            _subscriptions = new List<Subscription>
            {
                Oy.Subscribe<Change>("MapDocument:Changed", DocumentChanged),
                Oy.Subscribe<MapDocument>("Document:Activated", DocumentActivated),
                Oy.Subscribe<MapDocument>("MapDocument:SelectionChanged", SelectionChanged)
            };
        }

        private void Unsubscribe()
        {
            if (_subscriptions == null) return;
            _subscriptions.ForEach(x => x.Dispose());
            _subscriptions = null;
        }

        /// <summary>
        /// Checks for changes and prompts the user if they want to save.
        /// </summary>
        private Task<bool> ConfirmIfChanged()
        {
            if (_currentDocument == null) return Task.FromResult(true);
            if (!_tabs.Any(x => x.Value.HasChanges)) return Task.FromResult(true);
            var result = MessageBox.Show(DoYouWantToSaveFirst, UnsavedChanges, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);

            if (result == DialogResult.Yes) return Save(); // Save first, then close
            if (result == DialogResult.No) return Task.FromResult(true); // Don't save, and close
            return Task.FromResult(false); // Don't save, don't close
        }

        /// <summary>
        /// Saves the current changes (if any)
        /// </summary>
        private async Task<bool> Save()
        {
            if (_currentDocument == null) return true;
            var changed = _tabs.Select(x => x.Value).Where(x => x.HasChanges).ToList();
            if (!changed.Any()) return true;

            var changes = changed.SelectMany(x => x.GetChanges(_currentDocument, _currentDocument.Selection.GetSelectedParents().ToList()));
            var tsn = new Transaction(changes);

            await MapDocumentOperation.Perform(_currentDocument, tsn);
            return true;
        }

        /// <summary>
        /// Undoes any pending changes in the form
        /// </summary>
        private Task Reset(Task t = null)
        {
            return DocumentActivated(_currentDocument);
        }

        private async Task DocumentActivated(MapDocument doc)
        {
            _currentDocument = doc;
            var list = doc?.Selection.GetSelectedParents().ToList() ?? new List<IMapObject>();
            foreach (var tab in _tabs)
            {
                await tab.Value.SetObjects(doc, list);
            }
        }

        private async Task SelectionChanged(MapDocument doc)
        {
            this.Invoke(() => UpdateTabVisibility(_context));
            await DocumentActivated(doc);
        }

        private Task DocumentChanged(Change change)
        {
            return DocumentActivated(change.Document);
        }

        private void ApplyClicked(object sender, EventArgs e) => Save().ContinueWith(Reset);
        private void OkClicked(object sender, EventArgs e) => Save().ContinueWith(Close);
        private void CancelClicked(object sender, EventArgs e) => ConfirmIfChanged().ContinueWith(Close);
        private void ResetClicked(object sender, EventArgs e) => Reset();
    }
}
