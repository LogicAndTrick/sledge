using System;
using System.Drawing;
using System.Windows.Forms;
using Sledge.Gui.Shell;

namespace Sledge.Gui.WinForms.Shell
{
    public class WinFormsToolbarItem : ToolStripButton, IToolbarItem
    {
        public string Identifier { get; set; }
        public Bitmap Icon { set { Image = value; } }

        public event EventHandler Clicked
        {
            add { Click += value; }
            remove { Click -= value; }
        }

        public WinFormsToolbarItem(string identifier, string text)
        {
            base.Text = text;
            ToolTipText = text;
            Identifier = identifier;
            DisplayStyle = ToolStripItemDisplayStyle.Image;
        }
    }
}