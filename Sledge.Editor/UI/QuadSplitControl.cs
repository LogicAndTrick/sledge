using System;
using System.Windows.Forms;

namespace Sledge.Editor.UI
{
    public sealed class QuadSplitControl : TableLayoutPanel
    {
        private bool _inH;
        private bool _inV;

        private bool _resizing;

        public QuadSplitControl()
        {
            _resizing = _inH = _inV = false;
            RowCount = ColumnCount = 2;
            ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_resizing)
            {
                if (_inH && Width > 0)
                {
                    var mp = e.Y / (float)Height * 100;
                    mp = Math.Min(95, Math.Max(5, mp));
                    RowStyles[0].Height = mp;
                    RowStyles[1].Height = 100 - mp;
                }
                if (_inV && Height > 0)
                {
                    var mp = e.X / (float)Width * 100;
                    mp = Math.Min(95, Math.Max(5, mp));
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
                RowStyles[0].Height = RowStyles[1].Height = 50;
            }
            if (_inV)
            {
                ColumnStyles[0].Width = ColumnStyles[1].Width = 50;
            }
        }
    }
}
