using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Editing.Components.Visgroup.Operations;
using Sledge.BspEditor.Modification;
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
    [SidebarComponent(OrderHint = "G")]
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

        private WeakReference<MapDocument> _activeDocument;

        public VisgroupSidebarPanel()
        {
            InitializeComponent();
            //Mediator.Subscribe(EditorMediator.DocumentActivated, this);
            //Mediator.Subscribe(EditorMediator.DocumentAllClosed, this);
            //Mediator.Subscribe(EditorMediator.VisgroupsChanged, this);
            //Mediator.Subscribe(EditorMediator.VisgroupVisibilityChanged, this);
            CreateHandle();
        }

        public bool IsInContext(IContext context)
        {
            return context.TryGet("ActiveDocument", out MapDocument _);
        }

        public int? SelectedVisgroup => VisgroupPanel.GetSelectedVisgroup();


        private async Task DocumentActivated(IDocument doc)
        {
            var md = doc as MapDocument;

            _activeDocument = new WeakReference<MapDocument>(md);
            VisgroupPanel.Update(md);
        }

        private async Task DocumentChanged(Change change)
        {
            if (_activeDocument.TryGetTarget(out MapDocument t) && change.Document == t && change.HasDataChanges)
            {
                if (change.AffectedData.Any(x => x is Primitives.MapData.Visgroup))
                {
                    VisgroupPanel.Update(change.Document);
                }
            }
        }
        
        private void VisgroupVisibilityChanged(int visgroupId)
        {
            //var doc = DocumentManager.CurrentDocument;
            //if (doc == null) return;
            //
            //// Update the group
            //var visItems = GetVisgroupItems(visgroupId, doc);
            //SetCheckState(visgroupId, visItems);
            //
            //// Update any other visgroups those objects are in
            //var otherGroups = visItems.SelectMany(x => x.GetVisgroups(true)).Distinct().Where(x => x != visgroupId);
            //foreach (var oid in otherGroups)
            //{
            //    SetCheckState(oid, GetVisgroupItems(oid, doc));
            //}
        }

        //private void SetCheckState(int visgroupId, ICollection<IMapObject> visItems)
        //{
        //    var numHidden = visItems.Count(x => x.IsVisgroupHidden);
        //
        //    CheckState state;
        //    if (numHidden == visItems.Count) state = CheckState.Unchecked; // All hidden
        //    else if (numHidden > 0) state = CheckState.Indeterminate; // Some hidden
        //    else state = CheckState.Checked; // None hidden
        //
        //    VisgroupPanel.SetCheckState(visgroupId, state);
        //}

        //private static List<MapObject> GetVisgroupItems(int visgroupId, MapDocument doc)
        //{
        //    var visItems = doc.Map.WorldSpawn.Find(x => x.IsInVisgroup(visgroupId, true), true);
        //    return visItems;
        //}

        private void SelectButtonClicked(object sender, EventArgs e)
        {
            var sv = SelectedVisgroup;
            // if (sv.HasValue) Mediator.Publish(EditorMediator.VisgroupSelect, sv.Value);
        }

        private void EditButtonClicked(object sender, EventArgs e)
        {

        }

        private void ShowAllButtonClicked(object sender, EventArgs e)
        {

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

        public void Clear()
        {
            VisgroupPanel.Clear();
        }

        private void VisgroupSelected(object sender)
        {

        }
    }
}
