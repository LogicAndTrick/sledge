using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sledge.Editor.UI.Sidebar
{
    class SidebarPanel : Panel
    {
        private Color _headerBackgroundColor = Color.Gray;
        public Color HeaderBackgroundColor
        {
            get { return _headerBackgroundColor; }
            set { _headerBackgroundColor = value; Invalidate(); Update(); }
        }

        private Color _headerForegroundColor = Color.WhiteSmoke;
        public Color HeaderForegroundColor
        {
            get { return _headerForegroundColor; }
            set { _headerForegroundColor = value; Invalidate(); Update(); }
        }

        private bool _hidden;
        public bool Hidden
        {
            get { return _hidden; }
            set {
                _hidden = value;
                Invalidate();
                Update();
            }
        }

        public SidebarPanel()
        {
            DoubleBuffered = true;
            Padding = new Padding(3, 20, 3, 3);
            Text = "This is a test panel";
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var pts = new[]
                              {
                                  new Point(4, 0),
                                  new Point(Width - 4, 0),
                                  new Point(Width - 1, 1),
                                  new Point(Width, 4),
                                  new Point(Width, 20),
                                  new Point(0, 20),
                                  new Point(0, 4),
                                  new Point(1, 1),
                              };
            using (var brush = new SolidBrush(_headerBackgroundColor))
            {
                e.Graphics.FillPolygon(brush, pts);
            }
            using (var pen = new Pen(_headerBackgroundColor))
            {
                e.Graphics.DrawLine(pen, 2, Height - 2, Width - 2, Height - 2);
            }
            using (var pen = new Pen(_headerForegroundColor))
            {
                e.Graphics.DrawPolygon(pen, pts);
            }
            using (var brush = new SolidBrush(_headerForegroundColor))
            {
                e.Graphics.DrawString(Text, Font, brush, 5, 4);
            }
            
            base.OnPaint(e);
        }
    }
}
