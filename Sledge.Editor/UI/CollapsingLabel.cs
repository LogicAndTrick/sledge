using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sledge.Editor.Properties;

namespace Sledge.Editor.UI
{
    public partial class CollapsingLabel : UserControl
    {
        private bool _collapsed;
        private int _cachedHeight;

        public bool Collapsed
        {
            get { return _collapsed; }
            set
            {
                _collapsed = value;
                UpdateCollapsedState();
            }
        }

        public string LabelText
        {
            get { return TextLabel.Text; }
            set { TextLabel.Text = value; }
        }

        public Control ControlToCollapse { get; set; }

        public CollapsingLabel()
        {
            _collapsed = false;
            InitializeComponent();
        }

        private void UpdateCollapsedState()
        {
            if (ControlToCollapse == null) return;
            var temp = ControlToCollapse.Height;
            ControlToCollapse.Height = _collapsed ? 0 : _cachedHeight;
            ArrowImage.Image = _collapsed ? Resources.Arrow_Down : Resources.Arrow_Up;
            _cachedHeight = temp;
        }

        private void LabelClick(object sender, EventArgs e)
        {
            _collapsed = !_collapsed;
            UpdateCollapsedState();
        }
    }
}
