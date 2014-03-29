using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Sledge.Editor.UI
{
    public class TableSplitConfiguration
    {
        public int Rows { get; set; }
        public int Columns { get; set; }
        public List<Rectangle> Rectangles { get; set; }

        public TableSplitConfiguration()
        {
            Rectangles = new List<Rectangle>();
        }

        public bool IsValid()
        {
            var cells = new bool[Rows, Columns];
            var set = 0;
            foreach (var r in Rectangles)
            {
                if (r.X < 0 || r.X + r.Width > Columns) return false;
                if (r.Y < 0 || r.Y + r.Height > Rows) return false;
                for (var i = r.X; i < r.X + r.Width; i++)
                {
                    for (var j = r.Y; j < r.Y + r.Height; j++)
                    {
                        if (cells[j, i]) return false;
                        cells[j, i] = true;
                        set++;
                    }
                }
            }
            return set == Columns * Rows;
        }

        public static TableSplitConfiguration Default()
        {
            return new TableSplitConfiguration
            {
                Columns = 2,
                Rows = 2,
                Rectangles =
                {
                    new Rectangle(0, 0, 1, 1),
                    new Rectangle(1, 0, 1, 1),
                    new Rectangle(0, 1, 1, 1),
                    new Rectangle(1, 1, 1, 1)
                }
            };
        }
    }

    public sealed class TableSplitControl : TableLayoutPanel
    {
        private bool _inH;
        private bool _inV;

        private bool _resizing;

        public int MinimumViewSize { get; set; }
        private int MaximumViewSize { get { return 100 - MinimumViewSize; } }

        private TableSplitConfiguration _configuration;

        public TableSplitConfiguration Configuration
        {
            get { return _configuration; }
            set
            {
                if (!value.IsValid()) return;
                _configuration = value;
                ResetLayout();
            }
        }

        private void ResetLayout()
        {
            RowCount = _configuration.Rows;
            ColumnCount = _configuration.Columns;
            for (var i = 0; i < ColumnCount; i++) ColumnStyles.Add(new ColumnStyle(SizeType.Percent, (int)(100m / ColumnCount)));
            for (var i = 0; i < RowCount; i++) RowStyles.Add(new RowStyle(SizeType.Percent, (int)(100m / RowCount)));
            ResetViews();
        }

        public TableSplitControl()
        {
            MinimumViewSize = 2;
            _resizing = _inH = _inV = false;
            _configuration = TableSplitConfiguration.Default();
            ResetLayout();
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            var r = GetPositionFromControl(e.Control);
            var rec = _configuration.Rectangles.FirstOrDefault(t => t.X <= r.Column && t.X + t.Width > r.Column && t.Y <= r.Row && t.Y + t.Height > r.Row);
            if (!rec.IsEmpty)
            {
                SetRowSpan(e.Control, rec.Height);
                SetColumnSpan(e.Control, rec.Width);
            }
            base.OnControlAdded(e);
        }

        public void ResetViews()
        {
            var c = (int) Math.Floor(100m / _configuration.Columns);
            var r = (int)Math.Floor(100m / _configuration.Rows);
            for (var i = 0; i < ColumnCount; i++) ColumnStyles[i].Width = i == 0 ? 100 - (c * (ColumnCount - 1)) : c;
            for (var i = 0; i < RowCount; i++) RowStyles[i].Height = i == 0 ? 100 - (r * (RowCount - 1)) : r;
        }

        public void FocusOn(Control ctrl)
        {
            if (ctrl == null || !Controls.Contains(ctrl)) return;
            var row = GetRow(ctrl);
            var col = GetColumn(ctrl);
            FocusOn(row, col);
        }

        public void FocusOn(int rowIndex, int columnIndex)
        {
            // todo
            if (rowIndex < 0 || rowIndex > 1 || columnIndex < 0 || columnIndex > 1) return;
            RememberFocus();
            ColumnStyles[columnIndex].Width = MaximumViewSize;
            ColumnStyles[(columnIndex + 1) % 2].Width = MinimumViewSize;
            RowStyles[rowIndex].Height = MaximumViewSize;
            RowStyles[(rowIndex + 1) % 2].Height = MinimumViewSize;
        }

        private void RememberFocus()
        {
            _memoryWidth = new float[ColumnStyles.Count];
            _memoryHeight = new float[RowStyles.Count];
            for (var i = 0; i < ColumnStyles.Count; i++)
            {
                _memoryWidth[i] = ColumnStyles[i].Width;
            }
            for (var i = 0; i < RowStyles.Count; i++)
            {
                _memoryHeight[i] = RowStyles[i].Height;
            }
        }

        private void ForgetFocus()
        {
            _memoryWidth = _memoryHeight = null;
        }

        public void Unfocus()
        {
            for (var i = 0; i < ColumnStyles.Count; i++)
            {
                ColumnStyles[i].Width = _memoryWidth[i];
            }
            for (var i = 0; i < RowStyles.Count; i++)
            {
                RowStyles[i].Height = _memoryHeight[i];
            }
            ForgetFocus();
        }

        public bool IsFocusing()
        {
            return _memoryWidth != null;
        }

        private float[] _memoryWidth;
        private float[] _memoryHeight;

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_resizing)
            {
                if (_inH && Width > 0)
                {
                    ForgetFocus();
                    var mp = e.Y / (float)Height * 100;
                    mp = Math.Min(MaximumViewSize, Math.Max(MinimumViewSize, mp));
                    RowStyles[0].Height = mp;
                    RowStyles[1].Height = 100 - mp;
                }
                if (_inV && Height > 0)
                {
                    ForgetFocus();
                    var mp = e.X / (float)Width * 100;
                    mp = Math.Min(MaximumViewSize, Math.Max(MinimumViewSize, mp));
                    ColumnStyles[0].Width = mp;
                    ColumnStyles[1].Width = 100 - mp;
                }
            }
            else
            {
                var cw = GetColumnWidths();
                var rh = GetRowHeights();
                var ht = rh[0] - Margin.Bottom;
                var hb = rh[0] + Margin.Top;
                var vl = cw[0] - Margin.Right;
                var vr = cw[0] + Margin.Left;

                _inH = e.X > Margin.Left && e.X < Width - Margin.Right && e.Y > ht && e.Y < hb;
                _inV = e.Y > Margin.Top && e.Y < Height - Margin.Bottom && e.X > vl && e.X < vr;

                if (_inH && _inV) Cursor = Cursors.SizeAll;
                else if (_inV) Cursor = Cursors.SizeWE;
                else if (_inH) Cursor = Cursors.SizeNS;
                else Cursor = Cursors.Default;
            }

            base.OnMouseMove(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            if (!_resizing) Cursor = Cursors.Default;
            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (_inV || _inH) _resizing = true;
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            _resizing = false;
            base.OnMouseUp(e);
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            if (_inH)
            {
                ForgetFocus();
                RowStyles[0].Height = RowStyles[1].Height = 50;
            }
            if (_inV)
            {
                ForgetFocus();
                ColumnStyles[0].Width = ColumnStyles[1].Width = 50;
            }
        }
    }
}
