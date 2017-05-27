using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Editing.Components.Visgroup.Operations;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.Operations.Selection;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Documents;
using Sledge.Common.Shell.Hooks;
using Sledge.Common.Translations;
using Sledge.Shell;

namespace Sledge.BspEditor.Editing.Components.Visgroup
{
    [AutoTranslate]
    [Export(typeof(ISidebarComponent))]
    [Export(typeof(IInitialiseHook))]
    [OrderHint("G")]
    public partial class VisgroupSidebarPanel : UserControl, ISidebarComponent, IInitialiseHook
    {
        public Task OnInitialise()
        {
            Oy.Subscribe<IDocument>("Document:Activated", DocumentActivated);
            Oy.Subscribe<Change>("MapDocument:Changed", DocumentChanged);
            return Task.FromResult(0);
        }

        public string Title { get; set; } = "Visgroups";
        public object Control => this;

        public string EditButton { set { this.Invoke(() => { btnEdit.Text = value; }); } }
        public string SelectButton { set { this.Invoke(() => { btnSelect.Text = value; }); } }
        public string ShowAllButton { set { this.Invoke(() => { btnShowAll.Text = value; }); } }
        public string NewButton { set { this.Invoke(() => { btnNew.Text = value; }); } }
        
        private WeakReference<MapDocument> _activeDocument = new WeakReference<MapDocument>(null);

        public VisgroupSidebarPanel()
        {
            InitializeComponent();
            CreateHandle();
        }

        public bool IsInContext(IContext context)
        {
            return context.TryGet("ActiveDocument", out MapDocument _);
        }

        private Primitives.MapData.Visgroup SelectedVisgroup
        {
            get
            {
                var sel = VisgroupPanel.GetSelectedVisgroup();
                if (!sel.HasValue) return null;
                if (!_activeDocument.TryGetTarget(out MapDocument md)) return null;
                return md.Map.Data.Get<Primitives.MapData.Visgroup>().FirstOrDefault(x => x.ID == sel);
            }
        }


        private async Task DocumentActivated(IDocument doc)
        {
            var md = doc as MapDocument;

            _activeDocument = new WeakReference<MapDocument>(md);
            this.Invoke(() =>
            {
                VisgroupPanel.Update(md);
            });
        }

        private async Task DocumentChanged(Change change)
        {
            if (_activeDocument.TryGetTarget(out MapDocument t) && change.Document == t && change.HasDataChanges)
            {
                if (change.AffectedData.Any(x => x is Primitives.MapData.Visgroup))
                {
                    this.Invoke(() =>
                    {
                        VisgroupPanel.Update(change.Document);
                    });
                }
            }
        }

        private void SelectButtonClicked(object sender, EventArgs e)
        {
            var sv = SelectedVisgroup;
            if (sv != null && _activeDocument.TryGetTarget(out MapDocument md))
            {
                MapDocumentOperation.Perform(md, new Transaction(new Deselect(md.Selection), new Select(sv.Objects)));
            }
        }

        private void EditButtonClicked(object sender, EventArgs e)
        {
            Oy.Publish("Command:Run", new CommandMessage("BspEditor:Map:Visgroups"));
        }

        private void ShowAllButtonClicked(object sender, EventArgs e)
        {
            if (_activeDocument.TryGetTarget(out MapDocument md))
            {
                var tns = new Transaction();
                foreach (var visgroup in md.Map.Data.Get<Primitives.MapData.Visgroup>())
                {
                    tns.Add(new SetVisgroupVisibility(visgroup.ID, false));
                }
                MapDocumentOperation.Perform(md, tns);
            }
        }

        private void NewButtonClicked(object sender, EventArgs e)
        {

        }

        private void VisgroupToggled(object sender, long visgroupId, CheckState state)
        {
            if (state == CheckState.Indeterminate) return;
            var visible = state == CheckState.Checked;
            if (_activeDocument.TryGetTarget(out MapDocument md))
            {
                MapDocumentOperation.Perform(md, new SetVisgroupVisibility(visgroupId, !visible));
            }

        }

        private void VisgroupSelected(object sender, long? visgroupId)
        {

        }
    }
}
