using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Sledge.Providers.Texture;
using Sledge.UI;

namespace Sledge.Editor.UI
{
    public sealed class TextureListPanel : Panel
    {
        public delegate void TextureSelectedEventHandler(object sender, TextureItem item);
        public delegate void SelectionChangedEventHandler(object sender, IEnumerable<TextureItem> selection);

        public event TextureSelectedEventHandler TextureSelected;

        private void OnTextureSelected(TextureItem item)
        {
            if (TextureSelected != null)
            {
                TextureSelected(this, item);
            }
        }

        public event SelectionChangedEventHandler SelectionChanged;

        private void OnSelectionChanged(IEnumerable<TextureItem> selection)
        {
            if (SelectionChanged != null)
            {
                SelectionChanged(this, selection);
            }
        }

        private readonly VScrollBar _scrollBar;

        private readonly List<TextureItem> _textures;
        private Size _calculatedSize;
        private int _imageSize;

        private bool _allowSelection;
        private bool _allowMultipleSelection;
        private TextureItem _lastSelectedItem;
        private readonly List<TextureItem> _selection;

        #region Properties

        public bool AllowSelection
        {
            get { return _allowSelection; }
            set
            {
                _allowSelection = value;
                if (!_allowSelection && _selection.Count > 0)
                {
                    _selection.Clear();
                    Refresh();
                }
            }
        }

        public bool AllowMultipleSelection
        {
            get { return _allowMultipleSelection; }
            set
            {
                _allowMultipleSelection = value;
                if (!_allowMultipleSelection && _selection.Count > 0)
                {
                    var first = _selection.First();
                    _selection.Clear();
                    _selection.Add(first);
                    Refresh();
                }
            }
        }

        public int ImageSize
        {
            get { return _imageSize; }
            set
            {
                _imageSize = value;
                UpdateScrollbarValues();
            }
        }

        #endregion

        public TextureListPanel()
        {
            BackColor = Color.Black;
            VScroll = true;
            AutoScroll = true;
            DoubleBuffered = true;

            AllowSelection = true;
            AllowMultipleSelection = true;

            _scrollBar = new VScrollBar {Dock = DockStyle.Right};
            _scrollBar.ValueChanged += (sender, e) => Refresh();
            _textures = new List<TextureItem>();
            _selection = new List<TextureItem>();
            _imageSize = 128;

            Controls.Add(_scrollBar);

            UpdateScrollbarValues();
        }

        #region Selection

        public void SetSelectedTextures(IEnumerable<TextureItem> items)
        {
            _selection.Clear();
            _selection.AddRange(items);
            OnSelectionChanged(_selection);
            Refresh();
        }

        public void ScrollToItem(TextureItem item)
        {
            var index = _textures.IndexOf(item);
            if (index < 0) return;

            var texturesPerRow = (int)Math.Floor(Width / (float)_calculatedSize.Width);
            var texRow = index / texturesPerRow;
            var yscroll = Math.Max(0, Math.Min(texRow * _calculatedSize.Height, _scrollBar.Maximum - ClientRectangle.Height));
            _scrollBar.Value = yscroll;
            Refresh();
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            if (KeyboardState.Ctrl || KeyboardState.Shift || _selection.Count != 1) return;

            var x = e.X;
            var y = _scrollBar.Value + e.Y;

            var texturesPerRow = (int)Math.Floor(Width / (float)_calculatedSize.Width);
            var clickedRow = (int)Math.Floor(y / (float)_calculatedSize.Height);
            var clickedColumn = (int)Math.Floor(x / (float)_calculatedSize.Width);
            var clickedIndex = texturesPerRow * clickedRow + clickedColumn;

            var item = _textures[clickedIndex];
            if (item == _selection[0])
            {
                OnTextureSelected(_selection[0]);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (!AllowSelection) return;
            if (!AllowMultipleSelection || !KeyboardState.Ctrl) _selection.Clear();

            var x = e.X;
            var y = _scrollBar.Value + e.Y;

            var texturesPerRow = (int) Math.Floor(Width / (float) _calculatedSize.Width);
            var clickedRow = (int) Math.Floor(y / (float) _calculatedSize.Height);
            var clickedColumn = (int) Math.Floor(x / (float) _calculatedSize.Width);
            var clickedIndex = texturesPerRow * clickedRow + clickedColumn;

            var item = clickedIndex >= 0 && clickedIndex < _textures.Count ? _textures[clickedIndex] : null;

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
                var bef = _textures.IndexOf(_lastSelectedItem);
                var start = Math.Min(bef, clickedIndex);
                var count = Math.Abs(clickedIndex - bef) + 1;
                _selection.AddRange(_textures.GetRange(start, count).Where(i => !_selection.Contains(i)));
            }
            else 
            {
                _selection.Add(item);
                _lastSelectedItem = item;
            }
            OnSelectionChanged(_selection);

            Refresh();
        }

        #endregion

        #region Add/Remove/Get Textures

        public IEnumerable<TextureItem> GetTextures()
        {
            return _textures;
        }

        public IEnumerable<TextureItem> GetSelectedTextures()
        {
            return _selection;
        }

        public void RemoveAllTextures()
        {
            _textures.Clear();
            _lastSelectedItem = null;
            _selection.Clear();
            OnSelectionChanged(_selection);
            UpdateScrollbarValues();
        }

        public void SetTextureList(IEnumerable<TextureItem> textures)
        {
            _textures.Clear();
            _lastSelectedItem = null;
            _selection.Clear();
            _textures.AddRange(textures);
            OnSelectionChanged(_selection);
            UpdateScrollbarValues();
        }

        public void AddTextures(IEnumerable<TextureItem> textures)
        {
            _textures.AddRange(textures);
            UpdateScrollbarValues();
        }

        public void Clear()
        {
            _textures.Clear();
            _selection.Clear();
            _lastSelectedItem = null;
            OnSelectionChanged(_selection);
        }

        #endregion

        #region Scrolling

        private void ScrollByAmount(int value)
        {
            var newValue = _scrollBar.Value + value;
            _scrollBar.Value = newValue < 0 ? 0 : Math.Min(newValue, Math.Max(0, _scrollBar.Maximum - ClientRectangle.Height));
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
            }
            base.OnKeyDown(e);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            Focus();
            base.OnMouseEnter(e);
        }

        #endregion

        #region Updating Scrollbar Values

        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);
            UpdateScrollbarValues();
        }

        private void UpdateScrollbarValues()
        {
            _calculatedSize = new Size(_imageSize + 6, _imageSize + 6 + SystemFonts.MessageBoxFont.Height + 4);

            var texturesPerRow = (int)Math.Floor(Width / (float)_calculatedSize.Width);
            var numberOfRows = (int)Math.Ceiling(_textures.Count / (float)texturesPerRow);

            _scrollBar.Maximum = numberOfRows * _calculatedSize.Height;
            _scrollBar.SmallChange = _calculatedSize.Height;
            _scrollBar.LargeChange = ClientRectangle.Height;

            if (_scrollBar.Value > _scrollBar.Maximum - ClientRectangle.Height)
            {
                _scrollBar.Value = Math.Max(0, _scrollBar.Maximum - ClientRectangle.Height);
            }
            
            Refresh();
        }

        #endregion

        #region Rendering

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            RenderTextures(e.Graphics);
        }

        public void RenderTextures(System.Drawing.Graphics g)
        {
            if (_textures.Count == 0) return;

            var x = 0;
            var y = _scrollBar.Value;
            var width = ClientRectangle.Width;
            var height = ClientRectangle.Height;

            var texturesPerRow = (int)Math.Floor(Width / (float)_calculatedSize.Width);
            var topRow = (int)Math.Floor(y / (float)_calculatedSize.Height);
            var bottomRow = (int)Math.Floor((y + height) / (float)_calculatedSize.Height);
            var skip = topRow * texturesPerRow;
            var take = (bottomRow - topRow + 1) * texturesPerRow;
            y = topRow * _calculatedSize.Height - y;

            var packs = _textures.Select(t => t.Package).Distinct();
            using (var stream = TextureProvider.GetStreamSourceForPackages(packs))
            {
                foreach (var ti in _textures.Skip(skip).Take(take))
                {
                    using (var bmp = stream.GetImage(ti))
                    {
                        DrawImage(g, bmp, ti, x, y);
                    }
                    x += _calculatedSize.Width;

                    if (x + _calculatedSize.Width > width)
                    {
                        x = 0;
                        y += _calculatedSize.Height;
                        if (y >= height)
                        {
                            break;
                        }
                    }
                }
            }
        }

        private void DrawImage(System.Drawing.Graphics g, Image bmp, TextureItem ti, int x, int y)
        {
            if (bmp == null) return;

            var iw = bmp.Width;
            var ih = bmp.Height;
            if (iw > _imageSize && iw >= ih)
            {
                ih = (int)Math.Floor(_imageSize * (ih / (float)iw));
                iw = _imageSize;
            }
            else if (ih > _imageSize)
            {
                iw = (int)Math.Floor(_imageSize * (iw / (float)ih));
                ih = _imageSize;
            }

            g.DrawImage(bmp, x + 3, y + 3, iw, ih);
            if (_selection.Contains(ti))
            {
                g.DrawRectangle(Pens.Red, x + 2, y + 2, _imageSize + 1, _imageSize + 1);
                g.DrawRectangle(Pens.Red, x + 1, y + 1, _imageSize + 3, _imageSize + 3);
            }
            else
            {
                g.DrawRectangle(Pens.White, x + 1, y + 1, _imageSize + 3, _imageSize + 3);
            }
            g.DrawString(ti.Name, SystemFonts.MessageBoxFont, System.Drawing.Brushes.White, x + 1, y + _imageSize + 6);
        }

        #endregion
    }
}
