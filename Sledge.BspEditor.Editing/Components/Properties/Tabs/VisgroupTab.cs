using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
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
    public partial class VisgroupTab : UserControl, IObjectPropertyEditorTab
    {
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
        public bool HasChanges
        {
            get { return false; }
        }

        public VisgroupTab()
        {
            InitializeComponent();
            CreateHandle();
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
                var d = GetVisgroups(document, objects);
                visgroupPanel.Update(d);
            });
            return Task.FromResult(0);
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
        public IEnumerable<MapDocumentOperation> GetChanges(MapDocument document)
        {
            yield break;
        }
    }
}
