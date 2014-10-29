using System;
using System.Drawing;
using System.Windows.Forms;
using Sledge.Gui.Interfaces.Shell;

namespace Sledge.Gui.WinForms.Shell
{
    public class WinFormsToolbarItem : ToolStripButton, IToolbarItem
    {
        public string TextKey { get; set; }
        public Image Icon { set { Image = value; } }

        public event EventHandler Clicked
        {
            add { Click += value; }
            remove { Click -= value; }
        }

        public WinFormsToolbarItem(string textKey, string text)
        {
            text = text ?? UIManager.Manager.StringProvider.Fetch(textKey);

            base.Text = text;
            ToolTipText = text;
            TextKey = textKey;
            DisplayStyle = ToolStripItemDisplayStyle.Image;
        }
    }
}