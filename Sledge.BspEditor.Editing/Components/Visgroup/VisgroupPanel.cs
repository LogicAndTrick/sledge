using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;

namespace Sledge.BspEditor.Editing.Components.Visgroup
{
    /// <summary>
    /// A visgroup viewing and editing panel.
    /// 
    /// Serves multiple purposes:
    ///  1. When editing visgroups, can be used to add and remove visgroups
    ///  2. When viewing objects, can be used to toggle visgroup visibilities
    ///  3. When editing objects, can be used to toggle visgroup membership
    /// </summary>
    public partial class VisgroupPanel : UserControl
    {
        public delegate void VisgroupToggledEventHandler(object sender, long visgroupId, CheckState state);
        public delegate void VisgroupSelectedEventHandler(object sender, long? visgroupId);

        /// <summary>
        /// Fires when the checkstate of a visgroup has changed.
        /// </summary>
        public event VisgroupToggledEventHandler VisgroupToggled;

        /// <summary>
        /// Fires when a visgroup is selected.
        /// </summary>
        public event VisgroupSelectedEventHandler VisgroupSelected;

        private void OnVisgroupToggled(long visgroupId, CheckState state)
        {
            VisgroupToggled?.Invoke(this, visgroupId, state);
        }

        private void OnVisgroupSelected(long? visgroupId)
        {
            VisgroupSelected?.Invoke(this, visgroupId);
        }

        /// <summary>
        /// True to show the checkbox next to each visgroup
        /// </summary>
        public bool ShowCheckboxes
        {
            get => VisgroupTree.StateImageList != null;
            set => VisgroupTree.StateImageList = value ? CheckboxImages : null;
        }

        /// <summary>
        /// True to disable user changes to automatic visgroups
        /// </summary>
        public bool DisableAutomatic { get; set; }

        /// <summary>
        /// True to show automatic visgroups
        /// </summary>
        public bool HideAutomatic { get; set; }

        /// <summary>
        /// True to put automatic visgroups above user visgroups
        /// </summary>
        public bool SortAutomaticFirst { get; set; }

        /// <summary>
        /// True to show hidden visgroups
        /// </summary>
        public bool ShowHidden { get; set; }

        public VisgroupPanel()
        {
            InitializeComponent();

            /* http://www.codeproject.com/script/Articles/ViewDownloads.aspx?aid=202435 */
            CheckboxImages.Images.Add("Unchecked", GetCheckboxBitmap(CheckBoxState.UncheckedNormal));
            CheckboxImages.Images.Add("Checked", GetCheckboxBitmap(CheckBoxState.CheckedNormal));
            CheckboxImages.Images.Add("Mixed", GetCheckboxBitmap(CheckBoxState.MixedNormal));
            
            CheckboxImages.Images.Add("UncheckedDisabled", GetCheckboxBitmap(CheckBoxState.UncheckedDisabled));
            CheckboxImages.Images.Add("CheckedDisabled", GetCheckboxBitmap(CheckBoxState.CheckedDisabled));
            CheckboxImages.Images.Add("MixedDisabled", GetCheckboxBitmap(CheckBoxState.MixedDisabled));
        }

        private static Bitmap GetCheckboxBitmap(CheckBoxState state)
        {
            /* http://stackoverflow.com/questions/5626031/tri-state-checkboxes-in-winforms-treeview */
            var bmp = new Bitmap(16, 16);
            using (var g = Graphics.FromImage(bmp))
            {
                CheckBoxRenderer.DrawCheckBox(g, new Point(0, 1), state);
            }
            return bmp;
        }

        private IEnumerable<Primitives.MapData.Visgroup> Sort(IEnumerable<Primitives.MapData.Visgroup> list)
        {
            return list;
            // todo
            // return SortAutomaticFirst
            //            ? list.OrderBy(x => x.IsAutomatic ? 0 : 1).ThenBy(x => x.Name)
            //            : list.OrderBy(x => x.IsAutomatic ? 1 : 0).ThenBy(x => x.Name);
        }

        private void AddNode(TreeNode parent, List<Primitives.MapData.Visgroup> visgroups, Primitives.MapData.Visgroup visgroup, Func<Primitives.MapData.Visgroup, CheckState> getCheckState)
        {
            // todo
            //if (!ShowHidden && visgroup is AutoVisgroup && ((AutoVisgroup)visgroup).IsHidden) return;
            //if (HideAutomatic && visgroup.IsAutomatic) return;
            var node = new TreeNode(visgroup.Name)
            {
                StateImageKey = CastCheckState(getCheckState(visgroup)), //+ (DisableAutomatic && visgroup.IsAutomatic ? "Disabled" : ""),
                BackColor = visgroup.Colour,
                Tag = visgroup.ID
            };

            if (parent == null) VisgroupTree.Nodes.Add(node);
            else parent.Nodes.Add(node);

            var children = visgroups.Where(x => x.Parent == visgroup.ID);
            foreach (var vg in Sort(children))
            {
                AddNode(node, visgroups, vg, getCheckState);
            }
        }

        /// <summary>
        /// Update the visgroup list with the visgroup list from a document and the visibility states of the objects in each visgroup.
        /// </summary>
        public void Update(MapDocument document)
        {
            Clear();
            if (document == null) return;

            var visgroups = document.Map.Data.Get<Primitives.MapData.Visgroup>().ToList();
            var topLevel = visgroups.Where(x => visgroups.All(v => x.Parent != v.ID));
            foreach (var v in Sort(topLevel))
            {
                AddNode(null, visgroups, v, x => GetVisibilityCheckState(x.Objects));
            }
        }

        /// <summary>
        /// Update the visgroup list with a list of visgroups and their current visible states.
        /// 
        /// The visible state of a visgroup may differ from the visible state of the objects
        /// in the visgroup. This method does not consider "mixed" visibility.
        /// </summary>
        public void Update(IEnumerable<Primitives.MapData.Visgroup> visgroups)
        {
            Clear();
            var vgs = visgroups.ToList();
            foreach (var v in Sort(vgs))
            {
                AddNode(null, vgs, v, x => x.Visible ? CheckState.Checked : CheckState.Unchecked);
            }
        }

        /// <summary>
        /// Update the visgroup list with a list of visgroups and the provided checkstates.
        /// </summary>
        public void Update(Dictionary<Primitives.MapData.Visgroup, CheckState> visgroupsAndCheckStates)
        {
            Clear();
            var vgs = visgroupsAndCheckStates.Keys.ToList();
            foreach (var v in Sort(vgs))
            {
                AddNode(null, vgs, v, x => visgroupsAndCheckStates[v]);
            }
        }

        /// <summary>
        /// Convert a CheckState into a string representation of the checkbox icon.
        /// </summary>
        private string CastCheckState(CheckState state)
        {
            return state == CheckState.Indeterminate ? "Mixed" : state.ToString();
        }

        /// <summary>
        /// Get the checkstate by considering the visibility of a list of objects.
        /// </summary>
        private CheckState GetVisibilityCheckState(IEnumerable<IMapObject> objects)
        {
            var bools = objects.Select(x => x.Data.GetOne<VisgroupHidden>()?.IsHidden ?? false);
            return GetCheckState(bools);
        }

        /// <summary>
        /// Get the checkstate from a raw list of toggles.
        /// 
        /// If all the toggles are true, the state is Checked.
        /// If all the toggles are false, the state is Unchecked.
        /// Otherwise there is a mix of true/false values, and the state is Mixed.
        /// </summary>
        private CheckState GetCheckState(IEnumerable<bool> bools)
        {
            var a = bools.Distinct().ToArray();
            if (a.Length == 0) return CheckState.Checked;
            if (a.Length == 1) return a[0] ? CheckState.Unchecked : CheckState.Checked;
            return CheckState.Indeterminate;
        }

        private IEnumerable<TreeNode> GetAllNodes()
        {
            return GetAllNodes(VisgroupTree.Nodes.OfType<TreeNode>());
        }

        private IEnumerable<TreeNode> GetAllNodes(IEnumerable<TreeNode> nodes)
        {
            var n = nodes.ToList();
            return n.SelectMany(x => GetAllNodes(x.Nodes.OfType<TreeNode>())).Union(n);
        }

        /// <summary>
        /// Remove all nodes from the visgroup tree
        /// </summary>
        public void Clear()
        {
            VisgroupTree.Nodes.Clear();
        }

        /// <summary>
        /// Expand all nodes in the visgroup tree
        /// </summary>
        public void ExpandAllNodes()
        {
            VisgroupTree.ExpandAll();
        }

        /// <summary>
        /// Get the treenode that matches a given visgroup id
        /// </summary>
        private TreeNode GetNodeForVisgroupID(long visgroupId)
        {
            return GetAllNodes().FirstOrDefault(x => x.Tag is long && (long)x.Tag == visgroupId);
        }

        /// <summary>
        /// Get the id of the currently selected visgroup
        /// </summary>
        /// <returns></returns>
        public long? GetSelectedVisgroup()
        {
            var selected = VisgroupTree.SelectedNode;
            var id = selected == null ? (long?)null : (long)selected.Tag;
            return id;
        }

        /// <summary>
        /// Set the currently selected visgroup
        /// </summary>
        public void SetSelectedVisgroup(long? visgroupId)
        {
            VisgroupTree.SelectedNode = GetNodeForVisgroupID(visgroupId.GetValueOrDefault(long.MinValue));
        }

        /// <summary>
        /// Alter the name of an item in the panel. Doesn't modify the actual map objects represented by the visgroup.
        /// </summary>
        public void UpdateVisgroupName(long visgroupId, string name)
        {
            var node = GetNodeForVisgroupID(visgroupId);
            if (node != null) node.Text = name;
        }

        /// <summary>
        /// Alter the colour of an item in the panel. Doesn't modify the actual map objects represented by the visgroup.
        /// </summary>
        public void UpdateVisgroupColour(long visgroupId, Color colour)
        {
            var node = GetNodeForVisgroupID(visgroupId);
            if (node != null) node.BackColor = colour;
        }

        /// <summary>
        /// Get the checkstate of a visgroup.
        /// </summary>
        public CheckState GetCheckState(long visgroupId)
        {
            var node = GetNodeForVisgroupID(visgroupId);
            return GetCheckState(node);
        }

        /// <summary>
        /// Get the checkstate of a tree node.
        /// </summary>
        private CheckState GetCheckState(TreeNode node)
        {
            if (node == null || node.StateImageKey.StartsWith("Unchecked")) return CheckState.Unchecked;
            if (node.StateImageKey.StartsWith("Checked")) return CheckState.Checked;
            return CheckState.Indeterminate;
        }

        /// <summary>
        /// Get a dictionary of visgroup ids and their current check states.
        /// </summary>
        public Dictionary<long, CheckState> GetAllCheckStates()
        {
            return GetAllNodes()
                .Where(x => x.Tag is long)
                .ToDictionary(x => (long) x.Tag, GetCheckState);
        }

        /// <summary>
        /// Set the checkstate of a visgroup.
        /// </summary>
        public void SetCheckState(long visgroupId, CheckState state)
        {
            var node = GetNodeForVisgroupID(visgroupId);
            if (node == null) return;
            var disabled = node.StateImageKey.EndsWith("Disabled");
            node.StateImageKey = CastCheckState(state);
            if (disabled) node.StateImageKey += "Disabled";
        }

        private void NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (!ShowCheckboxes) return;

            // Only do something if the click is over the image (checkbox)
            var hit = VisgroupTree.HitTest(e.X, e.Y);
            if (hit.Location != TreeViewHitTestLocations.StateImage) return;

            var disabled = e.Node.StateImageKey.EndsWith("Disabled");
            if (disabled) return;

            var id = (long) e.Node.Tag;
            // unchecked -> checked, checked -> unchecked, mixed -> unchecked
            var visible = e.Node.StateImageKey.StartsWith("Unchecked");
            e.Node.StateImageKey = (visible ? "Checked" : "Unchecked");
            OnVisgroupToggled(id, visible ? CheckState.Checked : CheckState.Unchecked);
        }

        private void NodeSelected(object sender, TreeViewEventArgs e)
        {
            OnVisgroupSelected(GetSelectedVisgroup());
        }

        private void OnItemDrag(object sender, ItemDragEventArgs e)
        {
            // todo ordering
            //DoDragDrop(e.Item, DragDropEffects.Move);
        }

        private void OnDragDrop(object sender, DragEventArgs e)
        {
            //MessageBox.Show("blah?");
        }

        private void OnDragEnter(object sender, DragEventArgs e)
        {
            //if (e.Data.GetData(typeof (TreeNode)) != null) e.Effect = DragDropEffects.Move;
        }
    }
}
