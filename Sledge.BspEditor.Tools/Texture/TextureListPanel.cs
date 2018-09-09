using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sledge.BspEditor.Environment;
using Sledge.BspEditor.Environment.Empty;
using Sledge.Common.Threading;
using Sledge.Providers.Texture;
using Sledge.Shell;
using Sledge.Shell.Input;
using Timer = System.Windows.Forms.Timer;

namespace Sledge.BspEditor.Tools.Texture
{
    /// <summary>
    /// A list of textures, rendered from a stream source.
    /// </summary>
    public sealed class TextureListPanel : Panel
    {
        /// <summary>
        /// Occurs when the user has chosen a single texture by double clicking or pressing enter
        /// </summary>
        public event EventHandler<string> TextureSelected;

        /// <summary>
        /// Occurs when the list of highlighted textures has been changed
        /// </summary>
        public event EventHandler<List<string>> HighlightedTexturesChanged;
        
        private string _lastSelectedItem;
        private string _scrollToItem;
        private ITextureStreamSource _streamSource;

        private readonly Timer _updateTimer;
        private readonly VScrollBar _scrollBar;
        private readonly ThreadSafeList<string> _textures;
        private readonly ThreadSafeSet<string> _selection;
        private readonly ConcurrentDictionary<string, TextureControl> _controls;

        #region Properties

        private TextureCollection _collection;

        /// <summary>
        /// The texture collection that this list is showing textures from
        /// </summary>
        public TextureCollection Collection
        {
            get => _collection;
            set
            {
                if (value != _collection)
                {
                    _streamSource?.Dispose();
                    _streamSource = null;
                }
                _collection = value;
                Invalidate();
            }
        }

        private bool _allowHighlighting;

        /// <summary>
        /// True to allow highlighting of textures
        /// </summary>
        public bool AllowHighlighting
        {
            get => _allowHighlighting;
            set
            {
                _allowHighlighting = value;
                if (!_allowHighlighting && _selection.Count > 0)
                {
                    _selection.Clear();
                    Invalidate();
                }
            }
        }

        private bool _allowMultipleHighlighting;

        /// <summary>
        /// True to allow multiple textures to be highlighted
        /// </summary>
        public bool AllowMultipleHighlighting
        {
            get => _allowMultipleHighlighting;
            set
            {
                _allowMultipleHighlighting = value;
                if (!_allowMultipleHighlighting && _selection.Count > 0)
                {
                    var first = _selection.First();
                    _selection.Clear();
                    _selection.Add(first);
                    Invalidate();
                }
            }
        }

        private int _imageSize;

        /// <summary>
        /// The size of the image thumbnails
        /// </summary>
        public int ImageSize
        {
            get => _imageSize;
            set
            {
                _imageSize = value;
                foreach (var kv in _controls.Values.ToList()) kv.ImageSize = ImageSize;
                ControlInvalidated();
            }
        }

        /// <summary>
        /// True to enable textures to be dragged using WinForms drag and drop
        /// </summary>
        public bool EnableDrag { get; set; }

        #endregion

        public TextureListPanel()
        {
            _collection = new EmptyTextureCollection(new List<TexturePackage>());

            BackColor = Color.Black;
            VScroll = true;
            AutoScroll = true;
            DoubleBuffered = true;

            AllowHighlighting = true;
            AllowMultipleHighlighting = true;

            _scrollBar = new VScrollBar { Dock = DockStyle.Right };
            _scrollBar.ValueChanged += (sender, e) => Invalidate();
            Controls.Add(_scrollBar);

            _updateTimer = new Timer {Enabled = false, Interval = 100};
            _updateTimer.Tick += (s, e) =>
            {
                _updateTimer.Stop();
                UpdateTexturePanels();
                if (IsHandleCreated) Invalidate();
            };
        
            _textures = new ThreadSafeList<string>();
            _selection = new ThreadSafeSet<string>();
            _controls = new ConcurrentDictionary<string, TextureControl>();
            _imageSize = 128;

            UpdateTextureList();
            ControlInvalidated();
        }

        private void ControlInvalidated()
        {
            this.InvokeLater(() => _updateTimer.Start());
        }

        /// <summary>
        /// Set the list of textures which are highlighted
        /// </summary>
        /// <param name="items">The list of texture names</param>
        public void SetHighlightedTextures(IEnumerable<string> items)
        {
            _selection.Clear();
            _selection.UnionWith(items);

            ControlInvalidated();
        }

        /// <summary>
        /// Scroll to a particular item in the list, if it exists
        /// </summary>
        /// <param name="item"></param>
        public void ScrollToTexture(string item)
        {
            _scrollToItem = item;
            ControlInvalidated();
        }

        #region Highlighting

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            if (KeyboardState.Ctrl || KeyboardState.Shift || _selection.Count != 1) return;

            var x = e.X;
            var y = _scrollBar.Value + e.Y;

            var clickedIndex = GetIndexAt(x, y);

            var item = _textures.ElementAt(clickedIndex);
            if (_selection.Contains(item)) TextureSelected?.Invoke(this, item);
        }

        private bool _down;
        private Point _downPoint;

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (!AllowHighlighting) return;
            if (!AllowMultipleHighlighting || !KeyboardState.Ctrl) _selection.Clear();

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
            else if (AllowMultipleHighlighting && KeyboardState.Ctrl && _selection.Contains(item))
            {
                _selection.Remove(item);
                _lastSelectedItem = null;
            }
            else if (AllowMultipleHighlighting && KeyboardState.Shift && _lastSelectedItem != null)
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

            HighlightedTexturesChanged?.Invoke(this, _selection.ToList());
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
            for (var i = 0; i < _textures.Count; i++)
            {
                var tex = _textures[i];

                if (!_controls.TryGetValue(tex, out var con)) continue;

                var rec = new Rectangle(con.Point, con.Size);
                if (rec.Contains(x, y)) return i;
            }
            return -1;
        }

        #endregion

        #region Add/Remove/Get Textures

        /// <summary>
        /// Change the sort order of the textures in this list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sortFunc">Function to sort by</param>
        /// <param name="descending">True to sort in descending order</param>
        public void SortTextureList<T>(Func<string, T> sortFunc, bool descending)
        {
            var newList = descending
                ? _textures.OrderByDescending(sortFunc).ToList()
                : _textures.OrderBy(sortFunc).ToList();

            _textures.Clear();
            _textures.AddRange(newList);

            ControlInvalidated();
        }

        /// <summary>
        /// Get the list of textures which are currently highlighted
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetHighlightedTextures()
        {
            return _selection;
        }

        /// <summary>
        /// Set the list of textures visible in this list
        /// </summary>
        /// <param name="textures"></param>
        /// <returns></returns>
        public Task SetTextureList(IEnumerable<string> textures)
        {
            _textures.Clear();
            _lastSelectedItem = null;
            _selection.Clear();
            _textures.AddRange(textures);

            if (_streamSource == null) _streamSource = _collection.GetStreamSource();
            
            UpdateTextureList();
            ControlInvalidated();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Get the list of textures visible in this list
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetTextureList()
        {
            return _textures.ToList();
        }

        /// <summary>
        /// Remove all textures from this list
        /// </summary>
        public void Clear()
        {
            _textures.Clear();
            _lastSelectedItem = null;
            _selection.Clear();

            _streamSource?.Dispose();
            _streamSource = null;

            ControlInvalidated();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Clear();
                _updateTimer.Dispose();
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
                    if (_selection.Count > 0) TextureSelected?.Invoke(this, _selection.First());
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

        private Task<Bitmap> GetTextureBitmap(string name, int maxWidth, int maxHeight)
        {
            if (_streamSource == null) return Task.FromResult<Bitmap>(null);
            return _streamSource.GetImage(name, maxWidth, maxHeight);
        }

        private void UpdateTextureList()
        {
            foreach (var rem in _controls.Keys.Except(_textures).ToList())
            {
                if (_controls.TryRemove(rem, out var r)) r.Dispose();
            }

            foreach (var add in _textures.Except(_controls.Keys).ToList())
            {
                var ctrl = new TextureControl(add, GetTextureBitmap, Invalidate)
                {
                    ImageSize = ImageSize
                };
                _controls.TryAdd(add, ctrl);
            }
        }

        private void UpdateTexturePanels()
        {
            int viewportWidth = ClientRectangle.Width - _scrollBar.Width,
                currentX = 0,
                currentY = 0,
                maxHeight = 0;

            foreach (var tex in _textures)
            {
                if (!_controls.TryGetValue(tex, out var con)) continue;

                var cw = con.Size.Width;
                var ch = con.Size.Height;

                if (currentX > 0 && currentX + cw > viewportWidth)
                {
                    currentX = 0;
                    currentY += maxHeight;
                    maxHeight = 0;
                }

                con.Point = new Point(currentX, currentY);
                con.Positioned = true;

                currentX += cw;
                maxHeight = Math.Max(maxHeight, ch);
            }

            _scrollBar.Maximum = currentY + maxHeight;
            _scrollBar.SmallChange = _imageSize > 0 ? _imageSize : 128;
            _scrollBar.LargeChange = ClientRectangle.Height;
            if (_scrollToItem != null)
            {
                if (_controls.TryGetValue(_scrollToItem, out var con))
                {
                    _scrollBar.Value = Math.Max(0, Math.Min(con.Point.Y, _scrollBar.Maximum - ClientRectangle.Height));
                }
                _scrollToItem = null;
            }
            if (_scrollBar.Value > _scrollBar.Maximum - ClientRectangle.Height)
            {
                _scrollBar.Value = Math.Max(0, _scrollBar.Maximum - ClientRectangle.Height);
            }
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
            
            var y = _scrollBar.Value;
            var height = ClientRectangle.Height;

            var done = false;
            foreach (var tex in _textures)
            {
                if (!_controls.TryGetValue(tex, out var con)) continue;
                if (!con.Positioned) continue;

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

        #endregion

        protected override void CreateHandle()
        {
            base.CreateHandle();
            ControlInvalidated();
        }

        protected override void DestroyHandle()
        {
            base.DestroyHandle();
            _streamSource?.Dispose();
            _streamSource = null;
            _updateTimer.Stop();
        }

        /// <summary>
        /// A virtual control which holds the location and size of a texture.
        /// The texture itself can be created and released from memory on demand.
        /// </summary>
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

            public bool Positioned { get; set; }
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
            private static readonly Font Font = SystemFonts.MessageBoxFont;
            private static readonly int FontHeight = SystemFonts.MessageBoxFont.Height;

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
                // If we get a request to paint, the image needs to be loaded if it's not already
                if (_bitmapTask == null)
                {
                    _bitmapTask = _getTextureBitmap(TextureName, 256, 256);
                    _bitmapTask.ContinueWith(b => _invalidated()); // Make sure the parent repaints once the bitmap is done loading
                }

                // Draw the border
                if (selected)
                {
                    g.DrawRectangle(Pens.Red, new Rectangle(x + 1, y + 1, Size.Width - 2, Size.Height - 2));
                    g.DrawRectangle(Pens.Red, new Rectangle(x + 2, y + 2, Size.Width - 4, Size.Height - 4));
                }
                else
                {
                    g.DrawRectangle(Pens.Gray, new Rectangle(x + 1, y + 1, Size.Width - 2, Size.Height - 2));
                }

                // Draw the texture name
                g.DrawString(TextureName, Font, Brushes.White, x + 1, y + Size.Height - FontHeight - Padding);

                // Draw the image (if it's loaded)
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
                Release();
            }

            /// <summary>
            /// Release the bitmap held by this control, but ensure it can be re-created if required.
            /// </summary>
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
