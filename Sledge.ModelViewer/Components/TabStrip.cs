//
// TabStrip.cs
//
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
//
// Copyright (c) 2014 Xamarin Inc. (http://xamarin.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

// Modified 2014 Daniel Walder http://logic-and-trick.com

using System.Linq;
using Gdk;
using Gtk;
using System;
using Mono.TextEditor;
using System.Collections.Generic;
using Cairo;
using MonoDevelop.Components;
using MonoDevelop.Core;
using Pango;
using Xwt.Motion;
using MonoDevelop.Ide.Gui;
using CairoHelper = Gdk.CairoHelper;
using Color = Cairo.Color;
using Context = Cairo.Context;
using Image = Xwt.Drawing.Image;
using Layout = Pango.Layout;
using Point = Gdk.Point;
using Rectangle = Gdk.Rectangle;
using Scale = Pango.Scale;

namespace Sledge.ModelViewer.Components
{
    internal class TabStrip : EventBox, IAnimatable
    {
        private static readonly Image TabbarPrevImage = Image.FromResource("tabbar-prev-light-12.png");
        private static readonly Image TabbarNextImage = Image.FromResource("tabbar-next-light-12.png");

        private readonly List<Widget> _children = new List<Widget>();
        private readonly List<DockNotebookTab> _tabs = new List<DockNotebookTab>();
        private int _currentTabIndex;
        private int _highlightedTabIndex;
        private bool _overCloseButton;
        private bool _buttonPressedOnTab;
        private int _tabStartX, _tabEndX;

        private readonly MouseTracker _tracker;

        private bool _draggingTab;
        private int _dragX;
        private int _dragOffset;
        private double _dragXProgress;

        private int _renderOffset;
        private int _targetOffset;
        private int _animationTarget;

        private readonly Dictionary<int, DockNotebookTab> _closingTabs;

        private readonly Button _previousButton;
        private readonly Button _nextButton;
        private readonly MenuButton _dropDownButton;

        private const int TopBarPadding = 3;
        private const int BottomBarPadding = 3;
        private const int LeftRightPadding = 10;
        private const int TopPadding = 8;
        private const int BottomPadding = 8;
        private const int LeftBarPadding = 58;
        private const int VerticalTextSize = 11;
        private const int TabSpacing = -1;
        private const int Radius = 2;
        private const int LeanWidth = 18;
        private const int CloseButtonSize = 14;

        private const int TextOffset = 1;

        // Vertically aligns the close image(s) with the tab label.
        private const int CloseImageTopOffset = 3;

        private int TabWidth { get; set; }

        private int LastTabWidthAdjustment { get; set; }

        private int _targetWidth;

        private int _totalHeight;

        public int BarHeight
        {
            get { return _totalHeight - BottomBarPadding + 1; }
        }

        private int lastDragX;

        private int TargetWidth
        {
            get { return _targetWidth; }
            set
            {
                _targetWidth = value;
                if (TabWidth != value)
                {
                    this.Animate("TabWidth",
                        f => TabWidth = (int) f,
                        TabWidth,
                        value,
                        easing: Easing.CubicOut);
                }
            }
        }

        public bool NavigationButtonsVisible
        {
            get { return _children.Contains(_previousButton); }
            set
            {
                if (value == NavigationButtonsVisible) return;
                if (value)
                {
                    _children.Add(_nextButton);
                    _children.Add(_previousButton);
                    OnSizeAllocated(Allocation);
                    _previousButton.ShowAll();
                    _nextButton.ShowAll();
                }
                else
                {
                    _children.Remove(_previousButton);
                    _children.Remove(_nextButton);
                    OnSizeAllocated(Allocation);
                }
            }
        }

        private void OnAddTab(int index)
        {
            
        }

        private void OnReorderTab(int index)
        {

        }

        private void OnRemoveTab(int index)
        {

        }

        private void OnSelectTab(int index)
        {

        }

        private void OnShowContextMenu(int index, EventButton evnt)
        {

        }

        private void OnDoubleClickTab(int index, EventButton evnt)
        {

        }

        public TabStrip()
        {
            _tabs = new List<DockNotebookTab>();
            TabWidth = 125;
            TargetWidth = 125;
            _tracker = new MouseTracker(this);
            GtkWorkarounds.FixContainerLeak(this);

            WidgetFlags |= WidgetFlags.AppPaintable;
            Events |= EventMask.PointerMotionMask | EventMask.LeaveNotifyMask | EventMask.ButtonPressMask;

            var arr = new ImageView(TabbarPrevImage) {HeightRequest = 10, WidthRequest = 10};
            _previousButton = new Button(arr) {Relief = ReliefStyle.None, CanDefault = false, CanFocus = false};

            arr = new ImageView(TabbarNextImage) {HeightRequest = 10, WidthRequest = 10};
            _nextButton = new Button(arr) {Relief = ReliefStyle.None, CanDefault = false, CanFocus = false};

            _dropDownButton = new MenuButton {Relief = ReliefStyle.None, CanDefault = false, CanFocus = false};

            _previousButton.ShowAll();
            _nextButton.ShowAll();
            _dropDownButton.ShowAll();

            _previousButton.Name = "MonoDevelop.DockNotebook.BarButton";
            _nextButton.Name = "MonoDevelop.DockNotebook.BarButton";
            _dropDownButton.Name = "MonoDevelop.DockNotebook.BarButton";

            _previousButton.Parent = this;
            _nextButton.Parent = this;
            _dropDownButton.Parent = this;

            _children.Add(_previousButton);
            _children.Add(_nextButton);
            _children.Add(_dropDownButton);

            _nextButton.Clicked += (sender, e) =>
            {
                _currentTabIndex = (_currentTabIndex + 1) % _tabs.Count;
                OnSelectTab(_currentTabIndex);
                Update();
            };
            _previousButton.Clicked += (sender, e) =>
            {
                _currentTabIndex = ((_currentTabIndex - 1) + _tabs.Count) % _tabs.Count;
                OnSelectTab(_currentTabIndex);
                Update();
            };

            _tracker.HoveredChanged += (sender, e) =>
            {
                if (_tracker.Hovered) return;
                SetHighlightedTab(-1);
                UpdateTabWidth(_tabEndX - _tabStartX);
                QueueDraw();
            };

            _closingTabs = new Dictionary<int, DockNotebookTab>();
        }

        private void UpdateIndexes(int startIndex)
        {
            for (var n = startIndex; n < _tabs.Count; n++)
                _tabs[n].Index = n;
        }

        public void AddTab(string text, bool markup = false, bool dirty = false)
        {
            InsertTab(-1, text, markup, dirty);
        }

        public void InsertTab(int index, string text, bool markup = false, bool dirty = false)
        {
            var tab = new DockNotebookTab(this);
            if (markup) tab.Markup = text;
            else tab.Text = text;
            tab.Dirty = dirty;

            if (index == -1)
            {
                _tabs.Add(tab);
                tab.Index = _tabs.Count - 1;
            }
            else
            {
                _tabs.Insert(index, tab);
                tab.Index = index;
                UpdateIndexes(index + 1);
            }

            if (_tabs.Count == 1)
            {
                _currentTabIndex = 0;
                OnSelectTab(_currentTabIndex);
            }

            StartOpenAnimation(tab);
            Update();
            _dropDownButton.Sensitive = _tabs.Count > 0;

            OnAddTab(tab.Index);
        }

        public void ReorderTab(int tab, int index)
        {
            if (tab == index || tab < 0 || tab >= _tabs.Count || index < 0 || index > _tabs.Count) return;
            if (tab > index)
            {
                var ins = _tabs[tab];
                _tabs.RemoveAt(tab);
                _tabs.Insert(index, ins);
            }
            else
            {
                var ins = _tabs[tab];
                _tabs.Insert(index + 1, ins);
                _tabs.RemoveAt(tab);
            }
            UpdateIndexes(Math.Min(tab, index));
            if (_currentTabIndex == tab)
            {
                _currentTabIndex = index;
                OnSelectTab(_currentTabIndex);
            }
            Update();

            OnReorderTab(index);
        }

        public void RemoveTab(int index)
        {
            if (index < 0 || index >= _tabs.Count) return;
            var tab = _tabs[index];
            StartCloseAnimation(tab);
            if (index == _currentTabIndex)
            {
                if (!_tabs.Any()) _currentTabIndex = -1;
                if (index > 0) _currentTabIndex = index - 1;
                OnSelectTab(_currentTabIndex);
            }

            _tabs.RemoveAt(index);
            UpdateIndexes(index);
            
            Update();
            _dropDownButton.Sensitive = _tabs.Count > 0;

            OnRemoveTab(index);
        }

        void IAnimatable.BatchBegin()
        {
        }

        void IAnimatable.BatchCommit()
        {
            QueueDraw();
        }

        private void StartOpenAnimation(DockNotebookTab tab)
        {
            tab.WidthModifier = 0;
            new Animation(f => tab.WidthModifier = f)
                .AddConcurrent(new Animation(f => tab.Opacity = f), 0.0d, 0.2d)
                .Commit(tab, "Open", easing: Easing.CubicInOut);
        }

        private void StartCloseAnimation(DockNotebookTab tab)
        {
            _closingTabs[tab.Index] = tab;
            new Animation(f => tab.WidthModifier = f, tab.WidthModifier, 0)
                .AddConcurrent(new Animation(f => tab.Opacity = f, tab.Opacity, 0), 0.8d)
                .Commit(tab, "Closing",
                    easing: Easing.CubicOut,
                    finished: (f, a) =>
                    {
                        if (!a) _closingTabs.Remove(tab.Index);
                    });
        }

        protected override void ForAll(bool includeInternals, Callback callback)
        {
            base.ForAll(includeInternals, callback);
            foreach (var c in _children.ToArray())
            {
                callback(c);
            }
        }

        protected override void OnRemoved(Widget widget)
        {
            _children.Remove(widget);
        }

        protected override void OnSizeAllocated(Rectangle allocation)
        {
            _tabStartX = (NavigationButtonsVisible ? LeftBarPadding : 0) + LeanWidth / 2;
            _tabEndX = allocation.Width - _dropDownButton.SizeRequest().Width;

            var height = Math.Max(0, allocation.Height - BottomBarPadding);

            _previousButton.SizeAllocate(new Rectangle(0, 0, LeftBarPadding / 2, height));
            _nextButton.SizeAllocate(new Rectangle(LeftBarPadding / 2, 0, LeftBarPadding / 2, height));
            _dropDownButton.SizeAllocate(new Rectangle(_tabEndX, allocation.Y, _dropDownButton.SizeRequest().Width, height));

            base.OnSizeAllocated(allocation);
            Update();
        }

        protected override void OnSizeRequested(ref Requisition requisition)
        {
            base.OnSizeRequested(ref requisition);
            requisition.Height = _totalHeight;
            requisition.Width = 0;
        }

        private void SetHighlightedTab(int index)
        {
            if (_highlightedTabIndex == index) return;

            if (_highlightedTabIndex >= 0 && _highlightedTabIndex < _tabs.Count)
            {
                var tmp = _tabs[_highlightedTabIndex];
                tmp.Animate("Glow", f => tmp.GlowStrength = f, tmp.GlowStrength, 0);
            }

            if (index >= 0 && index < _tabs.Count)
            {
                var tab = _tabs[index];
                tab.Animate("Glow", f => tab.GlowStrength = f, tab.GlowStrength, 1);
            }
            else
            {
                index = -1;
            }

            _highlightedTabIndex = index;
            QueueDraw();
        }

        private bool _mouseHasLeft;

        protected override bool OnLeaveNotifyEvent(EventCrossing evnt)
        {
            if (_draggingTab && !_mouseHasLeft) _mouseHasLeft = true;
            return base.OnLeaveNotifyEvent(evnt);
        }

        private Rectangle GetScreenRect()
        {
            int ox, oy;
            ParentWindow.GetOrigin(out ox, out oy);
            var alloc = Allocation;
            alloc.X += ox;
            alloc.Y += oy;
            return alloc;
        }

        protected override bool OnMotionNotifyEvent(EventMotion evnt)
        {
            if (_draggingTab && _mouseHasLeft)
            {
                var sr = GetScreenRect();
                sr.Height = BarHeight;
                sr.Inflate(30, 30);

                int x, y;
                Display.Default.GetPointer(out x, out y);

                if (x < sr.Left || x > sr.Right || y < sr.Top || y > sr.Bottom)
                {
                    _draggingTab = false;
                    _mouseHasLeft = false;
                }
            }

            string newTooltip = null;

            if (!_draggingTab)
            {
                var t = FindTab((int) evnt.X, (int) evnt.Y);

                // If the user clicks and drags on the 'x' which closes the current
                // tab we can end up with a null tab here
                if (t < 0) return base.OnMotionNotifyEvent(evnt);

                SetHighlightedTab(t);

                var newOver = IsOverCloseButton(_tabs[t], (int) evnt.X, (int) evnt.Y);
                if (newOver != _overCloseButton)
                {
                    _overCloseButton = newOver;
                    QueueDraw();
                }

                var tab = _tabs[t];
                if (!_overCloseButton && !_draggingTab && _buttonPressedOnTab)
                {
                    _draggingTab = true;
                    _mouseHasLeft = false;
                    _dragXProgress = 1.0f;
                    var x = (int) evnt.X;
                    _dragOffset = x - tab.Allocation.X;
                    _dragX = x - _dragOffset;
                    lastDragX = (int) evnt.X;
                }
                else
                {
                    newTooltip = tab.Tooltip;
                }
            }
            else if (evnt.State.HasFlag(ModifierType.Button1Mask))
            {
                _dragX = (int) evnt.X - _dragOffset;
                QueueDraw();

                var t = FindTab((int) evnt.X, TopPadding + 3);
                if (t < 0)
                {
                    var last = _tabs.Last();
                    if (_dragX > last.Allocation.Right) t = _tabs.Count - 1;
                    if (_dragX < 0) t = 0;
                }
                if (t >= 0 && t != _currentTabIndex && (
                    ((int) evnt.X > lastDragX && t > _currentTabIndex) ||
                    ((int) evnt.X < lastDragX && t < _currentTabIndex)))
                {
                    var tab = _tabs[t];
                    tab.SaveAllocation();
                    tab.SaveStrength = 1;
                    ReorderTab(_currentTabIndex, t);

                    tab.Animate("TabMotion",
                        f => tab.SaveStrength = f,
                        1.0f,
                        0.0f,
                        easing: Easing.CubicInOut);
                }
                lastDragX = (int) evnt.X;
            }

            if (newTooltip != null && TooltipText != null && TooltipText != newTooltip) TooltipText = null;
            else TooltipText = newTooltip;

            return base.OnMotionNotifyEvent(evnt);
        }

        private bool _overCloseOnPress;
        private bool _allowDoubleClick;

        protected override bool OnButtonPressEvent(EventButton evnt)
        {
            var t = FindTab((int) evnt.X, (int) evnt.Y);
            if (t >= 0)
            {
                if (evnt.IsContextMenuButton())
                {
                    OnShowContextMenu(t, evnt);
                    return true;
                }
                // Don't select the tab if we are clicking the close button
                if (IsOverCloseButton(_tabs[t], (int) evnt.X, (int) evnt.Y))
                {
                    _overCloseOnPress = true;
                    return true;
                }
                _overCloseOnPress = false;

                if (evnt.Type == EventType.TwoButtonPress)
                {
                    if (_allowDoubleClick)
                    {
                        OnDoubleClickTab(t, evnt);
                        _buttonPressedOnTab = false;
                    }
                    return true;
                }
                if (evnt.Button == 2)
                {
                    RemoveTab(t);
                    return true;
                }

                _buttonPressedOnTab = true;
                _currentTabIndex = t;
                OnSelectTab(_currentTabIndex);
                return true;
            }
            _buttonPressedOnTab = true;
            QueueDraw();
            return base.OnButtonPressEvent(evnt);
        }

        protected override bool OnButtonReleaseEvent(EventButton evnt)
        {
            _buttonPressedOnTab = false;

            if (!_draggingTab && _overCloseOnPress)
            {
                var t = FindTab((int) evnt.X, (int) evnt.Y);
                if (t >= 0 && IsOverCloseButton(_tabs[t], (int) evnt.X, (int) evnt.Y))
                {
                    RemoveTab(t);
                    _allowDoubleClick = false;
                    return true;
                }
            }
            _overCloseOnPress = false;
            _allowDoubleClick = true;
            if (_dragX != 0)
            {
                this.Animate("EndDrag",
                    f => _dragXProgress = f,
                    1.0d,
                    0.0d,
                    easing: Easing.CubicOut,
                    finished: (f, a) => _draggingTab = false);
            }
            QueueDraw();
            return base.OnButtonReleaseEvent(evnt);
        }

        protected override void OnUnrealized()
        {
            // Cancel drag operations and animations
            _buttonPressedOnTab = false;
            _overCloseOnPress = false;
            _allowDoubleClick = true;
            _draggingTab = false;
            _dragX = 0;
            this.AbortAnimation("EndDrag");
            base.OnUnrealized();
        }

        private int FindTab(int x, int y)
        {
            // we will not actually draw anything, just do bounds checking
            using (var context = CairoHelper.Create(GdkWindow))
            {
                context.Rectangle(_tabStartX - LeanWidth / 2, y, _tabEndX - _tabStartX + LeanWidth, Allocation.Height);
                if (!context.InFill(x, y)) return -1;
                context.NewPath();

                if (_currentTabIndex >= 0 && _currentTabIndex < _tabs.Count)
                {
                    var current = _tabs[_currentTabIndex];
                    LayoutTabBorder(context, Allocation, current.Allocation.Width, current.Allocation.X, 0, false);
                    if (context.InFill(x, y)) return _currentTabIndex;
                }

                context.NewPath();
                for (var i = 0; i < _tabs.Count; i++)
                {
                    var tab = _tabs[i];
                    LayoutTabBorder(context, Allocation, tab.Allocation.Width, tab.Allocation.X, 0, false);
                    if (context.InFill(x, y)) return i;
                    context.NewPath();
                }
            }
            return -1;
        }

        private static bool IsOverCloseButton(DockNotebookTab tab, int x, int y)
        {
            return tab != null && tab.CloseButtonAllocation.Contains(x, y);
        }

        public void Update()
        {
            if (!_tracker.Hovered)
            {
                UpdateTabWidth(_tabEndX - _tabStartX);
            }
            else if (_closingTabs.ContainsKey(_tabs.Count))
            {
                UpdateTabWidth(_closingTabs[_tabs.Count].Allocation.Right - _tabStartX, true);
            }
            QueueDraw();
        }

        private void UpdateTabWidth(int width, bool adjustLast = false)
        {
            if (_tabs.Any()) TargetWidth = Clamp(width / _tabs.Count, 50, 200);

            if (adjustLast)
            {
                // adjust to align close buttons properly
                LastTabWidthAdjustment = width - (TargetWidth * _tabs.Count) + 1;
                LastTabWidthAdjustment = Math.Abs(LastTabWidthAdjustment) < 50 ? LastTabWidthAdjustment : 0;
            }
            else
            {
                LastTabWidthAdjustment = 0;
            }
            if (!IsRealized)
                TabWidth = TargetWidth;
        }

        private static int Clamp(int val, int min, int max)
        {
            return Math.Max(min, Math.Min(max, val));
        }

        private void DrawBackground(Context ctx, Rectangle region)
        {
            var h = region.Height;
            ctx.Rectangle(0, 0, region.Width, h);
            using (var gr = new LinearGradient(0, 0, 0, h))
            {
                gr.AddColorStop(0, Styles.TabBarActiveGradientStartColor);
                gr.AddColorStop(1, Styles.TabBarActiveGradientEndColor);
                ctx.SetSource(gr);
                ctx.Fill();
            }

            ctx.MoveTo(region.X, 0.5);
            ctx.LineTo(region.Right + 1, 0.5);
            ctx.LineWidth = 1;
            ctx.SetSourceColor(Styles.TabBarGradientShadowColor);
            ctx.Stroke();
        }

        private int GetRenderOffset()
        {
            var tabArea = _tabEndX - _tabStartX;
            if (_currentTabIndex >= 0)
            {
                var normalizedArea = (tabArea / TargetWidth) * TargetWidth;
                var maxOffset = Math.Max(0, (_tabs.Count * TargetWidth) - normalizedArea);

                var distanceToTabEdge = TargetWidth * _currentTabIndex;
                var window = normalizedArea - TargetWidth;
                _targetOffset = Math.Min(maxOffset, Clamp(_renderOffset, distanceToTabEdge - window, distanceToTabEdge));

                if (_targetOffset != _animationTarget)
                {
                    this.Animate("ScrollTabs",
                        easing: Easing.CubicOut,
                        start: _renderOffset,
                        end: _targetOffset,
                        callback: f => _renderOffset = (int) f);
                    _animationTarget = _targetOffset;
                }
            }

            return _tabStartX - _renderOffset;
        }

        private Action<Context> DrawClosingTab(int index, Rectangle region, out int width)
        {
            width = 0;
            if (_closingTabs.ContainsKey(index))
            {
                var closingTab = _closingTabs[index];
                width = (int) (closingTab.WidthModifier * TabWidth);
                var tmp = width;
                return
                    c =>
                        DrawTab(c, closingTab, Allocation, new Rectangle(region.X, region.Y, tmp, region.Height), false, false, false,
                            CreateTabLayout(closingTab));
            }
            return c => {
            };
        }

        private void Draw(Context ctx)
        {
            var tabArea = _tabEndX - _tabStartX;
            var x = GetRenderOffset();
            const int y = 0;
            var n = 0;
            Action<Context> drawActive = c => {
            };
            var drawCommands = new List<Action<Context>>();
            for (; n < _tabs.Count; n++)
            {
                if (x + TabWidth < _tabStartX)
                {
                    x += TabWidth;
                    continue;
                }

                if (x > _tabEndX)
                    break;

                int closingWidth;
                var cmd = DrawClosingTab(n, new Rectangle(x, y, 0, Allocation.Height), out closingWidth);
                drawCommands.Add(cmd);
                x += closingWidth;

                var tab = _tabs[n];
                bool active = n == _currentTabIndex;

                var width = Math.Min(TabWidth, Math.Max(50, _tabEndX - x - 1));
                if (tab == _tabs.Last()) width += LastTabWidthAdjustment;
                width = (int) (width * tab.WidthModifier);

                if (active)
                {
                    var tmp = x;
                    drawActive = c => DrawTab(c, tab, Allocation, new Rectangle(tmp, y, width, Allocation.Height), true, true, _draggingTab, CreateTabLayout(tab));
                    tab.Allocation = new Rectangle(tmp, Allocation.Y, width, Allocation.Height);
                }
                else
                {
                    var tmp = x;
                    bool highlighted = n == _highlightedTabIndex;

                    if (tab.SaveStrength > 0.0f)
                    {
                        tmp = (int) (tab.SavedAllocation.X + (tmp - tab.SavedAllocation.X) * (1.0f - tab.SaveStrength));
                    }

                    drawCommands.Add(c => DrawTab(c, tab, Allocation, new Rectangle(tmp, y, width, Allocation.Height), highlighted, false, false, CreateTabLayout(tab)));
                    tab.Allocation = new Rectangle(tmp, Allocation.Y, width, Allocation.Height);
                }

                x += width;
            }

            var allocation = Allocation;
            int tabWidth;
            drawCommands.Add(DrawClosingTab(n, new Rectangle(x, y, 0, allocation.Height), out tabWidth));
            drawCommands.Reverse();

            DrawBackground(ctx, allocation);

            // Draw breadcrumb bar header
            if (_tabs.Count > 0)
            {
                ctx.Rectangle(0, allocation.Height - BottomBarPadding, allocation.Width, BottomBarPadding);
                ctx.SetSourceColor(Styles.BreadcrumbBackgroundColor);
                ctx.Fill();
            }

            ctx.Rectangle(_tabStartX - LeanWidth / 2, y, tabArea + LeanWidth, allocation.Height);
            ctx.Clip();

            foreach (var cmd in drawCommands) cmd(ctx);

            ctx.ResetClip();

            // Redraw the dragging tab here to be sure its on top. We drew it before to get the sizing correct, this should be fixed.
            drawActive(ctx);
        }

        protected override bool OnExposeEvent(EventExpose evnt)
        {
            using (var context = CairoHelper.Create(evnt.Window))
            {
                Draw(context);
            }
            return base.OnExposeEvent(evnt);
        }

        private static void DrawCloseButton(Context context, Point center, bool hovered, double opacity, double animationProgress)
        {
            if (hovered)
            {
                const double radius = 6;
                context.Arc(center.X, center.Y, radius, 0, Math.PI * 2);
                context.SetSourceRGBA(.6, .6, .6, opacity);
                context.Fill();

                context.SetSourceRGBA(0.95, 0.95, 0.95, opacity);
                context.LineWidth = 2;

                context.MoveTo(center.X - 3, center.Y - 3);
                context.LineTo(center.X + 3, center.Y + 3);
                context.MoveTo(center.X - 3, center.Y + 3);
                context.LineTo(center.X + 3, center.Y - 3);
                context.Stroke();
            }
            else
            {
                var lineColor = .63 - .1 * animationProgress;
                const double fillColor = .74;

                var heightMod = Math.Max(0, 1.0 - animationProgress * 2);
                context.MoveTo(center.X - 3, center.Y - 3 * heightMod);
                context.LineTo(center.X + 3, center.Y + 3 * heightMod);
                context.MoveTo(center.X - 3, center.Y + 3 * heightMod);
                context.LineTo(center.X + 3, center.Y - 3 * heightMod);

                context.LineWidth = 2;
                context.SetSourceRGBA(lineColor, lineColor, lineColor, opacity);
                context.Stroke();

                if (animationProgress > 0.5)
                {
                    var partialProg = (animationProgress - 0.5) * 2;
                    context.MoveTo(center.X - 3, center.Y);
                    context.LineTo(center.X + 3, center.Y);

                    context.LineWidth = 2 - partialProg;
                    context.SetSourceRGBA(lineColor, lineColor, lineColor, opacity);
                    context.Stroke();

                    var radius = partialProg * 3.5;

                    // Background
                    context.Arc(center.X, center.Y, radius, 0, Math.PI * 2);
                    context.SetSourceRGBA(fillColor, fillColor, fillColor, opacity);
                    context.Fill();

                    // Inset shadow
                    using (var lg = new LinearGradient(0, center.Y - 5, 0, center.Y))
                    {
                        context.Arc(center.X, center.Y + 1, radius, 0, Math.PI * 2);
                        lg.AddColorStop(0, new Color(0, 0, 0, 0.2 * opacity));
                        lg.AddColorStop(1, new Color(0, 0, 0, 0));
                        context.SetSource(lg);
                        context.Stroke();
                    }

                    // Outline
                    context.Arc(center.X, center.Y, radius, 0, Math.PI * 2);
                    context.SetSourceRGBA(lineColor, lineColor, lineColor, opacity);
                    context.Stroke();

                }
            }
        }

        private void DrawTab(Context ctx, DockNotebookTab tab, Rectangle allocation, Rectangle tabBounds, bool highlight, bool active, bool dragging, Layout la)
        {
            // This logic is stupid to have here, should be in the caller!
            if (dragging)
            {
                tabBounds.X = (int) (tabBounds.X + (_dragX - tabBounds.X) * _dragXProgress);
                tabBounds.X = Clamp(tabBounds.X, _tabStartX, _tabEndX - tabBounds.Width);
            }
            var padding = LeftRightPadding;
            padding = (int) (padding * Math.Min(1.0, Math.Max(0.5, (tabBounds.Width - 30) / 70.0)));

            ctx.LineWidth = 1;
            LayoutTabBorder(ctx, allocation, tabBounds.Width, tabBounds.X, 0, active);
            ctx.ClosePath();
            using (var gr = new LinearGradient(tabBounds.X, TopBarPadding, tabBounds.X, allocation.Bottom))
            {
                if (active)
                {
                    gr.AddColorStop(0, Styles.BreadcrumbGradientStartColor.MultiplyAlpha(tab.Opacity));
                    gr.AddColorStop(1, Styles.BreadcrumbBackgroundColor.MultiplyAlpha(tab.Opacity));
                }
                else
                {
                    gr.AddColorStop(0, CairoExtensions.ParseColor("f4f4f4").MultiplyAlpha(tab.Opacity));
                    gr.AddColorStop(1, CairoExtensions.ParseColor("cecece").MultiplyAlpha(tab.Opacity));
                }
                ctx.SetSource(gr);
            }
            ctx.Fill();

            ctx.SetSourceColor(new Color(1, 1, 1, .5).MultiplyAlpha(tab.Opacity));
            LayoutTabBorder(ctx, allocation, tabBounds.Width, tabBounds.X, 1, active);
            ctx.Stroke();

            ctx.SetSourceColor(Styles.BreadcrumbBorderColor.MultiplyAlpha(tab.Opacity));
            LayoutTabBorder(ctx, allocation, tabBounds.Width, tabBounds.X, 0, active);
            ctx.StrokePreserve();

            if (tab.GlowStrength > 0)
            {
                var mouse = _tracker.MousePosition;
                using (var rg = new RadialGradient(mouse.X, tabBounds.Bottom, 0, mouse.X, tabBounds.Bottom, 100))
                {
                    rg.AddColorStop(0, new Color(1, 1, 1, 0.4 * tab.Opacity * tab.GlowStrength));
                    rg.AddColorStop(1, new Color(1, 1, 1, 0));

                    ctx.SetSource(rg);
                    ctx.Fill();
                }
            }
            else
            {
                ctx.NewPath();
            }

            // Render Close Button (do this first so we can tell how much text to render)

            var ch = allocation.Height - TopBarPadding - BottomBarPadding + CloseImageTopOffset;
            var crect = new Rectangle(tabBounds.Right - padding - CloseButtonSize + 3,
                tabBounds.Y + TopBarPadding + (ch - CloseButtonSize) / 2,
                CloseButtonSize, CloseButtonSize);
            tab.CloseButtonAllocation = crect;
            tab.CloseButtonAllocation.Inflate(2, 2);

            var closeButtonHovered = _tracker.Hovered && tab.CloseButtonAllocation.Contains(_tracker.MousePosition) && tab.WidthModifier >= 1.0f;
            var drawCloseButton = tabBounds.Width > 60 || highlight || closeButtonHovered;
            if (drawCloseButton)
            {
                DrawCloseButton(ctx, new Point(crect.X + crect.Width / 2, crect.Y + crect.Height / 2), closeButtonHovered, tab.Opacity, tab.DirtyStrength);
            }

            // Render Text
            var w = tabBounds.Width - (padding * 2 + CloseButtonSize);
            if (!drawCloseButton)
                w += CloseButtonSize;

            var textStart = tabBounds.X + padding;

            ctx.MoveTo(textStart, tabBounds.Y + TopPadding + TextOffset + VerticalTextSize);
            if (!Platform.IsMac && !Platform.IsWindows)
            {
                // This is a work around for a linux specific problem.
                // A bug in the proprietary ATI driver caused TAB text not to draw.
                // If that bug get's fixed remove this HACK asap.
                la.Ellipsize = EllipsizeMode.End;
                la.Width = (int) (w * Scale.PangoScale);
                ctx.SetSourceColor(tab.Notify ? new Color(0, 0, 1) : Styles.TabBarActiveTextColor);
                Pango.CairoHelper.ShowLayoutLine(ctx, la.GetLine(0));
            }
            else
            {
                // ellipses are for space wasting ..., we cant afford that
                using (var lg = new LinearGradient(textStart + w - 5, 0, textStart + w + 3, 0))
                {
                    var color = tab.Notify ? new Color(0, 0, 1) : Styles.TabBarActiveTextColor;
                    color = color.MultiplyAlpha(tab.Opacity);
                    lg.AddColorStop(0, color);
                    color.A = 0;
                    lg.AddColorStop(1, color);
                    ctx.SetSource(lg);
                    Pango.CairoHelper.ShowLayoutLine(ctx, la.GetLine(0));
                }
            }
            la.Dispose();
        }

        private static void LayoutTabBorder(Context ctx, Rectangle allocation, int contentWidth, int px, int margin, bool active = true)
        {
            var x = 0.5 + px;
            var y = allocation.Height + 0.5 - BottomBarPadding + margin;
            double height = allocation.Height - TopBarPadding - BottomBarPadding;

            x += TabSpacing + margin;
            contentWidth -= (TabSpacing + margin) * 2;

            var rightx = x + contentWidth;

            var lean = Math.Min(LeanWidth, contentWidth / 2);
            var halfLean = lean / 2;
            const int smoothing = 2;
            if (active)
            {
                ctx.MoveTo(0, y + 0.5);
                ctx.LineTo(0, y);
                ctx.LineTo(x - halfLean, y);
            }
            else
            {
                ctx.MoveTo(x - halfLean, y + 0.5);
                ctx.LineTo(x - halfLean, y);
            }
            ctx.CurveTo(new PointD(x + smoothing, y),
                new PointD(x - smoothing, y - height),
                new PointD(x + halfLean, y - height));
            ctx.LineTo(rightx - halfLean, y - height);
            ctx.CurveTo(new PointD(rightx + smoothing, y - height),
                new PointD(rightx - smoothing, y),
                new PointD(rightx + halfLean, y));

            if (active)
            {
                ctx.LineTo(allocation.Width, y);
                ctx.LineTo(allocation.Width, y + 0.5);
            }
            else
            {
                ctx.LineTo(rightx + halfLean, y + 0.5);
            }
        }

        private Layout CreateSizedLayout()
        {
            var la = new Layout(PangoContext) {FontDescription = FontDescription.FromString("normal")};
            la.FontDescription.AbsoluteSize = Units.FromPixels(VerticalTextSize);
            return la;
        }

        private Layout CreateTabLayout(DockNotebookTab tab)
        {
            var la = CreateSizedLayout();
            if (!string.IsNullOrEmpty(tab.Markup)) la.SetMarkup(tab.Markup);
            else if (!string.IsNullOrEmpty(tab.Text)) la.SetText(tab.Text);
            return la;
        }

        private class DockNotebookTab : IAnimatable
        {
            private readonly TabStrip _strip;

            private string _text;
            private string _markup;

            internal Gdk.Rectangle Allocation;
            internal Gdk.Rectangle CloseButtonAllocation;

            public int Index { get; internal set; }
            public bool Notify { get; set; }
            public double WidthModifier { get; set; }
            public double Opacity { get; set; }
            public double GlowStrength { get; set; }
            public bool Hidden { get; set; }
            public double DirtyStrength { get; set; }

            void IAnimatable.BatchBegin()
            {
            }

            void IAnimatable.BatchCommit()
            {
                QueueDraw();
            }

            private bool _dirty;

            public bool Dirty
            {
                get { return _dirty; }
                set
                {
                    if (_dirty == value) return;
                    _dirty = value;
                    this.Animate("Dirty", f => DirtyStrength = f, easing: Easing.CubicInOut, start: DirtyStrength, end: value ? 1 : 0);
                }
            }

            public string Text
            {
                get { return _text; }
                set
                {
                    _text = value;
                    _markup = null;
                    _strip.Update();
                }
            }

            public string Markup
            {
                get { return _markup; }
                set
                {
                    _markup = value;
                    _text = null;
                    _strip.Update();
                }
            }

            public string Tooltip { get; set; }

            internal DockNotebookTab(TabStrip strip)
            {
                _strip = strip;
            }

            internal Gdk.Rectangle SavedAllocation { get; private set; }
            internal double SaveStrength { get; set; }

            internal void SaveAllocation()
            {
                SavedAllocation = Allocation;
            }

            public void QueueDraw()
            {
                _strip.QueueDraw();
            }
        }
    }
}