using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sledge.BspEditor.Environment;
using Sledge.Providers.Texture;
using Sledge.Shell;
using Sledge.Shell.Input;

namespace Sledge.BspEditor.Tools.Texture
{
    public sealed class TextureListPanel : Panel
    {
        public event EventHandler<string> TextureSelected;
        public event EventHandler<List<string>> SelectionChanged;
        private event EventHandler DebouncedInvalidate;

        private void OnTextureSelected(string item)
        {
            TextureSelected?.Invoke(this, item);
        }

        private void OnSelectionChanged(IEnumerable<string> selection)
        {
            SelectionChanged?.Invoke(this, selection.ToList());
        }

        private readonly VScrollBar _scrollBar;
        private List<string> _textures;

        private string _lastSelectedItem;
        private readonly HashSet<string> _selection;

        private readonly IDisposable _observable;

        private readonly object _lock = new object();
        private readonly Dictionary<string, TextureControl> _controls = new Dictionary<string, TextureControl>();

        #region Properties

        private TextureCollection _collection;
        public TextureCollection Collection
        {
            get => _collection;
            set
            {
                _collection = value;
                Invalidate();
            }
        }

        private bool _allowSelection;
        public bool AllowSelection
        {
            get => _allowSelection;
            set
            {
                _allowSelection = value;
                if (!_allowSelection && _selection.Count > 0)
                {
                    _selection.Clear();
                    Invalidate();
                }
            }
        }

        private bool _allowMultipleSelection;
        public bool AllowMultipleSelection
        {
            get => _allowMultipleSelection;
            set
            {
                _allowMultipleSelection = value;
                if (!_allowMultipleSelection && _selection.Count > 0)
                {
                    var first = _selection.First();
                    _selection.Clear();
                    _selection.Add(first);
                    Invalidate();
                }
            }
        }

        private int _imageSize;
        public int ImageSize
        {
            get => _imageSize;
            set
            {
                _imageSize = value;
                lock (_lock)
                {
                    foreach (var kv in _controls)
                    {
                        kv.Value.ImageSize = ImageSize;
                    }
                }
                ControlInvalidated();
            }
        }

        public bool EnableDrag { get; set; }

        #endregion

        public TextureListPanel()
        {
            _collection = new TextureCollection(new List<TexturePackage>());

            BackColor = Color.Black;
            VScroll = true;
            AutoScroll = true;
            DoubleBuffered = true;

            AllowSelection = true;
            AllowMultipleSelection = true;

            _scrollBar = new VScrollBar { Dock = DockStyle.Right };
            _scrollBar.ValueChanged += (sender, e) => Invalidate();
            _textures = new List<string>();
            _selection = new HashSet<string>();
            _imageSize = 128;

            Controls.Add(_scrollBar);

            UpdateTextureList();

            _observable = Observable.FromEventPattern(x => DebouncedInvalidate += x, x => DebouncedInvalidate -= x)
                .Throttle(TimeSpan.FromMilliseconds(100))
                .Subscribe(x =>
                {
                    UpdateTexturePanels();
                    if (IsHandleCreated) Invalidate();
                });
        }

        #region Selection

        public void SetSelectedTextures(IEnumerable<string> items)
        {
            _selection.Clear();
            _selection.UnionWith(items);
            ControlInvalidated();
        }

        public void ScrollToItem(string item)
        {
            lock (_lock)
            {
                if (!_controls.ContainsKey(item)) return;
                var con = _controls[item];

                var rec = new Rectangle(con.Point, con.Size);
                var yscroll = Math.Max(0, Math.Min(rec.Top, _scrollBar.Maximum - ClientRectangle.Height));
                _scrollBar.Value = yscroll;

                ControlInvalidated();
            }
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            if (KeyboardState.Ctrl || KeyboardState.Shift || _selection.Count != 1) return;

            var x = e.X;
            var y = _scrollBar.Value + e.Y;

            var clickedIndex = GetIndexAt(x, y);

            var item = _textures.ElementAt(clickedIndex);
            if (_selection.Contains(item))
            {
                OnTextureSelected(item);
            }
        }

        private bool _down;
        private Point _downPoint;

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (!AllowSelection) return;
            if (!AllowMultipleSelection || !KeyboardState.Ctrl) _selection.Clear();

            if (e.Button == MouseButtons.Left)
            {
                _down = true;
                _downPoint = e.Location;
            }

            var x = e.X;
            var y = _scrollBar.Value + e.Y;

            var clickedIndex = GetIndexAt(x, y);

            var item = clickedIndex >= 0 && clickedIndex < _textures.Count
                ? _textures.ElementAt(clickedIndex)
                : null;

            if (item == null)
            {
                _selection.Clear();
            }
            else if (AllowMultipleSelection && KeyboardState.Ctrl && _selection.Contains(item))
            {
                _selection.Remove(item);
                _lastSelectedItem = null;
            }
            else if (AllowMultipleSelection && KeyboardState.Shift && _lastSelectedItem != null)
            {
                var bef = _textures.ToList().IndexOf(_lastSelectedItem);
                var start = Math.Min(bef, clickedIndex);
                var count = Math.Abs(clickedIndex - bef) + 1;
                _selection.UnionWith(_textures.GetRange(start, count).Where(i => !_selection.Contains(i)));
            }
            else
            {
                _selection.Add(item);
                _lastSelectedItem = item;
            }
            OnSelectionChanged(_selection);

            Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (_down && EnableDrag && _selection.Any() &&
                (Math.Abs(e.X - _downPoint.X) > 2 || Math.Abs(e.Y - _downPoint.Y) > 2))
            {
                _down = false;
                DoDragDrop(_selection.ToList(), DragDropEffects.Copy);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) _down = false;
            base.OnMouseUp(e);
        }

        private int GetIndexAt(int x, int y)
        {
            lock (_lock)
            {
                for (var i = 0; i < _textures.Count; i++)
                {
                    var tex = _textures[i];

                    if (!_controls.ContainsKey(tex)) continue;
                    var con = _controls[tex];

                    var rec = new Rectangle(con.Point, con.Size);
                    if (rec.Contains(x, y)) return i;
                }
            }
            return -1;
        }

        #endregion

        #region Add/Remove/Get Textures

        public void SortTextureList<T>(Func<string, T> sortFunc, bool descending)
        {
            lock (_lock)
            {
                _textures = descending
                    ? _textures.OrderByDescending(sortFunc).ToList()
                    : _textures.OrderBy(sortFunc).ToList();
            }
            ControlInvalidated();
        }

        public IEnumerable<string> GetSelectedTextures()
        {
            return _selection;
        }

        private ITextureStreamSource _streamSource;
        private Task<ITextureStreamSource> _streamSourceTask = Task.FromResult<ITextureStreamSource>(null);

        public async Task<Bitmap> GetTextureBitmap(string name, int maxWidth, int maxHeight)
        {
            var ss = await _streamSourceTask;
            if (ss == null) return null;
            return await ss.GetImage(name, maxWidth, maxHeight);
        }

        public async Task SetTextureList(IEnumerable<string> textures)
        {
            lock (_lock)
            {
                _textures.Clear();
                _lastSelectedItem = null;
                _selection.Clear();
                _textures.AddRange(textures);
            }

            _streamSource?.Dispose();
            _streamSource = null;
            _streamSourceTask = _collection.GetStreamSource();
            _streamSource = await _streamSourceTask;
            
            UpdateTextureList();

            UpdateTexturePanels();
            Invalidate();
        }

        public IEnumerable<string> GetTextureList()
        {
            return _textures.ToList();
        }

        public void Clear()
        {
            lock (_lock)
            {
                _textures.Clear();
                _lastSelectedItem = null;
                _selection.Clear();
            }

            _streamSource?.Dispose();
            _streamSource = null;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Clear();
                _observable.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion

        #region Scrolling

        private void ScrollByAmount(int value)
        {
            var newValue = _scrollBar.Value + value;
            _scrollBar.Value = newValue < 0
                ? 0
                : Math.Min(newValue, Math.Max(0, _scrollBar.Maximum - ClientRectangle.Height));
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            ScrollByAmount(_scrollBar.SmallChange * (e.Delta / -120));
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.PageDown:
                    ScrollByAmount(_scrollBar.LargeChange);
                    break;
                case Keys.PageUp:
                    ScrollByAmount(-_scrollBar.LargeChange);
                    break;
                case Keys.End:
                    ScrollByAmount(int.MaxValue);
                    break;
                case Keys.Home:
                    ScrollByAmount(-int.MaxValue);
                    break;
                case Keys.Enter:
                    if (_selection.Count > 0) OnTextureSelected(_selection.First());
                    break;
            }
            base.OnKeyDown(e);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            Focus();
            base.OnMouseEnter(e);
        }

        #endregion

        #region Updating controls

        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);
            ControlInvalidated();
        }

        private void ControlInvalidated()
        {
            DebouncedInvalidate?.Invoke(this, EventArgs.Empty);
        }

        private void UpdateTextureList()
        {
            lock (_lock)
            {
                foreach (var rem in _controls.Keys.Except(_textures).ToList())
                {
                    var r = _controls[rem];
                    _controls.Remove(rem);
                    r.Dispose();
                }

                foreach (var add in _textures.Except(_controls.Keys).ToList())
                {
                    var ctrl = new TextureControl(add, GetTextureBitmap, ControlInvalidated)
                    {
                        ImageSize = ImageSize
                    };
                    _controls[add] = ctrl;
                }
            }
        }

        private void UpdateTexturePanels()
        {
            int viewportWidth = ClientRectangle.Width - _scrollBar.Width,
                currentX = 0,
                currentY = 0,
                maxHeight = 0;

            lock (_lock)
            {
                foreach (var tex in _textures)
                {
                    if (!_controls.ContainsKey(tex)) continue;
                    var con = _controls[tex];

                    var cw = con.Size.Width;
                    var ch = con.Size.Height;

                    if (currentX > 0 && currentX + cw > viewportWidth)
                    {
                        currentX = 0;
                        currentY += maxHeight;
                        maxHeight = 0;
                    }

                    con.Point = new Point(currentX, currentY);

                    currentX += cw;
                    maxHeight = Math.Max(maxHeight, ch);
                }
            }

            Task.Factory.StartNew(() =>
            {
                _scrollBar.Invoke(() =>
                {
                    _scrollBar.Maximum = currentY + maxHeight;
                    _scrollBar.SmallChange = _imageSize > 0 ? _imageSize : 128;
                    _scrollBar.LargeChange = ClientRectangle.Height;

                    if (_scrollBar.Value > _scrollBar.Maximum - ClientRectangle.Height)
                    {
                        _scrollBar.Value = Math.Max(0, _scrollBar.Maximum - ClientRectangle.Height);
                    }
                });
            });
        }

        #endregion

        #region Rendering

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            RenderTextures(e.Graphics);
        }

        private void RenderTextures(Graphics g)
        {
            if (_textures.Count == 0 || _streamSource == null) return;

            lock (_lock)
            {
                var y = _scrollBar.Value;
                var height = ClientRectangle.Height;

                var done = false;
                foreach (var tex in _textures)
                {
                    if (!_controls.ContainsKey(tex)) continue;
                    var con = _controls[tex];

                    var rec = new Rectangle(con.Point, con.Size);
                    if (rec.Top > y + height)
                    {
                        done = true;
                    }
                    if (done || rec.Bottom < y)
                    {
                        con.Release();
                        continue;
                    }
                    con.Paint(g, rec.X, rec.Y - y, _selection.Contains(tex));
                }
            }
        }

        #endregion

        private class TextureControl : IDisposable
        {
            private readonly Func<string, int, int, Task<Bitmap>> _getTextureBitmap;
            private Task<Bitmap> _bitmapTask;

            private readonly Action _invalidated;

            private int _imageSize;
            public int ImageSize
            {
                get => _imageSize;
                set
                {
                    if (_imageSize == value) return;
                    _imageSize = value;
                    SetSize(_imageSize, _imageSize);
                }
            }

            private string _textureName;
            public string TextureName
            {
                get => _textureName;
                set
                {
                    _textureName = value;
                    _invalidated();
                }
            }

            public Point Point { get; set; }

            private Size _size;
            public Size Size
            {
                get => _size;
                set
                {
                    _size = value;
                    _invalidated();
                }
            }

            private const int Padding = 3;
            private static Font Font = SystemFonts.MessageBoxFont;
            private static int FontHeight = SystemFonts.MessageBoxFont.Height;

            private void SetSize(int imageWidth, int imageHeight)
            {
                Size = new Size(imageWidth + Padding * 2, imageHeight + Padding * 2 + FontHeight);
            }

            public TextureControl(string textureName, Func<string, int, int, Task<Bitmap>> getTextureBitmap, Action invalidated)
            {
                _getTextureBitmap = getTextureBitmap;
                _invalidated = invalidated;
                TextureName = textureName;
            }

            public void Paint(Graphics g, int x, int y, bool selected)
            {
                if (_bitmapTask == null)
                {
                    _bitmapTask = _getTextureBitmap(TextureName, 256, 256);
                    _bitmapTask.ContinueWith(b => _invalidated());
                }

                if (selected)
                {
                    g.DrawRectangle(Pens.Red, new Rectangle(x + 1, y + 1, Size.Width - 2, Size.Height - 2));
                    g.DrawRectangle(Pens.Red, new Rectangle(x + 2, y + 2, Size.Width - 4, Size.Height - 4));
                }
                else
                {
                    g.DrawRectangle(Pens.Gray, new Rectangle(x + 1, y + 1, Size.Width - 2, Size.Height - 2));
                }
                g.DrawString(TextureName, Font, Brushes.White, x + 1, y + Size.Height - FontHeight - Padding);

                if (_bitmapTask != null && _bitmapTask.IsCompleted)
                {
                    var img = _bitmapTask.Result;
                    if (img != null)
                    {
                        DrawImage(g, img, x + Padding, y + Padding, Size.Width - Padding * 2, Size.Height - Padding * 2 - FontHeight);
                    }
                }
            }

            private void DrawImage(Graphics g, Image bmp, int x, int y, int w, int h)
            {
                if (bmp == null) return;

                var iw = bmp.Width;
                var ih = bmp.Height;
                if (iw > w && iw >= ih)
                {
                    ih = (int)Math.Floor(h * (ih / (float)iw));
                    iw = w;
                }
                else if (ih > h)
                {
                    iw = (int)Math.Floor(w * (iw / (float)ih));
                    ih = h;
                }
                g.DrawImage(bmp, x, y, iw, ih);
            }

            public void Dispose()
            {
                _bitmapTask?.ContinueWith(x =>
                {
                    x.Result?.Dispose();
                    x.Dispose();
                });
            }

            public void Release()
            {
                _bitmapTask?.ContinueWith(x =>
                {
                    x.Result?.Dispose();
                    x.Dispose();
                });
                _bitmapTask = null;
            }
        }
    }
}
