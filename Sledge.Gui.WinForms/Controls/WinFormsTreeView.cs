using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Sledge.Gui.Attributes;
using Sledge.Gui.Interfaces;
using Point = System.Drawing.Point;
using Size = Sledge.Gui.Interfaces.Size;
using TreeNode = System.Windows.Forms.TreeNode;

namespace Sledge.Gui.WinForms.Controls
{
    [ControlImplementation("WinForms")]
    public class WinFormsTreeView : WinFormsControl, ITreeView
    {
        private readonly TreeView _tree;
        private Container _components;
        private ImageList _checkboxImages;
        private ITreeModel _model;
        private Dictionary<ITreeNode, System.Windows.Forms.TreeNode> _nodes;

        public bool ShowCheckboxes
        {
            get { return _tree.StateImageList != null; }
            set { _tree.StateImageList = value ? _checkboxImages : null; }
        }

        protected override Size DefaultPreferredSize
        {
            get { return new Size(200, 200); }
        }

        public ITreeModel Model
        {
            get { return _model; }
            set
            {
                if (_model == value) return;
                if (_model != null) UnbindModel(_model);
                _model = value;
                if (_model != null) BindModel(_model);
            }
        }

        private void UnbindModel(ITreeModel model)
        {
            model.NodesAdded -= NodesAdded;
            model.NodesRemoved -= NodesRemoved;
            model.NodesSelected -= NodesSelected;
            model.NodesToggled -= NodesToggled;
        }

        private void BindModel(ITreeModel model)
        {
            model.NodesAdded += NodesAdded;
            model.NodesRemoved += NodesRemoved;
            model.NodesSelected += NodesSelected;
            model.NodesToggled += NodesToggled;
        }

        private void NodesAdded(object sender, List<ITreeNode> nodes)
        {
            foreach (var tn in nodes)
            {
                var node = new TreeNode(tn.Text)
                {
                    StateImageKey = GetImageKey(tn)
                };
                var parent = Model.GetParentNode(tn);
                var collection = parent != null ? _nodes[parent].Nodes : _tree.Nodes;
                collection.Add(node);
                _nodes.Add(tn, node);
            }
        }

        private string GetImageKey(ITreeNode node)
        {
            if (node.Indeterminate) return "Mixed";
            return node.Checked ? "Checked" : "Unchecked";
        }

        private void NodesRemoved(object sender, List<ITreeNode> nodes)
        {
            foreach (var tn in nodes)
            {
                var parent = Model.GetParentNode(tn);
                if (parent == null) return;
                _nodes[parent].Nodes.Remove(_nodes[tn]);
                _nodes.Remove(tn);
            }
        }

        private void NodesSelected(object sender, List<ITreeNode> nodes)
        {
            _tree.SelectedNode = nodes.Any() ? _nodes[nodes.Last()] : null;
        }

        private void NodesToggled(object sender, List<ITreeNode> nodes)
        {
            foreach (var tn in nodes)
            {
                var node = _nodes[tn];
                if (node != null) node.StateImageKey = GetImageKey(tn);
            }
        }

        public WinFormsTreeView() : base(new TreeView())
        {
            _tree = (TreeView) Control;
            _nodes = new Dictionary<ITreeNode, TreeNode>();
            Model = new TreeModel();
            InitialiseImages();
        }

        private void InitialiseImages()
        {
            _components = new Container();
            _checkboxImages = new ImageList(_components);

            /* http://www.codeproject.com/script/Articles/ViewDownloads.aspx?aid=202435 */
            _checkboxImages.Images.Add("Unchecked", GetCheckboxBitmap(CheckBoxState.UncheckedNormal));
            _checkboxImages.Images.Add("Checked", GetCheckboxBitmap(CheckBoxState.CheckedNormal));
            _checkboxImages.Images.Add("Mixed", GetCheckboxBitmap(CheckBoxState.MixedNormal));

            _checkboxImages.Images.Add("UncheckedDisabled", GetCheckboxBitmap(CheckBoxState.UncheckedDisabled));
            _checkboxImages.Images.Add("CheckedDisabled", GetCheckboxBitmap(CheckBoxState.CheckedDisabled));
            _checkboxImages.Images.Add("MixedDisabled", GetCheckboxBitmap(CheckBoxState.MixedDisabled));
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
    }
}