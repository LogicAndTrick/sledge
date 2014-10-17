using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Sledge.Gui.Attributes;
using Sledge.Gui.Interfaces;
using Padding = System.Windows.Forms.Padding;

namespace Sledge.Gui.WinForms.Controls
{
    [ControlImplementation("WinForms")]
    public class WinFormsTable : WinFormsContainer, ITable
    {
        private TableLayoutPanel _table;

        public int ControlPadding { get; set; }

        public WinFormsTable()
            : base(new TableLayoutPanel())
        {
            _table = (TableLayoutPanel)Control;
        }

        public int[] GetColumnWidths()
        {
            return _table.GetColumnWidths();
        }

        public int[] GetRowHeights()
        {
            return _table.GetRowHeights();
        }

        public void Insert(int row, int column, IControl child, int rowSpan = 1, int columnSpan = 1, bool rowFill = false, bool columnFill = false)
        {
            Insert(NumChildren, child, new ContainerMetadata
            {
                {"Row", row},
                {"Column", column},
                {"RowSpan", rowSpan},
                {"ColumnSpan", columnSpan},
                {"RowFill", rowFill},
                {"ColumnFill", columnFill}
            });
        }

        public override void Insert(int index, IControl child, ContainerMetadata metadata)
        {
            if (metadata.Get("Row", -1) < 0) throw new Exception("The Row must be specified and non-negative.");
            if (metadata.Get("Column", -1) < 0) throw new Exception("The Column must be specified and non-negative.");
            if (metadata.Get("RowSpan", 0) <= 0) metadata["RowSpan"] = 1;
            if (metadata.Get("ColumnSpan", 0) <= 0) metadata["ColumnSpan"] = 1;
            if (!metadata.Get("RowFill", false)) metadata["RowFill"] = false;
            if (!metadata.Get("ColumnFill", false)) metadata["ColumnFill"] = false;
            base.Insert(index, child, metadata);
        }

        public void SetColumnWidth(int column, int width)
        {
            throw new NotImplementedException();
        }

        public void SetRowHeight(int row, int height)
        {
            throw new NotImplementedException();
        }

        public override Size PreferredSize
        {
            get
            {
                Dictionary<int, int> desiredWidths = new Dictionary<int, int>(), desiredHeights = new Dictionary<int, int>();
                foreach (var child in Children)
                {
                    var meta = Metadata[child];

                    var r = meta.Get<int>("Row");
                    var rs = meta.Get<int>("RowSpan");
                    var c = meta.Get<int>("Column");
                    var cs = meta.Get<int>("ColumnSpan");

                    var ps = child.PreferredSize;

                    for (var i = r; i < r + rs; i++)
                    {
                        if (desiredHeights.ContainsKey(i)) desiredHeights[i] = Math.Max(desiredHeights[i], (ps.Height / rs) + ControlPadding);
                        else desiredHeights[i] = ps.Height;
                    }

                    for (var i = c; i < r + cs; i++)
                    {
                        if (desiredWidths.ContainsKey(i)) desiredWidths[i] = Math.Max(desiredWidths[i], (ps.Width / cs) + ControlPadding);
                        else desiredWidths[i] = ps.Width;
                    }
                }
                var height = desiredHeights.Sum(x => x.Value);
                var width = desiredWidths.Sum(x => x.Value);
                width += Margin.Left + Margin.Right;
                height += Margin.Top + Margin.Bottom;
                return new Size(width, height);
            }
        }

        protected override void CalculateLayout()
        {
            if (_table == null || NumChildren == 0) return;

            Control.SuspendLayout();

            int minCol = int.MaxValue, maxCol = 0;
            int minRow = int.MaxValue, maxRow = 0;
            Dictionary<int, bool> fillRows = new Dictionary<int, bool>(), fillCols = new Dictionary<int, bool>();
            Dictionary<int, int> desiredWidths = new Dictionary<int, int>(), desiredHeights = new Dictionary<int, int>();
            foreach (var child in Children)
            {
                var meta = Metadata[child];
                
                var r = meta.Get<int>("Row");
                var rs = meta.Get<int>("RowSpan");
                var c = meta.Get<int>("Column");
                var cs = meta.Get<int>("ColumnSpan");

                minCol = Math.Min(minCol, c);
                maxCol = Math.Max(maxCol, c + cs);
                minRow = Math.Min(minRow, r);
                maxRow = Math.Max(maxRow, r + rs);

                var ps = child.PreferredSize;

                for (var i = r; i < r + rs; i++)
                {
                    if (meta.Get<bool>("RowFill")) fillRows[i] = true;
                    else if (desiredHeights.ContainsKey(i)) desiredHeights[i] = Math.Max(desiredHeights[i], (ps.Height / rs) + ControlPadding);
                    else desiredHeights[i] = ps.Height;
                }

                for (var i = c; i < c + cs; i++)
                {
                    if (meta.Get<bool>("ColumnFill")) fillCols[i] = true;
                    else if (desiredWidths.ContainsKey(i)) desiredWidths[i] = Math.Max(desiredWidths[i], (ps.Width / cs) + ControlPadding);
                    else desiredWidths[i] = ps.Width;
                }

                _table.SetCellPosition(child.Control, new TableLayoutPanelCellPosition(c, r));
                _table.SetColumnSpan(child.Control, cs);
                _table.SetRowSpan(child.Control, rs);

                child.Control.Dock = DockStyle.Fill;
                child.Control.Margin = new Padding(ControlPadding, ControlPadding / 2, ControlPadding, (int)Math.Ceiling(ControlPadding / 2.0));
            }

            var desiredHeight = desiredHeights.Sum(x => x.Value);
            var desiredWidth = desiredWidths.Sum(x => x.Value);

            var numFillRows = fillRows.Count(x => !desiredHeights.ContainsKey(x.Key) || desiredHeights[x.Key] == 0);
            var numFillCols = fillCols.Count(x => !desiredWidths.ContainsKey(x.Key) || desiredWidths[x.Key] == 0);
            var fillRowVal = numFillRows == 0 ? 0 : 100.0f / numFillRows;
            var fillColVal = numFillCols == 0 ? 0 : 100.0f / numFillCols;
            var standardColVal = desiredWidth > 0 ? Math.Min(1, ActualSize.Width / (float)desiredWidth) : 1;
            var standardRowVal = desiredHeight > 0 ? Math.Min(1, ActualSize.Height / (float)desiredHeight) : 1;

            var numRows = maxRow - minRow;
            var numCols = maxCol - minCol;

            while (_table.ColumnStyles.Count > maxCol) _table.ColumnStyles.RemoveAt(0);
            while (_table.ColumnStyles.Count < maxCol) _table.ColumnStyles.Add(new ColumnStyle());
            _table.ColumnCount = maxCol;

            while (_table.RowStyles.Count > maxRow) _table.RowStyles.RemoveAt(0);
            while (_table.RowStyles.Count < maxRow) _table.RowStyles.Add(new RowStyle());
            _table.RowCount = maxRow;

            for (var i = minRow; i < maxRow; i++)
            {
                var preferredHeight = desiredHeights.ContainsKey(i) ? desiredHeights[i] : 0;
                var fill = preferredHeight == 0 && fillRows.ContainsKey(i) && fillRows[i];

                _table.RowStyles[i].SizeType = fill ? SizeType.Percent : SizeType.Absolute;
                _table.RowStyles[i].Height = fill ? fillRowVal : standardRowVal * preferredHeight + ControlPadding;
            }
            for (var i = minCol; i < maxCol; i++)
            {
                var preferredWidth = desiredWidths.ContainsKey(i) ? desiredWidths[i] : 0;
                var fill = preferredWidth == 0 && fillCols.ContainsKey(i) && fillCols[i];

                _table.ColumnStyles[i].SizeType = fill ? SizeType.Percent : SizeType.Absolute;
                _table.ColumnStyles[i].Width = fill ? fillColVal : standardColVal * preferredWidth + ControlPadding;
            }
            if (numFillRows == 0)
            {
                _table.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
                _table.RowCount += 1;
            }
            if (numFillCols == 0)
            {
                _table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
                _table.ColumnCount += 1;
            }

            Control.ResumeLayout();
        }
    }
}