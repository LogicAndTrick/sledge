using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.Operations.Selection;
using Sledge.BspEditor.Modification.Operations.Tree;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Context;
using Sledge.Common.Translations;
using Sledge.Shell;

namespace Sledge.BspEditor.Editing.Components
{
    [Export(typeof(IDialog))]
    [AutoTranslate]
    public partial class EntityReportDialog : Form, IDialog, IManualTranslate
    {
        [Import("Shell", typeof(Form))] private Lazy<Form> _parent;
        [Import] private IContext _context;

        private class ColumnComparer : IComparer
        {
            public int Column { get; set; }
            public SortOrder SortOrder { get; set; }

            public ColumnComparer(int column)
            {
                Column = column;
                SortOrder = SortOrder.Ascending;
            }

            public int Compare(object x, object y)
            {
                var i1 = (ListViewItem)x;
                var i2 = (ListViewItem)y;
                var compare = String.CompareOrdinal(i1.SubItems[Column].Text, i2.SubItems[Column].Text);
                return SortOrder == SortOrder.Descending ? -compare : compare;
            }
        }

        private readonly ColumnComparer _sorter;

        private List<Subscription> _subscriptions;

        public EntityReportDialog()
        {
            InitializeComponent();

            _sorter = new ColumnComparer(0);
            EntityList.ListViewItemSorter = _sorter;
        }

        public void Translate(ITranslationStringProvider strings)
        {
            CreateHandle();
            var prefix = GetType().FullName;
            this.InvokeLater(() =>
            {
                Text = strings.GetString(prefix, "Title");
                ClassNameHeader.Text = strings.GetString(prefix, "ClassHeader");
                EntityNameHeader.Text = strings.GetString(prefix, "NameHeader");
                GoToButton.Text = strings.GetString(prefix, "GoTo");
                DeleteButton.Text = strings.GetString(prefix, "Delete");
                PropertiesButton.Text = strings.GetString(prefix, "Properties");
                FollowSelection.Text = strings.GetString(prefix, "FollowSelection");
                FilterGroup.Text = strings.GetString(prefix, "Filter");
                TypeAll.Text = strings.GetString(prefix, "ShowAll");
                TypePoint.Text = strings.GetString(prefix, "ShowPoint");
                TypeBrush.Text = strings.GetString(prefix, "ShowBrush");
                IncludeHidden.Text = strings.GetString(prefix, "IncludeHidden");
                FilterByKeyValueLabel.Text = strings.GetString(prefix, "FilterByKeyValue");
                FilterByClassLabel.Text = strings.GetString(prefix, "FilterByClass");
                FilterClassExact.Text = strings.GetString(prefix, "Exact");
                FilterKeyValueExact.Text = strings.GetString(prefix, "Exact");
                ResetFiltersButton.Text = strings.GetString(prefix, "ResetFilters");
                CloseButton.Text = strings.GetString(prefix, "Close");
            });
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            Oy.Publish("Context:Remove", new ContextInfo("BspEditor:EntityReport"));
        }

	    protected override void OnMouseEnter(EventArgs e)
		{
            Focus();
            base.OnMouseEnter(e);
        }

        public bool IsInContext(IContext context)
        {
            return context.HasAny("BspEditor:EntityReport");
        }

        public void SetVisible(IContext context, bool visible)
        {
            this.InvokeLater(() =>
            {
                if (visible)
                {
                    if (!Visible) Show(_parent.Value);
                    Subscribe();
                    ResetFilters(null, null);
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

        public async Task DocumentActivated(MapDocument document)
        {
            FiltersChanged(null, null);
        }

        public async Task SelectionChanged(MapDocument document)
        {
            if (!FollowSelection.Checked) return;

            var doc = _context.Get<MapDocument>("ActiveDocument");
            if (doc == null) return;

            var selection = doc.Selection.GetSelectedParents().LastOrDefault(x => x is Entity);
            SetSelected(selection);
        }

        private async Task DocumentChanged(Change change)
        {
            if (!change.HasObjectChanges) return;

            if (change.Added.Any(x => x is Entity) || change.Updated.Any(x => x is Entity) || change.Removed.Any(x => x is Entity))
            {
                FiltersChanged(null, null);
            }
        }

        private Entity GetSelected()
        {
            return EntityList.SelectedItems.Count == 0 ? null : (Entity) EntityList.SelectedItems[0].Tag;
        }

        private void SetSelected(IMapObject selection)
        {
            this.InvokeLater(() =>
            {
                if (selection == null) return;

                var item = EntityList.Items.OfType<ListViewItem>().FirstOrDefault(x => x.Tag == selection);
                if (item == null) return;

                item.Selected = true;
                EntityList.EnsureVisible(EntityList.Items.IndexOf(item));
            });
        }

        private void FiltersChanged(object sender, EventArgs e)
        {
            this.InvokeLater(() =>
            {
                EntityList.BeginUpdate();
                var selected = GetSelected();
                EntityList.ListViewItemSorter = null;
                EntityList.Items.Clear();

                var doc = _context.Get<MapDocument>("ActiveDocument");
                if (doc != null)
                {
                    var items = doc.Map.Root
                        .Find(x => x is Entity)
                        .OfType<Entity>()
                        .Where(DoFilters)
                        .Select(GetListItem)
                        .ToArray();
                    EntityList.Items.AddRange(items);

                    EntityList.ListViewItemSorter = _sorter;
                    EntityList.Sort();
                    SetSelected(selected);
                }

                EntityList.EndUpdate();
            });
        }

        private ListViewItem GetListItem(Entity entity)
        {
            var targetname = entity.EntityData.Properties.FirstOrDefault(x => x.Key.ToLower() == "targetname");
            return new ListViewItem(new[]
                                        {
                                            entity.EntityData.Name,
                                            targetname.Value ?? ""
                                        }) {Tag = entity};
        }

        private bool DoFilters(Entity ent)
        {
            var hasChildren = ent.Hierarchy.HasChildren;

            if (hasChildren && TypePoint.Checked) return false;
            if (!hasChildren && TypeBrush.Checked) return false;
            if (!IncludeHidden.Checked)
            {
                if (ent.Data.OfType<IObjectVisibility>().Any(x => x.IsHidden)) return false;
            }

            var classFilter = FilterClass.Text.ToUpperInvariant();
            var exactClass = FilterClassExact.Checked;
            var keyFilter = FilterKey.Text.ToUpperInvariant();
            var valueFilter = FilterValue.Text.ToUpperInvariant();
            var exactKeyValue = FilterKeyValueExact.Checked;

            if (!String.IsNullOrWhiteSpace(classFilter))
            {
                var name = (ent.EntityData.Name ?? "").ToUpperInvariant();
                if (exactClass && name != classFilter) return false;
                if (!exactClass && !name.Contains(classFilter)) return false;
            }

            if (!String.IsNullOrWhiteSpace(keyFilter))
            {
                if (ent.EntityData.Properties.All(x => x.Key.ToUpperInvariant() != keyFilter)) return false;
                var prop = ent.EntityData.Properties.FirstOrDefault(x => x.Key.ToUpperInvariant() == keyFilter);
                var val = prop.Value.ToUpperInvariant();
                if (exactKeyValue && val != valueFilter) return false;
                if (!exactKeyValue && !val.Contains(valueFilter)) return false;
            }

            return true;
        }

        private void ResetFilters(object sender, EventArgs e)
        {
            TypeAll.Checked = true;
            IncludeHidden.Checked = true;
            FilterKeyValueExact.Checked = false;
            FilterClassExact.Checked = false;
            FilterKey.Text = "";
            FilterValue.Text = "";
            FilterClass.Text = "";
            FiltersChanged(null, null);
        }

        private void SortByColumn(object sender, ColumnClickEventArgs e)
        {
            if (_sorter.Column == e.Column)
            {
                _sorter.SortOrder = _sorter.SortOrder == SortOrder.Descending
                                        ? SortOrder.Ascending
                                        : SortOrder.Descending;
            }
            else
            {
                _sorter.Column = e.Column;
                _sorter.SortOrder = SortOrder.Ascending;
            }
            EntityList.Sort();
            SetSelected(GetSelected()); // Reset the scroll value
        }

        private async Task SelectEntity(Entity sel)
        {
            var doc = _context.Get<MapDocument>("ActiveDocument");
            if (doc == null) return;

            var currentSelection = doc.Selection.Except(sel.FindAll()).ToList();
            var tran = new Transaction(
                new Deselect(currentSelection),
                new Select(sel.FindAll())
            );
            await MapDocumentOperation.Perform(doc, tran);
        }

        private void GoToSelectedEntity(object sender, EventArgs e)
        {
            var selected = GetSelected();
            if (selected == null) return;
            SelectEntity(selected);
            Oy.Publish("MapDocument:Viewport:Focus2D", selected.BoundingBox);
            Oy.Publish("MapDocument:Viewport:Focus3D", selected.BoundingBox);
        }

        private void DeleteSelectedEntity(object sender, EventArgs e)
        {
            var doc = _context.Get<MapDocument>("ActiveDocument");
            if (doc == null) return;

            var selected = GetSelected();
            if (selected == null) return;
            MapDocumentOperation.Perform(doc, new Detatch(selected.Hierarchy.Parent.ID, selected));
        }

        private void OpenEntityProperties(object sender, EventArgs e)
        {
            var selected = GetSelected();
            if (selected == null) return;
            SelectEntity(selected).ContinueWith(_ => Oy.Publish("Command:Run", new CommandMessage("BspEditor:Map:Properties")));
        }

        private void CloseButtonClicked(object sender, EventArgs e)
        {
            Close();
        }
    }
}
