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
    [Export(typeof(IDialog))]
    [Export(typeof(IInitialiseHook))]
    [AutoTranslate]
    public partial class ObjectPropertiesDialog : Form, IInitialiseHook, IDialog
    {
        [Import("Shell", typeof(Form))] private Lazy<Form> _parent;
        [ImportMany] private IEnumerable<Lazy<IObjectPropertyEditorTab>> _tabs;
        private List<Subscription> _subscriptions;

        public Task OnInitialise()
        {
            foreach (var tab in _tabs.Select(x => x.Value).OrderBy(x => x.OrderHint))
            {
                var page = new TabPage(tab.Name) {Tag = tab};
                tab.Control.Dock = DockStyle.Fill;
                page.Controls.Add(tab.Control);
                tabPanel.TabPages.Add(page);
            }
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

        public ObjectPropertiesDialog()
        {
            InitializeComponent();
            CreateHandle();
        }

        private void UpdateTabVisibility(IContext context)
        {
            tabPanel.SuspendLayout();
            foreach (var tp in tabPanel.TabPages.OfType<TabPage>())
            {
                var tab = (IObjectPropertyEditorTab) tp.Tag;
                var inContext = tab.IsInContext(context);
                tp.Visible = inContext;
                tp.Text = tab.Name;
            }
            tabPanel.ResumeLayout();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            Oy.Publish("Context:Remove", new ContextInfo("BspEditor:ObjectProperties"));
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
                    DocumentActivated(doc);
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

        private async Task DocumentActivated(MapDocument doc)
        {
            var list = doc?.Selection.ToList() ?? new List<IMapObject>();
            foreach (var tab in _tabs)
            {
                await tab.Value.SetObjects(doc, list);
            }
        }

        private Task DocumentChanged(Change change)
        {
            return DocumentActivated(change.Document);
        }

        private void Subscribe()
        {
            if (_subscriptions != null) return;
            _subscriptions = new List<Subscription>
            {
                Oy.Subscribe<Change>("MapDocument:Changed", DocumentChanged),
                Oy.Subscribe<MapDocument>("Document:Activated", DocumentActivated),
                Oy.Subscribe<MapDocument>("MapDocument:SelectionChanged", DocumentActivated)
            };
        }

        private void Unsubscribe()
        {
            if (_subscriptions == null) return;
            _subscriptions.ForEach(x => x.Dispose());
            _subscriptions = null;
        }
    }
}
