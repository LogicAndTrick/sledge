using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Sledge.Shell.Controls
{
    public class ClosableTabControl : TabControl
    {
        public delegate void RequestCloseEventHandler(object sender, int index);

        public event RequestCloseEventHandler RequestClose;

        private void OnRequestClose(int index)
        {
            RequestClose?.Invoke(this, index);
        }

        public ClosableTabControl()
        {
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            ImageList = new ImageList();
            ImageList.Images.Add("Clean", new Bitmap(8, 8));
            ImageList.Images.Add("Dirty", new Bitmap(8, 8));

            TabPages.Add(new TabPage("Tab 1") { ImageKey = "Dirty" });
            TabPages.Add("Tab 2");
            TabPages.Add("Tab 3");
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Render(e.Graphics);
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            ((TabPage) e.Control).ImageIndex = 0;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left && e.Button != MouseButtons.Middle) return;
            for (var i = 0; i < TabPages.Count; i++)
            {
                var rect = e.Button == MouseButtons.Left ? GetCloseRect(i) : GetTabRect(i);
                if (!rect.Contains(e.Location)) continue;
                OnRequestClose(i);
                break;
            }
        }

        // ReSharper disable InconsistentNaming : These are Windows constants, keep the same names

        private const int WM_NULL = 0x0;
        private const int WM_MBUTTONDOWN = 0x0207;
        private const int WM_LBUTTONDOWN = 0x0201;

        // ReSharper enable InconsistentNaming

        protected override void WndProc(ref Message m)
        {
            if (!DesignMode && (m.Msg == WM_LBUTTONDOWN || m.Msg == WM_MBUTTONDOWN))
            {
                var pt = PointToClient(Cursor.Position);
                for (var i = 0; i < TabPages.Count; i++)
                {
                    var rect = m.Msg == WM_LBUTTONDOWN ? GetCloseRect(i) : GetTabRect(i);
                    if (!rect.Contains(pt)) continue;
                    m.Msg = WM_NULL;
                    OnRequestClose(i);
                    break;
                }
            }
            base.WndProc(ref m);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            Invalidate();
        }

        private void Render(Graphics g)
        {
            if (!Visible) return;

            using (var b = new SolidBrush(BackColor))
            {
                g.FillRectangle(b, ClientRectangle);
            }

            var display = new Rectangle(DisplayRectangle.Location, DisplayRectangle.Size);
            if (TabPages.Count == 0) display.Y += 21;

            var border = SystemInformation.Border3DSize.Width;
            display.Inflate(border, border);
            g.DrawLine(SystemPens.ControlDark, display.X, display.Y, display.X + display.Width, display.Y);

            var clip = g.Clip;
            g.SetClip(new Rectangle(display.Left, ClientRectangle.Top, display.Width, ClientRectangle.Height));

            for (var i = 0; i < TabPages.Count; i++) RenderTab(g, i);

            g.Clip = clip;
        }

        private void RenderTab(Graphics g, int index)
        {
            var rect = GetTabRect(index);
            var closeRect = GetCloseRect(index);
            var selected = SelectedIndex == index;
            var tab = TabPages[index];

            var points = new[]
            {
                new Point(rect.Left, rect.Bottom),
                new Point(rect.Left, rect.Top + 3),
                new Point(rect.Left + 3, rect.Top),
                new Point(rect.Right - 3, rect.Top),
                new Point(rect.Right, rect.Top + 3),
                new Point(rect.Right, rect.Bottom),
                new Point(rect.Left, rect.Bottom)
            };

            // Background
            var p = PointToClient(MousePosition);
            var hoverClose = closeRect.Contains(p);
            var hover = rect.Contains(p);
            var backColour = tab.BackColor;

            if (selected) backColour = ControlPaint.Light(backColour, 1);
            else if (hover) backColour = ControlPaint.Light(backColour, 0.8f);

            using (var b = new SolidBrush(backColour))
            {
                g.FillPolygon(b, points);
            }

            // Border
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.DrawPolygon(SystemPens.ControlDark, points);
            if (selected)
            {
                using (var pen = new Pen(tab.BackColor))
                {
                    g.DrawLine(pen, rect.Left, rect.Bottom, rect.Right, rect.Bottom);
                }
            }

            // Icon
            if (tab.ImageKey == "Dirty")
            {
                g.FillEllipse(Brushes.OrangeRed, rect.X + 5, rect.Y + 5, 5, 5);
            }

            // Text
            var sf = new StringFormat(StringFormatFlags.NoWrap);
            var textWidth = (int) g.MeasureString(tab.Text, Font, SizeF.Empty, sf).Width;
            var textLeft = rect.X + 14;
            var textRight = rect.Right - 26;
            var textRect = new Rectangle(textLeft + (textRight - textLeft - textWidth) / 2, rect.Y + 4, rect.Width - 26, rect.Height - 5);
            using (var b = new SolidBrush(tab.ForeColor))
            {
                g.DrawString(tab.Text, Font, b, textRect, sf);
            }

            // Close icon
            using (var pen = new Pen(tab.ForeColor))
            {
                if (hoverClose)
                {
                    g.DrawRectangle(pen, closeRect.Left + 1, closeRect.Top + 1, closeRect.Width - 2, closeRect.Height - 2);
                }
                const int padding = 5;
                g.DrawLine(pen, closeRect.Left + padding, closeRect.Top + padding, closeRect.Right - padding, closeRect.Bottom - padding);
                g.DrawLine(pen, closeRect.Right - padding, closeRect.Top + padding, closeRect.Left + padding, closeRect.Bottom - padding);
            }
        }

        private Rectangle GetCloseRect(int index)
        {
            var rect = GetTabRect(index);
            return new Rectangle(rect.Right - 20, rect.Top + 1 + (rect.Height - 16) / 2, 16, 16);
        }
    }
}
