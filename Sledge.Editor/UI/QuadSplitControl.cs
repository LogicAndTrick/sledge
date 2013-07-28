using System;
using System.Windows.Forms;

namespace Sledge.Editor.UI
{
    public sealed class QuadSplitControl : TableLayoutPanel
    {
        private bool _inH;
        private bool _inV;

        private bool _resizing;

        public int MinimumViewSize { get; set; }
        private int MaximumViewSize { get { return 100 - MinimumViewSize; } }

        public QuadSplitControl()
        {
            MinimumViewSize = 2;
            _resizing = _inH = _inV = false;
            RowCount = ColumnCount = 2;
            ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        }

        public void ResetViews()
        {
            foreach (ColumnStyle cs in ColumnStyles) cs.Width = 50;
            foreach (RowStyle rs in RowStyles) rs.Height = 50;
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
