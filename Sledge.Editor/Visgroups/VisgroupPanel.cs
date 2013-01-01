using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Sledge.DataStructures.MapObjects;

namespace Sledge.Editor.Visgroups
{
    public partial class VisgroupPanel : UserControl
    {
        public delegate void VisgroupToggledEventHandler(object sender, int visgroupId, CheckState state);
        public delegate void VisgroupSelectedEventHandler(object sender, int? visgroupId);

        public event VisgroupToggledEventHandler VisgroupToggled;
        public event VisgroupSelectedEventHandler VisgroupSelected;

        protected void OnVisgroupToggled(int visgroupId, CheckState state)
        {
            if (VisgroupToggled != null)
            {
                VisgroupToggled(this, visgroupId, state);
            }
        }

        protected void OnVisgroupSelected(int? visgroupId)
        {
            if (VisgroupSelected != null)
            {
                VisgroupSelected(this, visgroupId);
            }
        }

        public bool ShowCheckboxes
        {
            get { return VisgroupTree.StateImageList != null; }
            set { VisgroupTree.StateImageList = value ? CheckboxImages : null; }
        }

        public VisgroupPanel()
        {
            InitializeComponent();
            /* http://www.codeproject.com/script/Articles/ViewDownloads.aspx?aid=202435 */
            CheckboxImages.Images.Add("Unchecked", GetCheckboxBitmap(CheckBoxState.UncheckedNormal));
            CheckboxImages.Images.Add("Checked", GetCheckboxBitmap(CheckBoxState.CheckedNormal));
            CheckboxImages.Images.Add("Mixed", GetCheckboxBitmap(CheckBoxState.CheckedDisabled));
        }

        private static Bitmap GetCheckboxBitmap(CheckBoxState state)
        {
            /* http://stackoverflow.com/questions/5626031/tri-state-checkboxes-in-winforms-treeview */
            var bmp = new Bitmap(16, 16);
            using (var g = System.Drawing.Graphics.FromImage(bmp))
            {
                CheckBoxRenderer.DrawCheckBox(g, new Point(0, 1), state);
            }
            return bmp;
        }

        public void Update(IEnumerable<Visgroup> visgroups)
        {
            Clear();
            foreach (var v in visgroups)
            {
                VisgroupTree.Nodes.Add(new TreeNode(v.Name) { StateImageKey = v.Visible ? "Checked" : "Unchecked", BackColor = v.Colour, Tag = v.ID });
            }
        }

        public void Clear()
        {
            VisgroupTree.Nodes.Clear();
        }

        private TreeNode GetNodeForVisgroupID(int visgroupId)
        {
            return VisgroupTree.Nodes.OfType<TreeNode>().FirstOrDefault(x => x.Tag is int && (int)x.Tag == visgroupId);
        }

        public int? GetSelectedVisgroup()
        {
            var selected = VisgroupTree.SelectedNode;
            var id = selected == null ? (int?)null : (int)selected.Tag;
            return id;
        }

        public void SetSelectedVisgroup(int visgroupId)
        {
            VisgroupTree.SelectedNode = GetNodeForVisgroupID(visgroupId);
        }

        public void UpdateVisgroupName(int visgroupId, string name)
        {
            var node = GetNodeForVisgroupID(visgroupId);
            if (node != null) node.Text = name;
        }

        public void UpdateVisgroupColour(int visgroupId, Color colour)
        {
            var node = GetNodeForVisgroupID(visgroupId);
            if (node != null) node.BackColor = colour;
        }

        public CheckState GetCheckState(int visgroupId)
        {
            var node = GetNodeForVisgroupID(visgroupId);
            if (node == null || node.StateImageKey == "Unchecked") return CheckState.Unchecked;
            if (node.StateImageKey == "Checked") return CheckState.Checked;
            return CheckState.Indeterminate;
        }

        public void SetCheckState(int visgroupId, CheckState state)
        {
            var node = GetNodeForVisgroupID(visgroupId);
            if (node == null) return;
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
        }

        private void NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (!ShowCheckboxes) return;

            // Only do something if the click is over the image (checkbox)
            var hit = VisgroupTree.HitTest(e.X, e.Y);
            if (hit.Location != TreeViewHitTestLocations.StateImage) return;

            // unchecked -> checked, checked -> unchecked, mixed -> unchecked
            var visible = e.Node.StateImageKey == "Unchecked";
            e.Node.StateImageKey = visible ? "Checked" : "Unchecked";
            OnVisgroupToggled((int) e.Node.Tag, visible ? CheckState.Checked : CheckState.Unchecked);
        }

        private void NodeSelected(object sender, TreeViewEventArgs e)
        {
            OnVisgroupSelected(GetSelectedVisgroup());
        }
    }
}
