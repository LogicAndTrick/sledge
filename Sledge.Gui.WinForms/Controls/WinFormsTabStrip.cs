using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sledge.Gui.Attributes;
using Sledge.Gui.Events;
using Sledge.Gui.Interfaces;
using Sledge.Gui.Interfaces.Controls;
using Sledge.Gui.Interfaces.Models;
using Sledge.Gui.Structures;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;
using Size = Sledge.Gui.Structures.Size;

namespace Sledge.Gui.WinForms.Controls
{
    [ControlImplementation("WinForms")]
    public class WinFormsTabStrip : WinFormsControl, ITabStrip
    {
        private readonly WinFormsTabStripInternal _tab;
        private readonly Dictionary<ITab, TabPage> _pages;
        private ITab _selectedTab;
        private int _selectedIndex;

        public ItemList<ITab> Tabs { get; private set; }

        public int NumTabs
        {
            get { return Tabs.Count; }
        }

        public ITab SelectedTab
        {
            get { return _selectedTab; }
            set
            {
                if (_selectedTab == value) return;
                _selectedTab = value;
                OnTabSelected();
            }
        }

        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                if (_selectedIndex == value) return;
                _selectedIndex = value;
                OnTabSelected();
            }
        }

        protected override Size DefaultPreferredSize
        {
            get { return new Size(1000, FontSize * 2); }
        }

        public event TabEventHandler TabCloseRequested;
        public event TabEventHandler TabSelected;

        protected virtual void OnTabSelected()
        {
            var st = _tab.SelectedTab;
            var tab = GetTab(st);
            var handler = TabSelected;
            if (handler != null) handler(this, tab);
        }

        public WinFormsTabStrip() : base(new WinFormsTabStripInternal())
        {
            _tab = (WinFormsTabStripInternal) Control;
            Tabs = new ItemList<ITab>();
            _tab.TabStrip = this;
            _pages = new Dictionary<ITab, TabPage>();
            Tabs.CollectionChanged += CollectionChanged;
            _tab.RequestClose += RequestClose;
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var selTab = SelectedTab;
            var selDel = false;
            if (e.OldItems != null)
            {
                foreach (ITab rem in e.OldItems)
                {
                    if (rem == selTab) selDel = true;
                    var page = _pages[rem];
                    _tab.TabPages.Remove(page);
                }
            }

            if (e.NewItems != null)
            {
                var idx = e.NewStartingIndex;
                foreach (ITab add in e.NewItems)
                {
                    var page = new TabPage(add.Text){ImageIndex = 0};
                    _pages.Add(add, page);
                    _tab.TabPages.Insert(idx, page);
                    idx++;
                }
            }

            if (selDel)
            {
                SelectedTab = Tabs.FirstOrDefault();
            }
        }

        private void RequestClose(object sender, int index)
        {
            var page = _tab.TabPages[index];
            var tab = GetTab(page);
            if (TabCloseRequested != null) TabCloseRequested(this, tab);
        }

        internal ITab GetTab(TabPage page)
        {
            var kvs = _pages.Where(x => x.Value == page).ToList();
            return kvs.Any() ? kvs[0].Key : null;
        }

        public class WinFormsTabStripInternal : TabControl
        {
            public delegate void RequestCloseEventHandler(object sender, int index);

            public event RequestCloseEventHandler RequestClose;

            private void OnRequestClose(int index)
            {
                if (RequestClose != null)
                {
                    RequestClose(this, index);
                }
            }

            internal WinFormsTabStrip TabStrip { get; set; }

            public WinFormsTabStripInternal()
            {
                SetStyle(ControlStyles.UserPaint, true);
                SetStyle(ControlStyles.AllPaintingInWmPaint, true);
                SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
                SetStyle(ControlStyles.ResizeRedraw, true);
                SetStyle(ControlStyles.SupportsTransparentBackColor, true);

                ImageList = new ImageList();
                ImageList.Images.Add(new Bitmap(8, 8));

                // https://stackoverflow.com/questions/1532301/visual-studio-tabcontrol-tabpages-insert-not-working
                var handle = Handle; // Don't remove - the handle must be created for TabPages.Insert() to work
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                Render(e.Graphics);
            }

            protected override void OnControlAdded(ControlEventArgs e)
            {
                // ((TabPage) e.Control).ImageIndex = 0;
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

            private const int WM_NULL = 0x0;
            private const int WM_MBUTTONDOWN = 0x0207;
            private const int WM_LBUTTONDOWN = 0x0201;

            protected override void WndProc(ref Message m)
            {
                if (!DesignMode && (m.Msg == WM_LBUTTONDOWN || m.Msg == WM_MBUTTONDOWN))
                {
                    var pt = PointToClient(Cursor.Position);
                    for (var i = 0; i < TabPages.Count; i++)
                    {
                        var rect = m.Msg == WM_LBUTTONDOWN ? GetCloseRect(i) : GetTabRect(i);
                        if (!rect.Contains(pt)) continue;
                        var tab = TabStrip.GetTab(TabPages[i]);
                        if (tab == null || tab.Closable)
                        {
                            m.Msg = WM_NULL;
                            OnRequestClose(i);
                        }
                        break;
                    }
                }
                base.WndProc(ref m);
            }

            protected override void OnMouseMove(MouseEventArgs e)
            {
                Invalidate();
            }

            private void Render(System.Drawing.Graphics g)
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
                for (var i = 0; i < TabPages.Count; i++)
                {
                    RenderTab(g, i);
                }
                g.Clip = clip;
            }

            private void RenderTab(System.Drawing.Graphics g, int index)
            {
                var rect = GetTabRect(index);
                var closeRect = GetCloseRect(index);
                var selected = SelectedIndex == index;
                var tab = TabPages[index];

                var obj = TabStrip.GetTab(tab);
                var dirty = obj != null && obj.Dirty;
                var closable = obj != null && obj.Closable;

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
                // Text
                var textWidth = (int) g.MeasureString(tab.Text, Font).Width;
                var textLeft = rect.X + 14;
                var textRight = rect.Right - 26;
                var textRect = new Rectangle(textLeft + (textRight - textLeft - textWidth) / 2, rect.Y + 4, rect.Width - 26, rect.Height - 5);
                using (var b = new SolidBrush(tab.ForeColor))
                {
                    if (dirty)
                    {
                        using (var f = new Font(Font, FontStyle.Bold))
                        {
                            g.DrawString(tab.Text, f, b, textRect);
                        }
                    }
                    else
                    {
                        g.DrawString(tab.Text, Font, b, textRect);
                    }
                }

                if (closable)
                {
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
            }

            private Rectangle GetCloseRect(int index)
            {
                var rect = GetTabRect(index);
                return new Rectangle(rect.Right - 20, rect.Top + 1 + (rect.Height - 16) / 2, 16, 16);
            }
        }
    }
}
