using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sledge.Editor.Tools.VMTools
{
    public partial class StandardControl : UserControl
    {
        public delegate void SplitEventHandler(object sender);
        public delegate void MergeEventHandler(object sender);

        public event SplitEventHandler Split;
        public event MergeEventHandler Merge;

        protected virtual void OnSplit()
        {
            if (Split != null)
            {
                Split(this);
            }
        }

        protected virtual void OnMerge()
        {
            if (Merge != null)
            {
                Merge(this);
            }
        }

        public bool AutomaticallyMerge
        {
            get { return AutoMerge.Checked; }
            set { AutoMerge.Checked = value; }
        }

        public bool SplitEnabled
        {
            get { return SplitButton.Enabled; }
            set { SplitButton.Enabled = value; }
        }

        public StandardControl()
        {
            InitializeComponent();
        }

        private void SplitButtonClicked(object sender, EventArgs e)
        {
            OnSplit();
        }

        private void MergeButtonClicked(object sender, EventArgs e)
        {
            OnMerge();
        }
    }
}
