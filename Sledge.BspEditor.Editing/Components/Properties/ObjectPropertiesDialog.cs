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
using Sledge.Shell.Forms;

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
    public sealed partial class ObjectPropertiesDialog : BaseForm, IInitialiseHook, IDialog
    {
        private readonly Lazy<Form> _parent;
        private readonly IEnumerable<Lazy<IObjectPropertyEditorTab>> _tabs;
        private readonly IContext _context;

        private List<Subscription> _subscriptions;
        private Dictionary<IObjectPropertyEditorTab, TabPage> _pages;
        private MapDocument _currentDocument;

        private bool _selectionForced;
        private List<IMapObject> _selectedObjects;

        public Task OnInitialise()
        {
            _pages = new Dictionary<IObjectPropertyEditorTab, TabPage>();
            this.InvokeLater(() =>
            {
                foreach (var tab in _tabs.Select(x => x.Value).OrderBy(x => x.OrderHint))
                {
                    var page = new TabPage(tab.Name) {Tag = tab};
                    tab.Control.Dock = DockStyle.Fill;
                    page.Controls.Add(tab.Control);
                    _pages[tab] = page;
                }
            });

            Oy.Subscribe<List<IMapObject>>("BspEditor:ObjectProperties:OpenWithSelection", OpenWithSelection);
            return Task.CompletedTask;
        }

        public string Title
        {
            get => Text;
            set => this.InvokeLater(() => Text = value);
        }

        public string Apply
        {
            get => btnApply.Text;
            set => this.InvokeLater(() => btnApply.Text = value);
        }

        public string CloseButton
        {
            get => btnClose.Text;
            set => this.InvokeLater(() => btnClose.Text = value);
        }

        public string ResetUnsavedChanges
        {
            get => btnReset.Text;
            set => this.InvokeLater(() => btnReset.Text = value);
        }

        [ImportingConstructor]
        public ObjectPropertiesDialog(
            [Import("Shell")] Lazy<Form> parent,
            [ImportMany] IEnumerable<Lazy<IObjectPropertyEditorTab>> tabs,
            [Import] Lazy<IContext> context
        )
        {
            _parent = parent;
            _tabs = tabs;
            _context = context.Value;

            InitializeComponent();
            CreateHandle();
        }

        private async Task OpenWithSelection(List<IMapObject> selection)
        {
            if (Visible) await Save();

            _selectedObjects = selection;
            _selectionForced = true;

            await Oy.Publish("Context:Add", new ContextInfo("BspEditor:ObjectProperties"));
        }

        /// <summary>
        /// Update the visibility of all the loaded tabs based on the current selection and context.
        /// </summary>
        private void UpdateTabVisibility(IContext context, List<IMapObject> objects)
        {
            var changed = false;
            tabPanel.SuspendLayout();

            var currentlyVisibleTabs = tabPanel.TabPages.OfType<TabPage>().Select(x => _pages.FirstOrDefault(p => p.Value == x).Key).ToList();
            var newVisibleTabs = _tabs.Where(x => x.Value.IsInContext(context, objects)).OrderBy(x => x.Value.OrderHint).Select(x => x.Value).ToList();

            // Add tabs which aren't visible and should be
            foreach (var add in newVisibleTabs.Except(currentlyVisibleTabs).ToList())
            {
                // Locate the next or previous tab in the visible tab set so we can insert the new tab before/after it
                var prevCv = currentlyVisibleTabs.Where(x => String.Compare(x.OrderHint, add.OrderHint, StringComparison.Ordinal) < 0).OrderByDescending(x => x.OrderHint).FirstOrDefault();
                var nextCv = currentlyVisibleTabs.Where(x => String.Compare(x.OrderHint, add.OrderHint, StringComparison.Ordinal) > 0).OrderBy(x => x.OrderHint).FirstOrDefault();
                var idx = prevCv != null ? tabPanel.TabPages.IndexOf(_pages[prevCv]) + 1
                        : nextCv != null ? tabPanel.TabPages.IndexOf(_pages[nextCv])
                        : 0;

                // Add the tab the the currently visible set for later index testing
                tabPanel.TabPages.Insert(idx, _pages[add]);
                currentlyVisibleTabs.Add(add);
                changed = true;
            }

            // Remove tables which are visible and shouldn't be
            foreach (var rem in currentlyVisibleTabs.Except(newVisibleTabs))
            {
                tabPanel.TabPages.Remove(_pages[rem]);
                changed = true;
            }

            if (changed) tabPanel.SelectedIndex = tabPanel.TabCount > 0 ? 0 : -1;

            tabPanel.ResumeLayout(changed);
        }

        /// <summary>
        /// Don't close the dialog, but check if changes are made and hide it if the check passes
        /// </summary>
        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            Save().ContinueWith(Close);
        }

		protected override void OnMouseEnter(EventArgs e)
		{
            Focus();
            base.OnMouseEnter(e);
        }

        /// <summary>
        /// Conditionally close the dialog. Doesn't perform any change detection.
        /// </summary>
        private void Close(Task<bool> actuallyClose)
        {
            if (actuallyClose.Result) Oy.Publish("Context:Remove", new ContextInfo("BspEditor:ObjectProperties"));
        }

        /// <inheritdoc />
        public bool IsInContext(IContext context)
        {
            return context.HasAny("BspEditor:ObjectProperties");
        }

        /// <inheritdoc />
        public void SetVisible(IContext context, bool visible)
        {
            this.InvokeLater(() =>
            {
                if (visible)
                {
                    var doc = context.Get<MapDocument>("ActiveDocument");

                    #pragma warning disable 4014 // Intentionally unawaited
                    DocumentActivated(doc);
                    #pragma warning restore 4014

                    if (!Visible) Show(_parent.Value);
                    Subscribe();
                }
                else
                {
                    _selectionForced = false;
                    _selectedObjects = null;
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
        /// Saves the current changes (if any)
        /// This will clear all values from all tabs, they'll need to be reset afterwards.
        /// </summary>
        private async Task<bool> Save()
        {
            if (_currentDocument == null) return true;
            var changed = _tabs.Select(x => x.Value).Where(x => x.HasChanges).ToList();
            if (!changed.Any()) return true;

            var list = _selectedObjects;
            var changes = changed.SelectMany(x => x.GetChanges(_currentDocument, list));
            var tsn = new Transaction(changes);

            // Clear all changes from all tabs
            foreach (var t in _tabs) await t.Value.SetObjects(_currentDocument, _selectedObjects);

            await MapDocumentOperation.Perform(_currentDocument, tsn);
            return true;
        }

        /// <summary>
        /// Undoes any pending changes in the form
        /// </summary>
        private Task Reset(Task t = null)
        {
            return UpdateSelection();
        }

        /// <summary>
        /// A different document has been activated, clear the selection and reset to the new document
        /// </summary>
        private async Task DocumentActivated(MapDocument doc)
        {
            await Save();

            _currentDocument = doc;

            // If the selection is forced, reset it
            if (_selectionForced && _selectedObjects != null && !_selectedObjects.All(x => ReferenceEquals(doc.Map.Root.FindByID(x.ID), x)))
            {
                // Special case for root object
                if (_selectedObjects.Count == 1 && _selectedObjects[0] is Root) _selectedObjects[0] = _currentDocument.Map.Root;
                // Otherwise clear the forced selection
                else _selectionForced = false;
            }

            await UpdateSelection();
        }

        private async Task SelectionChanged(MapDocument document)
        {
            await Save();
        }

        /// <summary>
        /// The active document has been modified, ensure the selection is still correct.
        /// </summary>
        private async Task DocumentChanged(Change change)
        {
            // If the selection is forced, make sure any deleted objects are removed from the selection
            _selectedObjects?.RemoveAll(x => change.Removed.Contains(x));

            await UpdateSelection();
        }

        /// <summary>
        /// Update all tabs with the new selection.
        /// </summary>
        private async Task UpdateSelection()
        {
            _selectedObjects = _selectionForced
                ? _selectedObjects
                : _currentDocument?.Selection.GetSelectedParents().ToList();
            
            foreach (var tab in _tabs)
            {
                await tab.Value.SetObjects(_currentDocument, _selectedObjects);
            }

            this.InvokeLater(() => UpdateTabVisibility(_context, _selectedObjects));
        }

        private void ApplyClicked(object sender, EventArgs e) => Save().ContinueWith(Reset);
        private void CancelClicked(object sender, EventArgs e) => Close();
        private void ResetClicked(object sender, EventArgs e) => Reset();
    }
}
