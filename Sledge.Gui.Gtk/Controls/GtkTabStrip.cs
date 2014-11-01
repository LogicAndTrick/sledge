using System;
using System.Collections.Specialized;
using System.Linq;
using Gdk;
using Gtk;
using Sledge.Gui.Attributes;
using Sledge.Gui.Events;
using Sledge.Gui.Interfaces.Controls;
using Sledge.Gui.Interfaces.Models;
using Sledge.Gui.Structures;
using Layout = Pango.Layout;

namespace Sledge.Gui.Gtk.Controls
{
    [ControlImplementation("GTK")]
    public class GtkTabStrip : GtkControl, ITabStrip
    {
        private ITab _selectedTab;
        private readonly HBox _hbox;

        public ITab SelectedTab
        {
            get { return _selectedTab; }
            set
            {
                if (_selectedTab == value) return;
                if (_selectedTab != null) _selectedTab.Selected = false;
                _selectedTab = value;
                if (_selectedTab != null) _selectedTab.Selected = true;
                OnTabSelected(_selectedTab);
            }
        }

        public int SelectedIndex
        {
            get { return Tabs.IndexOf(SelectedTab); }
            set { SelectedTab = value >= 0 && value < NumTabs ? Tabs[value] : null; }
        }

        public int NumTabs { get { return Tabs.Count; } }
        public ItemList<ITab> Tabs { get; private set; }

        public event TabEventHandler TabCloseRequested;

        protected virtual void OnTabCloseRequested(ITab tab)
        {
            if (TabCloseRequested != null) TabCloseRequested(this, tab);
        }

        public event TabEventHandler TabSelected;

        protected virtual void OnTabSelected(ITab tab)
        {
            Control.QueueDraw();
            if (TabSelected != null) TabSelected(this, tab);
        }

        public GtkTabStrip() : base(new TabBox())
        {
            _hbox = (HBox) Control;
            Tabs = new ItemList<ITab>();
            Tabs.CollectionChanged += CollectionChanged;
            Control.HeightRequest = 22;
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
                    RemoveTab(rem);
                }
            }

            if (e.NewItems != null)
            {
                var idx = e.NewStartingIndex;
                foreach (ITab add in e.NewItems)
                {
                    AddTab(idx, add);
                }
            }

            if (selDel)
            {
                SelectedTab = Tabs.FirstOrDefault();
            }

            Control.ShowAll();
        }

        private void AddTab(int index, ITab tab)
        {
            var widget = new Tab(this, tab);
            _hbox.PackStart(widget, false, false, 0);
            _hbox.ReorderChild(widget, index);
        }

        private void RemoveTab(ITab tab)
        {
            var widget = _hbox.Children.OfType<Tab>().FirstOrDefault(x => x.Instance == tab);
            if (widget != null)
            {
                _hbox.Remove(widget);
                widget.Dispose();
            }
        }

        private class TabBox : HBox
        {
            protected override void OnSizeAllocated(Gdk.Rectangle allocation)
            {
                base.OnSizeAllocated(allocation);

                if (Children.Length <= 0) return;

                var last = Children[Children.Length - 1];
                float max = last.Allocation.Width + last.Allocation.X;
                if (max < allocation.Width) return;

                var x = allocation.X;
                foreach (var tab in Children)
                {
                    var pc = tab.Allocation.Width / max;
                    var wid = (int) Math.Floor(pc * allocation.Width);
                    tab.SizeAllocate(new Gdk.Rectangle(x, allocation.Y, wid, allocation.Height));
                    x += wid;
                }
            }

            protected override bool OnExposeEvent(EventExpose evnt)
            {
                GdkWindow.DrawLine(Style.TextGC(StateType.Normal), 0, Allocation.Bottom, Allocation.Width, Allocation.Bottom);
                return base.OnExposeEvent(evnt);
            }
        }

        private class Tab : EventBox
        {
            public GtkTabStrip TabStrip { get; set; }
            public ITab Instance { get; private set; }

            private Layout _layout;
            private int _layoutWidth;
            private int _layoutHeight;

            public Tab(GtkTabStrip tabStrip, ITab tab)
            {
                TabStrip = tabStrip;
                Instance = tab;
                Events = EventMask.PointerMotionMask | EventMask.ButtonPressMask;
            }

            protected override void OnRealized()
            {
                Instance.PropertyChanged += (sender, args) => Refresh();
                base.OnRealized();
            }

            private void Refresh()
            {
                if (_layout == null) _layout = new Layout(PangoContext);
                if (Instance.Dirty) _layout.SetMarkup("<strong>" + Instance.Text + "</strong>");
                else _layout.SetText(Instance.Text);
                _layout.GetPixelSize(out _layoutWidth, out _layoutHeight);
                if (Instance.Closable) _layoutWidth += 40;
            }

            public override void Dispose()
            {
                if (_layout != null) _layout.Dispose();
                base.Dispose();
            }

            protected override void OnSizeRequested(ref Requisition requisition)
            {
                Refresh();
                requisition.Width = _layoutWidth;
                requisition.Height = _layoutHeight;
            }

            private Gdk.Rectangle GetCloseRectangle()
            {
                return new Gdk.Rectangle(Allocation.Width - 18, 5, 12, 12);
            }

            protected override bool OnEnterNotifyEvent(EventCrossing evnt)
            {
                QueueDraw();
                return base.OnEnterNotifyEvent(evnt);
            }

            protected override bool OnLeaveNotifyEvent(EventCrossing evnt)
            {
                QueueDraw();
                return base.OnLeaveNotifyEvent(evnt);
            }

            protected override bool OnMotionNotifyEvent(EventMotion evnt)
            {
                QueueDraw();
                return base.OnMotionNotifyEvent(evnt);
            }
            
            protected override bool OnButtonPressEvent(EventButton evnt)
            {
                var rect = GetCloseRectangle();
                var hoveringClose = rect.Contains((int) evnt.X, (int) evnt.Y);
                if (evnt.Button == 2 || (evnt.Button == 1 && hoveringClose))
                {
                    TabStrip.OnTabCloseRequested(Instance);
                    return true;
                }
                TabStrip.SelectedTab = Instance;
                return base.OnButtonPressEvent(evnt);
            }

            protected override bool OnExposeEvent(EventExpose evnt)
            {
                if (_layout == null) Refresh();

                int x, y;
                GetPointer(out x, out y);
                var rect = GetCloseRectangle();
                var hoveringClose = rect.Contains(x, y);
                var hovering = x >= 0 && x < Allocation.Width && y >= 0 && y < Allocation.Height;

                var bg = Instance.Selected
                    ? Style.BackgroundGC(StateType.Selected)
                    : hovering
                        ? Style.LightGC(StateType.Normal)
                        : Style.BackgroundGC(StateType.Normal);
                var fg = Instance.Selected
                    ? Style.TextGC(StateType.Selected)
                    : Style.TextGC(StateType.Normal);

                GdkWindow.DrawRectangle(bg, true, 0, 0, Allocation.Width - 1, Allocation.Height - 1);
                GdkWindow.DrawLayout(fg, 5, 4, _layout);
                GdkWindow.DrawRectangle(Style.TextGC(StateType.Normal), false, 0, 0, Allocation.Width - 1, Allocation.Height - 1);
                if (Instance.Closable)
                {
                    GdkWindow.DrawRectangle(bg, true, rect.X - 3, 1, Allocation.Width - rect.X + 2, Allocation.Height - 2);
                    if (hoveringClose) GdkWindow.DrawRectangle(fg, false, rect);
                    GdkWindow.DrawLine(fg, rect.Left + 3, rect.Top + 3, rect.Right - 2, rect.Bottom - 2);
                    GdkWindow.DrawLine(fg, rect.Left + 3, rect.Bottom - 2, rect.Right - 2, rect.Top + 3);
                }
                return true;
            }
        }
    }
}
