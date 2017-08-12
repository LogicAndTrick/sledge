using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.Operations.Data;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Shell.Context;
using Sledge.Common.Translations;
using Sledge.Shell;

namespace Sledge.BspEditor.Editing.Components.Properties.Tabs
{
    /// <summary>
    /// Visgroups are a way to group multiple objects together in multiple different ways.
    /// Visgroups are hierarchical and an object can be a member of any number of visgroups.
    /// Membership of a visgroup implies membership of the visgroup's parent (and so on).
    /// 
    /// Visgroups are always available as long as at least one object is selected.
    /// Automatic visgroups are handled by the editor and are not available to be modified.
    /// </summary>
    [AutoTranslate]
    [Export(typeof(IObjectPropertyEditorTab))]
    public sealed partial class VisgroupTab : UserControl, IObjectPropertyEditorTab
    {
        private Dictionary<Primitives.MapData.Visgroup, CheckState> _state;

        /// <inheritdoc />
        public string OrderHint => "Y";

        /// <inheritdoc />
        public Control Control => this;

        public string MemberOfGroup
        {
            get => lblMemberOfGroup.Text;
            set => this.Invoke(() => lblMemberOfGroup.Text = value);
        }

        public string EditVisgroups
        {
            get => btnEditVisgroups.Text;
            set => this.Invoke(() => btnEditVisgroups.Text = value);
        }

        /// <inheritdoc />
        public bool HasChanges => GetMembershipChanges().Count > 0;

        public event PropertyChangedEventHandler PropertyChanged;

        public VisgroupTab()
        {
            InitializeComponent();
            CreateHandle();

            _state = new Dictionary<Primitives.MapData.Visgroup, CheckState>();
        }

        /// <inheritdoc />
        public bool IsInContext(IContext context)
        {
            return context.TryGet("ActiveDocument", out MapDocument doc)
                && !doc.Selection.IsEmpty;
        }

        /// <inheritdoc />
        public Task SetObjects(MapDocument document, List<IMapObject> objects)
        {
            visgroupPanel.Invoke(() =>
            {
                _state = GetVisgroups(document, objects);
                visgroupPanel.Update(_state);
            });
            return Task.FromResult(0);
        }

        /// <summary>
        /// Get the list of visgroup membership that has been changed since the objects were set.
        /// Indeterminate checkboxes are never a change.
        /// </summary>
        private Dictionary<Primitives.MapData.Visgroup, bool> GetMembershipChanges()
        {
            var dic = new Dictionary<Primitives.MapData.Visgroup, bool>();
            var cs = visgroupPanel.GetAllCheckStates();
            foreach (var kv in _state)
            {
                if (kv.Value == CheckState.Indeterminate || !cs.ContainsKey(kv.Key.ID)) continue;
                var newState = cs[kv.Key.ID];
                if (newState != kv.Value) dic[kv.Key] = newState == CheckState.Checked;
            }
            return dic;
        }

        /// <summary>
        /// Get the list of visgroups in the document and the membership states of the given objects.
        /// </summary>
        private Dictionary<Primitives.MapData.Visgroup, CheckState> GetVisgroups(MapDocument document, List<IMapObject> objects)
        {
            var d = new Dictionary<Primitives.MapData.Visgroup, CheckState>();
            if (document == null) return d;

            var objGroups = objects
                .SelectMany(x => x.Data.Get<VisgroupID>().Select(v => new {Object = x, Visgroup = v.ID}))
                .GroupBy(x => x.Visgroup)
                .ToDictionary(x => x.Key, x => x.Count());

            foreach (var visgroup in document.Map.Data.Get<Primitives.MapData.Visgroup>())
            {
                var id = visgroup.ID;
                var cs = CheckState.Unchecked;
                if (objGroups.ContainsKey(id))
                {
                    if (objGroups[id] == objects.Count) cs = CheckState.Checked;
                    else cs = CheckState.Indeterminate;
                }
                d[visgroup] = cs;
            }

            return d;
        }

        /// <inheritdoc />
        public IEnumerable<IOperation> GetChanges(MapDocument document, List<IMapObject> objects)
        {
            var mc = GetMembershipChanges();
            if (mc.Count == 0) yield break;

            foreach (var mo in objects)
            {
                var visgroups = mo.Data.Get<VisgroupID>().ToDictionary(x => x.ID);
                foreach (var kv in mc)
                {
                    var id = kv.Key.ID;
                    if (kv.Value && !visgroups.ContainsKey(id))
                    {
                        // Object should be a member of this visgroup but it's not - add it
                        yield return new AddMapObjectData(mo.ID, new VisgroupID(id));
                    }
                    else if (!kv.Value && visgroups.ContainsKey(id))
                    {
                        // Object should not be a member of this visgroup but it is - remove it
                        yield return new RemoveMapObjectData(mo.ID, visgroups[id]);
                    }
                }
            }
        }

        private void VisgroupToggled(object sender, long visgroupId, CheckState state)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasChanges)));
        }
    }
}
