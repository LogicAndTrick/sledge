using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sledge.Editor.UI
{
    public partial class ViewportWindow : Form
    {
        public ViewportWindow(TableSplitConfiguration configuration)
        {
            InitializeComponent();

            SplitControl.Configuration = configuration;
        }

        public TableSplitControl TableSplitControl
        {
            get { return SplitControl; }
        }
    }
}
