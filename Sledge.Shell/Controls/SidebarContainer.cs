using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Sledge.Shell.Controls
{
    public partial class SidebarContainer : UserControl
    {
        public SidebarContainer()
        {
            InitializeComponent();

            Resize += CheckSize;
            ContentPanel.Resize += CheckSize;

            CheckSize(null, null);
        }

        public void Add(Control c)
        {
            Insert(c, ContentPanel.Controls.Count);
        }

        public void Insert(Control c, int index)
        {
            SuspendLayout();

            c.Dock = DockStyle.Top;
            c.Resize += DoLayout;
            ContentPanel.Controls.Add(c);
            ContentPanel.Controls.SetChildIndex(c, ContentPanel.Controls.Count - index - 1);

            ResumeLayout(true);
        }

        public int Count()
        {
            return ContentPanel.Controls.Count;
        }

        public void Remove(Control c)
        {
            ContentPanel.Controls.Remove(c);
            DoLayout(null, null);
        }

        public IEnumerable<Control> GetControls()
        {
            return ContentPanel.Controls.OfType<Control>();
        }

        private void DoLayout(object sender, EventArgs eventArgs)
        {
            ContentPanel.Height = ContentPanel.Controls.Cast<Control>().Sum(c => c.Height);
            CheckSize(sender, eventArgs);
        }

        private void CheckSize(object sender, EventArgs e)
        {
            SuspendLayout();
            if (ContentPanel.Height > Height)
            {
                ContentPanel.Size = new Size(Width - SystemInformation.VerticalScrollBarWidth, ContentPanel.Height);
                ScrollBar.Width = SystemInformation.VerticalScrollBarWidth;
                ScrollBar.Maximum = (ContentPanel.Height - Height) + ScrollBar.LargeChange - 1;
                if (ContentPanel.Location.Y + ContentPanel.Height < Height) ContentPanel.Location = new Point(0, Height - ContentPanel.Height);
            }
            else
            {
                ContentPanel.Location = new Point(0, 0);
                ScrollBar.Width = 0;
                ScrollBar.Value = 0;
                ContentPanel.Size = new Size(Width, ContentPanel.Height);
            }
            foreach (Control control in ContentPanel.Controls)
            {
                control.Width = ContentPanel.Width;
            }
            ResumeLayout();
        }

        private void Scrolled(object sender, EventArgs e)
        {
            ContentPanel.Location = new Point(0, -ScrollBar.Value);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            ScrollBar.Value = Math.Max(0, Math.Min(ScrollBar.Value + -ScrollBar.SmallChange*e.Delta/Math.Abs(e.Delta), ScrollBar.Maximum - ScrollBar.LargeChange + 1));
            base.OnMouseWheel(e);
        }
    }
}
