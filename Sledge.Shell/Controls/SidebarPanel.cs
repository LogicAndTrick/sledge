using System;
using System.Drawing;
using System.Windows.Forms;
using Sledge.Common;

namespace Sledge.Shell.Controls
{
    public class SidebarPanel : Panel
    {
        private bool _hidden;

        public bool Hidden
        {
            get { return _hidden; }
            set
            {
                _hidden = value;
                if (_hidden)
                {
                    Controls.Remove(_panel);
                }
                else
                {
                    Controls.Add(_panel);
                    Controls.SetChildIndex(_panel, 0);
                }
            }
        }

        public override string Text
        {
            get { return _header.Text; }
            set { _header.Text = value; }
        }

        private readonly SidebarHeader _header;
        private readonly Panel _panel;

        public SidebarPanel()
        {
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;

            _hidden = false;
            _header = new SidebarHeader {Text = "This is a test", Expanded = !_hidden, Dock = DockStyle.Top};
            _header.Click += HeaderClicked;

            _panel = new Panel {Dock = DockStyle.Top, AutoSize = true};
            Controls.Add(_panel);
            Controls.Add(_header);

            CreateHandle();
        }

        public void AddControl(Control c)
        {
            c.Dock = DockStyle.Top;
            _panel.Controls.Add(c);
        }

        private void HeaderClicked(object sender, EventArgs e)
        {
            Hidden = !Hidden;
            _header.Expanded = !Hidden;
        }

        private class SidebarHeader : Label
        {
            private bool _expanded;

            public bool Expanded
            {
                get { return _expanded; }
                set
                {
                    _expanded = value;
                    Refresh();
                }
            }

            public SidebarHeader()
            {
                DoubleBuffered = true;
                _expanded = false;
                Height = Font.Height + 12;
                AutoSize = false;
                Padding = new Padding(16, 5, 3, 1);
                SetStyle(ControlStyles.StandardDoubleClick, false);
                Cursor = Cursors.Hand;
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                e.Graphics.DrawLine(SystemPens.ControlDark, 0, 0, Width, 0);
                e.Graphics.DrawLine(SystemPens.ControlLightLight, 0, 1, Width, 1);
                if (_mouseIn)
                {
                    using (var brush = new SolidBrush(BackColor.Darken()))
                    {
                        e.Graphics.FillRectangle(brush, new Rectangle(0, 3, Width, Height - Padding.Vertical));
                    }
                }
                else
                {
                    using (var brush = new SolidBrush(BackColor.Darken(10)))
                    {
                        e.Graphics.FillRectangle(brush, new Rectangle(0, Height - Padding.Vertical + 2, Width, 1));
                    }
                }
                using (var brush = new SolidBrush(ForeColor))
                {
                    e.Graphics.FillPolygon(brush, GetTrianglePoints());
                }

                base.OnPaint(e);
            }

            private bool _mouseIn;

            protected override void OnMouseEnter(EventArgs e)
            {
                _mouseIn = true;
                Refresh();
                base.OnMouseEnter(e);
            }

            protected override void OnMouseLeave(EventArgs e)
            {
                _mouseIn = false;
                Refresh();
                base.OnMouseLeave(e);
            }

            private Point[] GetTrianglePoints()
            {
                if (_expanded)
                {
                    var left = 4;
                    var top = 5 + Padding.Top;
                    return new[]
                    {
                        new Point(left, top),
                        new Point(left + 8, top),
                        new Point(left + 4, top + 4),
                        new Point(left + 3, top + 4)
                    };
                }
                else
                {
                    var left = 6;
                    var top = 2 + Padding.Top;
                    return new[]
                    {
                        new Point(left, top),
                        new Point(left + 4, top + 4),
                        new Point(left + 4, top + 5),
                        new Point(left, top + 9)
                    };
                }
            }
        }
    }
}
