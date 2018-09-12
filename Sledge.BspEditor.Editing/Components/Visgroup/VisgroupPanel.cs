using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Sledge.Common;

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
        public delegate void VisgroupToggledEventHandler(object sender, VisgroupItem visgroup, CheckState state);
        public delegate void VisgroupSelectedEventHandler(object sender, VisgroupItem visgroup);

        /// <summary>
        /// Fires when the checkstate of a visgroup has changed.
        /// </summary>
        public event VisgroupToggledEventHandler VisgroupToggled;

        /// <summary>
        /// Fires when a visgroup is selected.
        /// </summary>
        public event VisgroupSelectedEventHandler VisgroupSelected;

        private void OnVisgroupToggled(VisgroupItem visgroup, CheckState state)
        {
            VisgroupToggled?.Invoke(this, visgroup, state);
        }

        private void OnVisgroupSelected(VisgroupItem visgroup)
        {
            VisgroupSelected?.Invoke(this, visgroup);
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
        /// Get the currently selected visgroup
        /// </summary>
        /// <returns></returns>
        public VisgroupItem SelectedVisgroup
        {
            get => VisgroupTree.SelectedNode?.Tag as VisgroupItem;
            set => VisgroupTree.SelectedNode = GetNodeForTag(value?.Tag);
        }

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

        private void AddNode(TreeNode parent, List<VisgroupItem> visgroups, VisgroupItem visgroup)
        {
            var node = new TreeNode(visgroup.Text)
            {
                StateImageKey = CastCheckState(visgroup.CheckState) + (visgroup.Disabled ? "Disabled" : ""),
                BackColor = visgroup.Colour,
                ForeColor = visgroup.Colour.GetIdealForegroundColour(),
                Tag = visgroup
            };

            if (parent == null) VisgroupTree.Nodes.Add(node);
            else parent.Nodes.Add(node);

            var children = visgroups.Where(x => x.Parent == visgroup);
            foreach (var vg in children)
            {
                AddNode(node, visgroups, vg);
            }
        }

        /// <summary>
        /// Update the visgroup list with a list of visgroups and their current visible states.
        /// 
        /// The visible state of a visgroup may differ from the visible state of the objects
        /// in the visgroup. This method does not consider "mixed" visibility.
        /// </summary>
        public void Update(IEnumerable<VisgroupItem> visgroups)
        {
            VisgroupTree.BeginUpdate();

            var state = GetExpandState();
            var scroll = GetTreeViewScrollPos(VisgroupTree);

            Clear();
            var vgs = visgroups.ToList();
            foreach (var v in vgs.Where(x => x.Parent == null))
            {
                AddNode(null, vgs, v);
            }

            RestoreExpandState(state);
            SetTreeViewScrollPos(VisgroupTree, scroll);

            VisgroupTree.EndUpdate();
        }

        private void RestoreExpandState(List<string> expanded)
        {
            foreach (var node in GetAllNodePaths().Where(x => expanded.Contains(x.Value)))
            {
                node.Key.Expand();
            }
        }

        private List<string> GetExpandState()
        {
            return GetAllNodePaths().Where(x => x.Key.IsExpanded).Select(x => x.Value).ToList();
        }

        private Dictionary<TreeNode, string> GetAllNodePaths()
        {
            return GetAllNodePaths(VisgroupTree.Nodes.OfType<TreeNode>().ToDictionary(x => x, x => x.Text));
        }

        private Dictionary<TreeNode, string> GetAllNodePaths(Dictionary<TreeNode, string> nodes)
        {
            var dic = new Dictionary<TreeNode, string>(nodes);

            var children = nodes.SelectMany(x => x.Key.Nodes.OfType<TreeNode>().Select(n => new {x, n}))
                .ToDictionary(x => x.n, x => x.x.Value + "/" + x.n.Text);
            if (children.Any())
            {
                foreach (var kv in GetAllNodePaths(children))
                {
                    dic[kv.Key] = kv.Value;
                }
            }

            return dic;
        }
        
        /// <summary>
        /// Convert a CheckState into a string representation of the checkbox icon.
        /// </summary>
        private string CastCheckState(CheckState state)
        {
            return state == CheckState.Indeterminate ? "Mixed" : state.ToString();
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
        /// Get the visgroup item in the tree with the given tag
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public VisgroupItem GetItemForTab(object tag)
        {
            return GetNodeForTag(tag)?.Tag as VisgroupItem;
        }

        /// <summary>
        /// Get the treenode that matches a given visgroup id
        /// </summary>
        private TreeNode GetNodeForTag(object tag)
        {
            return GetAllNodes().FirstOrDefault(x => (x.Tag as VisgroupItem)?.Tag == tag);
        }
        
        /// <summary>
        /// Alter the properties of an item in the panel and redraw the interface.
        /// </summary>
        public void UpdateVisgroupState(VisgroupItem visgroup)
        {
            var node = GetNodeForTag(visgroup.Tag);
            if (node == null) return;

            node.Text = visgroup.Text;
            node.StateImageKey = CastCheckState(visgroup.CheckState);
            node.BackColor = visgroup.Colour;
            node.ForeColor = visgroup.Colour.GetIdealForegroundColour();
            if (node.Tag is VisgroupItem i) i.Tag = visgroup.Tag;
            
            VisgroupTree.Invalidate();
        }

        /// <summary>
        /// Get the checkstate of a visgroup.
        /// </summary>
        public CheckState GetCheckState(VisgroupItem visgroup)
        {
            var node = GetNodeForTag(visgroup.Tag);
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
        public Dictionary<VisgroupItem, CheckState> GetAllCheckStates()
        {
            return GetAllNodes()
                .Where(x => x.Tag is VisgroupItem)
                .ToDictionary(x => (VisgroupItem) x.Tag, GetCheckState);
        }

        /// <summary>
        /// Get all the visgroup items in the tree as a flat list
        /// </summary>
        public List<VisgroupItem> GetAllItems()
        {
            return GetAllNodes().Select(x => x.Tag).OfType<VisgroupItem>().ToList();
        }

        /// <summary>
        /// Set the checkstate of a visgroup.
        /// </summary>
        public void SetCheckState(VisgroupItem visgroup, CheckState state)
        {
            var node = GetNodeForTag(visgroup.Tag);
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

            var vis = (VisgroupItem) e.Node.Tag;

            // unchecked -> checked, checked -> unchecked, mixed -> unchecked
            var visible = e.Node.StateImageKey.StartsWith("Unchecked");
            e.Node.StateImageKey = (visible ? "Checked" : "Unchecked");
            OnVisgroupToggled(vis, visible ? CheckState.Checked : CheckState.Unchecked);
        }

        private void NodeSelected(object sender, TreeViewEventArgs e)
        {
            OnVisgroupSelected(SelectedVisgroup);
        }

        private void OnItemDrag(object sender, ItemDragEventArgs e)
        {
            // todo post-beta: visgroup drag-and-drop ordering
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


        // https://stackoverflow.com/a/359896

        [DllImport("user32.dll",  CharSet = CharSet.Unicode)]
        public static extern int GetScrollPos(IntPtr hWnd, int nBar);

        [DllImport("user32.dll",  CharSet = CharSet.Unicode)]
        public static extern int SetScrollPos(IntPtr hWnd, int nBar, int nPos, bool bRedraw);

        private const int SbHorz = 0x0;
        private const int SbVert = 0x1;

        private Point GetTreeViewScrollPos(TreeView treeView)
        {
            return new Point(
                GetScrollPos(treeView.Handle, SbHorz), 
                GetScrollPos(treeView.Handle, SbVert));
        }
        private void SetTreeViewScrollPos(TreeView treeView, Point scrollPosition)
        {
            SetScrollPos(treeView.Handle, SbHorz, scrollPosition.X, true);
            SetScrollPos(treeView.Handle, SbVert, scrollPosition.Y, true); 
        }
    }
}
