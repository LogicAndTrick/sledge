using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sledge.Editor.UI;

namespace Sledge.Sandbox
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            var qsc = new QuadSplitControl { Dock = DockStyle.Fill };
            var qp1 = new Panel { BackColor = Color.Green, Dock = DockStyle.Fill};
            var qp2 = new Panel { BackColor = Color.Gray, Dock = DockStyle.Fill };
            var qp3 = new Panel { BackColor = Color.Blue, Dock = DockStyle.Fill };
            var qp4 = new Panel { BackColor = Color.Yellow, Dock = DockStyle.Fill };
            qsc.Controls.Add(qp1, 0, 0);
            qsc.Controls.Add(qp2, 1, 0);
            qsc.Controls.Add(qp3, 0, 1);
            qsc.Controls.Add(qp4, 1, 1);
            Controls.Add(qsc);
        }
    }
}
