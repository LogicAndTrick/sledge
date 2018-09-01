using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.Operations;
using Sledge.BspEditor.Modification.Operations.Selection;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
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
        [Import] private ITranslationStringProvider _translation;

        public Task OnInitialise()
        {
            Oy.Subscribe<IDocument>("Document:Activated", DocumentActivated);
            Oy.Subscribe<Change>("MapDocument:Changed", DocumentChanged);
            return Task.FromResult(0);
        }

        public string Title { get; set; } = "Visgroups";
        public object Control => this;

        public string EditButton { set { this.InvokeLater(() => { btnEdit.Text = value; }); } }
        public string SelectButton { set { this.InvokeLater(() => { btnSelect.Text = value; }); } }
        public string ShowAllButton { set { this.InvokeLater(() => { btnShowAll.Text = value; }); } }
        public string NewButton { set { this.InvokeLater(() => { btnNew.Text = value; }); } }
        public string AutoVisgroups { get; set; }
        
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
        
        private async Task DocumentActivated(IDocument doc)
        {
            var md = doc as MapDocument;

            _activeDocument = new WeakReference<MapDocument>(md);
            Update(md);
        }

        private async Task DocumentChanged(Change change)
        {
            if (_activeDocument.TryGetTarget(out MapDocument t) && change.Document == t)
            {
                if (change.HasObjectChanges || IsVisgroupDataChange(change))
                {
                    Update(change.Document);
                }
            }
        }

        private static bool IsVisgroupDataChange(Change change)
        {
            return change.HasDataChanges && change.AffectedData.Any(x => x is AutomaticVisgroup || x is Primitives.MapData.Visgroup);
        }

        private void Update(MapDocument document)
        {
            Task.Factory.StartNew(() =>
            {
                if (document == null)
                {
                    this.InvokeLater(() => VisgroupPanel.Clear());
                }
                else
                {
                    var tree = GetItemHierarchies(document);
                    this.InvokeLater(() => VisgroupPanel.Update(tree));
                }
            });
        }

        private List<VisgroupItem> GetItemHierarchies(MapDocument document)
        {
            var list = new List<VisgroupItem>();

            // add user visgroups
            var visgroups = document.Map.Data.Get<Primitives.MapData.Visgroup>().ToList();
            foreach (var v in visgroups)
            {
                list.Add(new VisgroupItem(v.Name)
                {
                    CheckState = GetVisibilityCheckState(v.Objects),
                    Colour = v.Colour,
                    Tag = v
                });
            }

            var auto = new VisgroupItem(AutoVisgroups)
            {
                Disabled = true
            };
            list.Insert(0, auto);

            // add auto visgroups
            var autoVisgroups = document.Map.Data.Get<AutomaticVisgroup>().ToList();
            var parents = new Dictionary<string, VisgroupItem> {{"", auto}};
            foreach (var av in autoVisgroups.OrderBy(x => x.Path.Length))
            {
                VisgroupItem parent = auto;
                if (!parents.ContainsKey(av.Path))
                {
                    var path = new List<string>();
                    foreach (var spl in av.Path.Split('/'))
                    {
                        path.Add(spl);
                        var seg = String.Join("/", path);
                        if (!parents.ContainsKey(seg))
                        {
                            var group = new VisgroupItem(_translation.GetString(spl))
                            {
                                Parent = parent,
                                Disabled = true
                            };
                            list.Add(group);
                            parents[seg] = group;
                        }
                        parent = parents[seg];
                    }
                }
                else
                {
                    parent = parents[av.Path];
                }
                list.Add(new VisgroupItem(_translation.GetString(av.Key))
                {
                    CheckState = GetVisibilityCheckState(av.Objects),
                    Tag = av,
                    Parent = parent
                });
            }

            for (var i = list.Count - 1; i >= 0; i--)
            {
                var v = list[i];
                if (v.Tag != null) continue;

                var children = list.Where(x => x.Parent == v).ToList();
                if (children.All(x => x.CheckState == CheckState.Checked)) v.CheckState = CheckState.Checked;
                else if (children.All(x => x.CheckState == CheckState.Unchecked)) v.CheckState = CheckState.Unchecked;
                else v.CheckState = CheckState.Indeterminate;
            }

            return list;
        }

        private CheckState GetVisibilityCheckState(IEnumerable<IMapObject> objects)
        {
            var bools = objects.Select(x => x.Data.GetOne<VisgroupHidden>()?.IsHidden ?? false);
            return GetCheckState(bools);
        }

        private CheckState GetCheckState(IEnumerable<bool> bools)
        {
            var a = bools.Distinct().ToArray();
            if (a.Length == 0) return CheckState.Checked;
            if (a.Length == 1) return a[0] ? CheckState.Unchecked : CheckState.Checked;
            return CheckState.Indeterminate;
        }

        private IEnumerable<IMapObject> GetVisgroupObjects(VisgroupItem item)
        {
            if (item?.Tag is Primitives.MapData.Visgroup v) return v.Objects;
            if (item?.Tag is AutomaticVisgroup av) return av.Objects;

            var children = VisgroupPanel.GetAllItems().Where(x => x.Parent == item).SelectMany(GetVisgroupObjects);
            return new HashSet<IMapObject>(children);
        }

        private void SelectButtonClicked(object sender, EventArgs e)
        {
            var sv = VisgroupPanel.SelectedVisgroup;
            if (sv != null && _activeDocument.TryGetTarget(out MapDocument md))
            {
                MapDocumentOperation.Perform(md, new Transaction(new Deselect(md.Selection), new Select(GetVisgroupObjects(sv))));
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
                var objects = md.Map.Root.Find(x => x.Data.GetOne<VisgroupHidden>()?.IsHidden == true, true).ToList();
                if (objects.Any())
                {
                    MapDocumentOperation.Perform(md, new TrivialOperation(
                        d => objects.ToList().ForEach(x => x.Data.Replace(new VisgroupHidden(false))),
                        c => c.AddRange(objects)
                    ));
                }
            }
        }

        private void NewButtonClicked(object sender, EventArgs e)
        {

        }

        private void VisgroupToggled(object sender, VisgroupItem visgroup, CheckState state)
        {
            if (state == CheckState.Indeterminate) return;
            var visible = state == CheckState.Checked;
            var objects = GetVisgroupObjects(visgroup).SelectMany(x => x.FindAll()).ToList();
            if (objects.Any() && _activeDocument.TryGetTarget(out MapDocument md))
            {
                MapDocumentOperation.Perform(md, new TrivialOperation(
                    d => objects.ForEach(x => x.Data.Replace(new VisgroupHidden(!visible))),
                    c => c.AddRange(objects)
                ));
            }

        }

        private void VisgroupSelected(object sender, VisgroupItem visgroup)
        {

        }
    }
}
