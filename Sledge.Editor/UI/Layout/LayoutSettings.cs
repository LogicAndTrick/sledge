using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sledge.Editor.UI.Layout
{
    public partial class LayoutSettings : Form
    {
        private List<ViewportWindowConfiguration> _configurations; 
        public LayoutSettings(IEnumerable<ViewportWindowConfiguration> configs)
        {
            InitializeComponent();
            _configurations = configs.Select(x => new ViewportWindowConfiguration
            {
                Maximised = x.Maximised,
                Size = x.Size,
                WindowID = x.WindowID,
                Configuration = new TableSplitConfiguration
                {
                    Columns = x.Configuration.Columns,
                    Rows = x.Configuration.Rows,
                    Rectangles = new List<Rectangle>(x.Configuration.Rectangles)
                }
            }).ToList();

            WindowDropDown.Items.Clear();
            WindowDropDown.Items.AddRange(_configurations
                .Select(x => x.WindowID == 0 ? "Main Window" : "Window " + x.WindowID)
                .OfType<object>().ToArray());
            WindowDropDown.SelectedIndex = 0;
        }

        private ViewportWindowConfiguration SelectedConfiguration
        {
            get { return _configurations[WindowDropDown.SelectedIndex]; }
        }

        private void WindowDropDownSelectedIndexChanged(object sender, EventArgs e)
        {
            Rows.Value = SelectedConfiguration.Configuration.Rows;
            Columns.Value = SelectedConfiguration.Configuration.Columns;
            UpdateTableLayout();
        }

        private void RegisterPanel(Control panel)
        {
            panel.AllowDrop = true;
            panel.MouseDown += PanelMouseDown;
            panel.DragEnter += PanelDragEnter;
            panel.DragLeave += PanelDragLeave;
            panel.DragDrop += PanelDragDrop;
        }

        private void UnregisterPanel(Control panel)
        {
            panel.MouseDown -= PanelMouseDown;
            panel.DragEnter -= PanelDragEnter;
            panel.DragLeave -= PanelDragLeave;
            panel.DragDrop -= PanelDragDrop;
        }

        private Point _dragStart;

        private void PanelMouseDown(object sender, MouseEventArgs e)
        {
            var pos = TableLayout.GetPositionFromControl((Control)sender);
            _dragStart = new Point(pos.Column, pos.Row);
            ColourPanels(_dragStart);

            // This call blocks!
            ((Control)sender).DoDragDrop(_dragStart, DragDropEffects.Link);
            foreach (Control control in TableLayout.Controls) control.BackColor = Color.Black;
        }

        private void PanelDragEnter(object sender, DragEventArgs e)
        {
            var pos = TableLayout.GetPositionFromControl((Control)sender);
            var point = new Point(pos.Column, pos.Row);
            if (e.Data.GetDataPresent(typeof(Point))) e.Effect = DragDropEffects.Link;
            ColourPanels(point);
        }

        private void PanelDragLeave(object sender, EventArgs e)
        {
            ColourPanels(_dragStart);
        }

        private void PanelDragDrop(object sender, DragEventArgs e)
        {
            var startPoint = (Point) e.Data.GetData(typeof (Point));
            var pos = TableLayout.GetPositionFromControl((Control)sender);
            var endPoint = new Point(pos.Column, pos.Row);

            var minx = Math.Min(startPoint.X, endPoint.X);
            var maxx = Math.Max(startPoint.X, endPoint.X);
            var miny = Math.Min(startPoint.Y, endPoint.Y);
            var maxy = Math.Max(startPoint.Y, endPoint.Y);

            var rectangle = new Rectangle(minx, miny, maxx - minx + 1, maxy - miny + 1);
            SelectedConfiguration.Configuration.Rectangles.RemoveAll(x => x.IntersectsWith(rectangle));
            SelectedConfiguration.Configuration.Rectangles.Add(rectangle);
            UpdateTableLayout();

            _dragStart = Point.Empty;
        }

        private void ColourPanels(Point point)
        {
            foreach (Control control in TableLayout.Controls) control.BackColor = Color.Black;
            if (point.X == _dragStart.X && point.Y == _dragStart.Y)
            {
                var ctrl = TableLayout.GetControlFromPosition(point.X, point.Y);
                if (ctrl != null) ctrl.BackColor = Color.Green;
            }
            else
            {
                for (var i = Math.Min(point.X, _dragStart.X); i <= Math.Max(point.X, _dragStart.X); i++)
                {
                    for (var j = Math.Min(point.Y, _dragStart.Y); j <= Math.Max(point.Y, _dragStart.Y); j++)
                    {
                        var ctrl = TableLayout.GetControlFromPosition(i, j);
                        if (ctrl != null) ctrl.BackColor = Color.Blue;
                    }
                }
            }
        }

        private void UpdateTableLayout()
        {
            FixRectangles(SelectedConfiguration.Configuration);

            TableLayout.ColumnCount = (int)Columns.Value;
            TableLayout.ColumnStyles.Clear();
            for (var i = 0; i < TableLayout.ColumnCount; i++) TableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / TableLayout.ColumnCount));

            TableLayout.RowCount = (int)Rows.Value;
            TableLayout.RowStyles.Clear();
            for (var i = 0; i < TableLayout.RowCount; i++) TableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100f / TableLayout.RowCount));

            foreach (Control panel in TableLayout.Controls) UnregisterPanel(panel);
            TableLayout.Controls.Clear();

            var counter = 0;
            foreach (var rec in SelectedConfiguration.Configuration.Rectangles)
            {
                var panel = new Panel();
                panel.BackColor = Sledge.Settings.View.ViewportBackground;
                panel.Dock = DockStyle.Fill;
                TableLayout.Controls.Add(panel, rec.X, rec.Y);
                TableLayout.SetColumnSpan(panel, rec.Width);
                TableLayout.SetRowSpan(panel, rec.Height);
                RegisterPanel(panel);
                counter += 1;
            }
        }

        private void FixRectangles(TableSplitConfiguration configuration)
        {
            var cells = new bool[configuration.Rows, configuration.Columns];
            var list = new List<Rectangle>(configuration.Rectangles);
            configuration.Rectangles.RemoveAll(x => list.Where(y => y != x).Any(y => y.IntersectsWith(x)));
            foreach (var r in configuration.Rectangles.ToList())
            {
                if (r.X < 0 || r.X + r.Width > configuration.Columns || r.Y < 0 || r.Y + r.Height > configuration.Rows)
                {
                    configuration.Rectangles.Remove(r);
                    continue;
                }
                for (var i = r.X; i < r.X + r.Width; i++)
                {
                    for (var j = r.Y; j < r.Y + r.Height; j++)
                    {
                        cells[j, i] = true;
                    }
                }
            }
            for (var i = 0; i < cells.GetLength(0); i++)
            {
                for (var j = 0; j < cells.GetLength(1); j++)
                {
                    if (!cells[i,j]) configuration.Rectangles.Add(new Rectangle(j, i, 1, 1));
                }
            }
        }

        private void ApplyButtonClick(object sender, EventArgs e)
        {
            ViewportManager.SetWindowConfigurations(_configurations);
            Close();
        }

        private void CancelButtonClick(object sender, EventArgs e)
        {
            Close();
        }

        private void RowsValueChanged(object sender, EventArgs e)
        {
            SelectedConfiguration.Configuration.Rows = (int) Rows.Value;
            UpdateTableLayout();
        }

        private void ColumnsValueChanged(object sender, EventArgs e)
        {
            SelectedConfiguration.Configuration.Columns = (int) Columns.Value;
            UpdateTableLayout();
        }
    }
}
