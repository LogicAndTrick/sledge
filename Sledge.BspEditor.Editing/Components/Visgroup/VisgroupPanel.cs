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
    public partial class VisgroupPanel : UserControl
    {
        public delegate void VisgroupToggledEventHandler(object sender, long visgroupId, CheckState state);
        public delegate void VisgroupSelectedEventHandler(object sender, long? visgroupId);

        public event VisgroupToggledEventHandler VisgroupToggled;
        public event VisgroupSelectedEventHandler VisgroupSelected;

        private void OnVisgroupToggled(long visgroupId, CheckState state)
        {
            VisgroupToggled?.Invoke(this, visgroupId, state);
        }

        private void OnVisgroupSelected(long? visgroupId)
        {
            VisgroupSelected?.Invoke(this, visgroupId);
        }

        public bool ShowCheckboxes
        {
            get => VisgroupTree.StateImageList != null;
            set => VisgroupTree.StateImageList = value ? CheckboxImages : null;
        }

        public bool DisableAutomatic { get; set; }
        public bool HideAutomatic { get; set; }
        public bool SortAutomaticFirst { get; set; }
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

        private void AddNode(TreeNode parent, List<Primitives.MapData.Visgroup> visgroups, Primitives.MapData.Visgroup visgroup, Func<Primitives.MapData.Visgroup, string> getCheckState)
        {
            // todo
            //if (!ShowHidden && visgroup is AutoVisgroup && ((AutoVisgroup)visgroup).IsHidden) return;
            //if (HideAutomatic && visgroup.IsAutomatic) return;
            var node = new TreeNode(visgroup.Name)
            {
                StateImageKey = getCheckState(visgroup), //+ (DisableAutomatic && visgroup.IsAutomatic ? "Disabled" : ""),
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

        public void Update(MapDocument document)
        {
            Clear();
            if (document == null) return;

            var visgroups = document.Map.Data.Get<Primitives.MapData.Visgroup>().ToList();
            var topLevel = visgroups.Where(x => visgroups.All(v => x.Parent != v.ID));
            foreach (var v in Sort(topLevel))
            {
                AddNode(null, visgroups, v, x => GetCheckState(x.Objects));
            }
        }

        private string GetCheckState(IEnumerable<IMapObject> objects)
        {
            var bools = objects.Select(x => x.Data.GetOne<VisgroupHidden>()?.IsHidden ?? false);
            return GetCheckState(bools);
        }

        private string GetCheckState(IEnumerable<bool> bools)
        {
            var a = bools.Distinct().ToArray();
            if (a.Length == 0) return "Checked";
            if (a.Length == 1) return a[0] ? "Unchecked" : "Checked";
            return "Mixed";
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

        public void Update(IEnumerable<Primitives.MapData.Visgroup> visgroups)
        {
            Clear();
            var vgs = visgroups.ToList();
            foreach (var v in Sort(vgs))
            {
                AddNode(null, vgs, v, x => x.Visible ? "Checked" : "Unchecked");
            }
        }

        public void Clear()
        {
            VisgroupTree.Nodes.Clear();
        }

        public void ExpandAllNodes()
        {
            VisgroupTree.ExpandAll();
        }

        private TreeNode GetNodeForVisgroupID(long visgroupId)
        {
            return GetAllNodes().FirstOrDefault(x => x.Tag is int && (int)x.Tag == visgroupId);
        }

        public int? GetSelectedVisgroup()
        {
            var selected = VisgroupTree.SelectedNode;
            var id = selected == null ? (int?)null : (int)selected.Tag;
            return id;
        }

        public void SetSelectedVisgroup(long visgroupId)
        {
            VisgroupTree.SelectedNode = GetNodeForVisgroupID(visgroupId);
        }

        public void UpdateVisgroupName(long visgroupId, string name)
        {
            var node = GetNodeForVisgroupID(visgroupId);
            if (node != null) node.Text = name;
        }

        public void UpdateVisgroupColour(long visgroupId, Color colour)
        {
            var node = GetNodeForVisgroupID(visgroupId);
            if (node != null) node.BackColor = colour;
        }

        public CheckState GetCheckState(long visgroupId)
        {
            var node = GetNodeForVisgroupID(visgroupId);
            return GetCheckState(node);
        }

        private CheckState GetCheckState(TreeNode node)
        {
            if (node == null || node.StateImageKey.StartsWith("Unchecked")) return CheckState.Unchecked;
            if (node.StateImageKey.StartsWith("Checked")) return CheckState.Checked;
            return CheckState.Indeterminate;
        }

        public Dictionary<int, CheckState> GetAllCheckStates()
        {
            return GetAllNodes()
                .Where(x => x.Tag is int)
                .ToDictionary(x => (int) x.Tag, GetCheckState);
        }

        public void SetCheckState(long visgroupId, CheckState state)
        {
            var node = GetNodeForVisgroupID(visgroupId);
            if (node == null) return;
            var disabled = node.StateImageKey.EndsWith("Disabled");
            switch (state)
            {
                case CheckState.Unchecked:
                    node.StateImageKey = "Unchecked";
                    break;
                case CheckState.Checked:
                    node.StateImageKey = "Checked";
                    break;
                case CheckState.Indeterminate:
                    node.StateImageKey = "Mixed";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("state");
            }
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
