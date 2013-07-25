using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sledge.Editor.UI
{
    public partial class AboutDialog : Form
    {
        public AboutDialog()
        {
            InitializeComponent();

            InfoLabel.Text = "Sledge " + FileVersionInfo.GetVersionInfo(typeof(Editor).Assembly.Location).FileVersion + "\n"
                 + "Created by Daniel Walder - http://logic-and-trick.com \n"
                 + "Open source software - http://github.com/LogicAndTrick/sledge";
        }
    }
}
