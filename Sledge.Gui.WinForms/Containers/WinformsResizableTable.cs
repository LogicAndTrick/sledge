using System;
using System.Linq;
using System.Windows.Forms;
using Sledge.Gui.Attributes;
using Sledge.Gui.Interfaces;
using Sledge.Gui.Interfaces.Containers;
using Sledge.Gui.Structures;
using Padding = System.Windows.Forms.Padding;

namespace Sledge.Gui.WinForms.Containers
{
    [ControlImplementation("WinForms")]
    public sealed class WinformsResizableTable : WinFormsContainer, IResizableTable
    {
        private TableLayoutPanel _table;

        private int _inH;
        private int _inV;

        private bool _resizing;

        public int MinimumViewSize { get; set; }
        private int MaximumViewSize { get { return 100 - MinimumViewSize; } }

        private ResizableTableConfiguration _configuration;

        public ResizableTableConfiguration Configuration
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
            _table.RowCount = _configuration.Rows;
            _table.ColumnCount = _configuration.Columns;
            _table.ColumnStyles.Clear();
            _table.RowStyles.Clear();
            for (var i = 0; i < _table.ColumnCount; i++) _table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, (int)(100m / _table.ColumnCount)));
            for (var i = 0; i < _table.RowCount; i++) _table.RowStyles.Add(new RowStyle(SizeType.Percent, (int)(100m / _table.RowCount)));
            ResetViews();
        }

        public WinformsResizableTable() : base(new TableLayoutPanel())
        {
            _table = (TableLayoutPanel) Control;
            _table.Padding = new Padding(3);

            _table.ControlAdded += OnControlAdded;
            _table.MouseMove += OnMouseMove;
            _table.MouseLeave += OnMouseLeave;
            _table.MouseDown += OnMouseDown;
            _table.MouseUp += OnMouseUp;
            _table.MouseDoubleClick += OnMouseDoubleClick;

            MinimumViewSize = 2;
            _resizing = false;
            _inH = _inV = -1;
            _configuration = ResizableTableConfiguration.Default();
            ResetLayout();
        }

        protected override void CalculateLayout()
        {
            Control.SuspendLayout();

            foreach (var child in Children)
            {
                child.Control.Dock = DockStyle.Fill;
                var meta = Metadata[child];
                _table.SetCellPosition(child.Control, new TableLayoutPanelCellPosition(meta.Get<int>("Column"), meta.Get<int>("Row")));
            }

            Control.ResumeLayout();
        }

        public int[] GetColumnWidths()
        {
            return _table.GetColumnWidths();
        }

        public int[] GetRowHeights()
        {
            return _table.GetRowHeights();
        }

        private int _controlPadding;

        public int ControlPadding
        {
            get { return _controlPadding; }
            set
            {
                _controlPadding = value;
                foreach (Control control in _table.Controls)
                {
                    control.Margin = new Padding(value);
                }
            }
        }

        public void Insert(int row, int column, IControl child, int rowSpan = 1, int columnSpan = 1, bool rowFill = false, bool columnFill = false)
        {
            Insert(NumChildren, child, new ContainerMetadata {{"Row", row}, {"Column", column}});
        }

        public void SetRowHeight(int row, int height)
        {
            throw new NotImplementedException(); // todo
        }

        public void FocusOn(IControl ctrl)
        {
            throw new NotImplementedException(); // todo
        }

        public void SetColumnWidth(int column, int width)
        {
            throw new NotImplementedException(); // todo
        }

        private void OnControlAdded(object sender, ControlEventArgs e)
        {
            e.Control.Margin = new Padding(_controlPadding / 2);
            var r = _table.GetPositionFromControl(e.Control);
            var rec = _configuration.Rectangles.FirstOrDefault(t => t.X <= r.Column && t.X + t.Width > r.Column && t.Y <= r.Row && t.Y + t.Height > r.Row);
            if (!rec.IsEmpty)
            {
                _table.SetRow(e.Control, rec.Y);
                _table.SetColumn(e.Control, rec.X);
                _table.SetRowSpan(e.Control, rec.Height);
                _table.SetColumnSpan(e.Control, rec.Width);
            }
        }

        public void ReplaceControl(Control oldControl, Control newControl)
        {
            int col = _table.GetColumn(oldControl),
                row = _table.GetRow(oldControl),
                csp = _table.GetColumnSpan(oldControl),
                rsp = _table.GetRowSpan(oldControl);
            _table.Controls.Add(newControl, col, row);
            _table.Controls.Remove(oldControl);
            _table.SetColumnSpan(newControl, csp);
            _table.SetRowSpan(newControl, rsp);
        }

        public void ResetViews()
        {
            var c = (int) Math.Floor(100m / _configuration.Columns);
            var r = (int)Math.Floor(100m / _configuration.Rows);
            for (var i = 0; i < _table.ColumnCount; i++) _table.ColumnStyles[i].Width = i == 0 ? 100 - (c * (_table.ColumnCount - 1)) : c;
            for (var i = 0; i < _table.RowCount; i++) _table.RowStyles[i].Height = i == 0 ? 100 - (r * (_table.RowCount - 1)) : r;
        }

        public void FocusOn(Control ctrl)
        {
            if (ctrl == null || !_table.Controls.Contains(ctrl)) return;
            var row = _table.GetRow(ctrl);
            var col = _table.GetColumn(ctrl);
            FocusOn(row, col);
        }

        public void FocusOn(int rowIndex, int columnIndex)
        {
            if (rowIndex < 0 || rowIndex > 1 || columnIndex < 0 || columnIndex > 1) return;
            RememberFocus();
            _table.ColumnStyles[columnIndex].Width = MaximumViewSize;
            _table.ColumnStyles[(columnIndex + 1) % 2].Width = MinimumViewSize;
            _table.RowStyles[rowIndex].Height = MaximumViewSize;
            _table.RowStyles[(rowIndex + 1) % 2].Height = MinimumViewSize;
        }

        private void RememberFocus()
        {
            _memoryWidth = new float[_table.ColumnStyles.Count];
            _memoryHeight = new float[_table.RowStyles.Count];
            for (var i = 0; i < _table.ColumnStyles.Count; i++)
            {
                _memoryWidth[i] = _table.ColumnStyles[i].Width;
            }
            for (var i = 0; i < _table.RowStyles.Count; i++)
            {
                _memoryHeight[i] = _table.RowStyles[i].Height;
            }
        }

        private void ForgetFocus()
        {
            _memoryWidth = _memoryHeight = null;
        }

        public void Unfocus()
        {
            for (var i = 0; i < _table.ColumnStyles.Count; i++)
            {
                _table.ColumnStyles[i].Width = _memoryWidth[i];
            }
            for (var i = 0; i < _table.RowStyles.Count; i++)
            {
                _table.RowStyles[i].Height = _memoryHeight[i];
            }
            ForgetFocus();
        }

        public bool IsFocusing()
        {
            return _memoryWidth != null;
        }

        private float[] _memoryWidth;
        private float[] _memoryHeight;

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_resizing)
            {
                if (_inH >= 0 && _table.Width > 0)
                {
                    ForgetFocus();
                    var mp = e.Y / (float)_table.Height * 100;
                    SetHorizontalSplitPosition(_inH, mp);
                }
                if (_inV >= 0 && _table.Height > 0)
                {
                    ForgetFocus();
                    var mp = e.X / (float)_table.Width * 100;
                    SetVerticalSplitPosition(_inV, mp);
                }
            }
            else
            {
                var cw = _table.GetColumnWidths();
                var rh = _table.GetRowHeights();
                _inH = _inV = -1;
                int hval = 0, vval = 0;
                var margin = Math.Ceiling(_controlPadding / 2f) + 3;

                //todo: rowspan checks

                for (var i = 0; i < rh.Length - 1; i++)
                {
                    hval += rh[i];
                    var top = hval - margin;
                    var bottom = hval + margin;
                    if (e.X <= margin || e.X >= _table.Width - margin || e.Y <= top || e.Y >= bottom) continue;
                    _inH = i;
                    break;
                }

                for (var i = 0; i < cw.Length - 1; i++)
                {
                    vval += cw[i];
                    var left = vval - margin;
                    var right = vval + margin;
                    if (e.Y <= margin || e.Y >= _table.Height - margin || e.X <= left || e.X >= right) continue;
                    _inV = i;
                    break;
                }

                if (_inH >= 0 && _inV >= 0) _table.Cursor = Cursors.SizeAll;
                else if (_inV >= 0) _table.Cursor = Cursors.SizeWE;
                else if (_inH >= 0) _table.Cursor = Cursors.SizeNS;
                else _table.Cursor = Cursors.Default;
            }
        }

        private void OnMouseLeave(object sender, EventArgs e)
        {
            if (!_resizing) _table.Cursor = Cursors.Default;
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (_inV >= 0 || _inH >= 0) _resizing = true;
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            _resizing = false;
        }

        private void OnMouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (_inH >= 0 && _inH < _table.RowCount - 1)
            {
                ForgetFocus();
                SetHorizontalSplitPosition(_inH, (_inH + 1f) / _table.RowCount * 100);
            }
            if (_inV >= 0)
            {
                ForgetFocus();
                SetVerticalSplitPosition(_inV, (_inV + 1f) / _table.ColumnCount * 100);
            }
        }

        private void SetVerticalSplitPosition(int index, float percentage)
        {
            percentage = Math.Min(100, Math.Max(0, percentage));
            if (_table.ColumnCount == 0 || index < 0 || index >= _table.ColumnCount - 1 || _table.Width <= 0) return;

            var widths = _table.ColumnStyles.OfType<ColumnStyle>().Select(x => x.Width).ToList();
            var currentPercent = widths.GetRange(0, index + 1).Sum();
            if (percentage < currentPercent)
            {
                // <--
                var diff = currentPercent - percentage;
                for (var i = index; i >= 0 && diff > 0; i--)
                {
                    var w = widths[i];
                    var nw = Math.Max(MinimumViewSize, w - diff);
                    widths[i] = nw;
                    widths[index + 1] += (w - nw);
                    diff -= (w - nw);
                }
            }
            else if (percentage > currentPercent)
            {
                // -->
                var diff = percentage - currentPercent;
                for (var i = index + 1; i < widths.Count && diff > 0; i++)
                {
                    var w = widths[i];
                    var nw = Math.Max(MinimumViewSize, w - diff);
                    widths[i] = nw;
                    widths[index] += (w - nw);
                    diff -= (w - nw);
                }
            }
            for (var i = 0; i < _table.ColumnCount; i++)
            {
                widths[i] = (float)Math.Round(widths[i] * 10) / 10;
                _table.ColumnStyles[i].Width = widths[i];
            }
        }

        private void SetHorizontalSplitPosition(int index, float percentage)
        {
            percentage = Math.Min(100, Math.Max(0, percentage));
            if (_table.RowCount == 0 || index < 0 || index >= _table.RowCount - 1 || _table.Height <= 0) return;

            var heights = _table.RowStyles.OfType<RowStyle>().Select(x => x.Height).ToList();
            var currentPercent = heights.GetRange(0, index + 1).Sum();
            if (percentage < currentPercent)
            {
                // <--
                var diff = currentPercent - percentage;
                for (var i = index; i >= 0 && diff > 0; i--)
                {
                    var h = heights[i];
                    var nh = Math.Max(MinimumViewSize, h - diff);
                    heights[i] = nh;
                    heights[index + 1] += (h - nh);
                    diff -= (h - nh);
                }
            }
            else if (percentage > currentPercent)
            {
                // -->
                var diff = percentage - currentPercent;
                for (var i = index + 1; i < heights.Count && diff > 0; i++)
                {
                    var h = heights[i];
                    var nh = Math.Max(MinimumViewSize, h - diff);
                    heights[i] = nh;
                    heights[index] += (h - nh);
                    diff -= (h - nh);
                }
            }
            for (var i = 0; i < _table.RowCount; i++)
            {
                heights[i] = (float)Math.Round(heights[i] * 10) / 10;
                _table.RowStyles[i].Height = heights[i];
            }
        }
    }
}
