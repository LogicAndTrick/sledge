﻿using System;
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
        private readonly static TableSplitConfiguration[] Presets;
        private readonly static Bitmap[] PresetPreviews;

        #region Presets

        static LayoutSettings()
        {
            Presets = new[]
            {
                // Default
                new TableSplitConfiguration
                {
                    Columns = 2,
                    Rows = 2,
                    Rectangles = new List<Rectangle>
                    {
                        new Rectangle(0, 0, 1, 1),
                        new Rectangle(0, 1, 1, 1),
                        new Rectangle(1, 0, 1, 1),
                        new Rectangle(1, 1, 1, 1),
                    }
                },
                // Twins
                new TableSplitConfiguration
                {
                    Columns = 2,
                    Rows = 1,
                    Rectangles = new List<Rectangle>
                    {
                        new Rectangle(0, 0, 1, 1),
                        new Rectangle(1, 0, 1, 1),
                    }
                },
                // Long left viewport
                new TableSplitConfiguration
                {
                    Columns = 2,
                    Rows = 2,
                    Rectangles = new List<Rectangle>
                    {
                        new Rectangle(0, 0, 1, 2),
                        new Rectangle(1, 0, 1, 1),
                        new Rectangle(1, 1, 1, 1),
                    }
                },
                // 9-grid
                new TableSplitConfiguration
                {
                    Columns = 3,
                    Rows = 3,
                    Rectangles = new List<Rectangle>
                    {
                        new Rectangle(0, 0, 1, 1),
                        new Rectangle(0, 1, 1, 1),
                        new Rectangle(0, 2, 1, 1),
                        new Rectangle(1, 0, 1, 1),
                        new Rectangle(1, 1, 1, 1),
                        new Rectangle(1, 2, 1, 1),
                        new Rectangle(2, 0, 1, 1),
                        new Rectangle(2, 1, 1, 1),
                        new Rectangle(2, 2, 1, 1),
                    }
                },
                // Triplets, vertical
                new TableSplitConfiguration
                {
                    Columns = 1,
                    Rows = 3,
                    Rectangles = new List<Rectangle>
                    {
                        new Rectangle(0, 0, 1, 1),
                        new Rectangle(0, 1, 1, 1),
                        new Rectangle(0, 2, 1, 1),
                    }
                },
                // Triplets, horizontal
                new TableSplitConfiguration
                {
                    Columns = 3,
                    Rows = 1,
                    Rectangles = new List<Rectangle>
                    {
                        new Rectangle(0, 0, 1, 1),
                        new Rectangle(1, 0, 1, 1),
                        new Rectangle(2, 0, 1, 1),
                    }
                },
                // Insanity
                new TableSplitConfiguration
                {
                    Columns = 4,
                    Rows = 4,
                    Rectangles = new List<Rectangle>
                    {
                        new Rectangle(0, 0, 1, 1),
                        new Rectangle(0, 1, 1, 1),
                        new Rectangle(0, 2, 1, 1),
                        new Rectangle(0, 3, 1, 1),
                        
                        new Rectangle(1, 0, 1, 1),
                        new Rectangle(2, 0, 1, 1),

                        new Rectangle(1, 1, 2, 2),
                        
                        new Rectangle(1, 3, 1, 1),
                        new Rectangle(2, 3, 1, 1),
                        
                        new Rectangle(3, 0, 1, 1),
                        new Rectangle(3, 1, 1, 1),
                        new Rectangle(3, 2, 1, 1),
                        new Rectangle(3, 3, 1, 1),
                    }
                }
            };
            PresetPreviews = new Bitmap[Presets.Length];
            var sides = new[] { 2, 2, 0, 0 };
            var blocks = new[] { 12, 5, 4, 3 };
            var gaps = new[] { 0, 2, 2, 1 };
            for (var i = 0; i < Presets.Length; i++)
            {
                var ps = Presets[i];
                var img = new Bitmap(16, 16);
                using (var g = System.Drawing.Graphics.FromImage(img))
                {
                    var xi = ps.Columns - 1;
                    var yi = ps.Rows - 1;
                    foreach (var rec in ps.Rectangles)
                    {
                        var x = sides[xi] + blocks[xi] * rec.X + gaps[xi] * rec.X;
                        var y = sides[yi] + blocks[yi] * rec.Y + gaps[yi] * rec.Y;
                        var w = blocks[xi] * rec.Width + gaps[xi] * (rec.Width - 1);
                        var h = blocks[yi] * rec.Height + gaps[yi] * (rec.Height - 1); ;
                        g.FillRectangle(System.Drawing.Brushes.Black, x, y, w, h);
                    }
                }
                PresetPreviews[i] = img;
            }
        }

        private void LoadPreviews()
        {
            for (var i = 0; i < Presets.Length; i++)
            {
                var preset = Presets[i];
                var preview = PresetPreviews[i];
                var button = new Button
                {
                    Width = 24,
                    Height = 24,
                    Image = preview,
                    ImageAlign = ContentAlignment.MiddleCenter,
                    Padding = new Padding(0),
                    Margin = new Padding(0)
                };
                button.Click += (s, e) =>
                {
                    Rows.Value = preset.Rows;
                    Columns.Value = preset.Columns;
                    SelectedConfiguration.Configuration = new TableSplitConfiguration
                    {
                        Columns = preset.Columns,
                        Rows = preset.Rows,
                        Rectangles = new List<Rectangle>(preset.Rectangles)
                    };
                    UpdateTableLayout();
                };
                PresetButtonPanel.Controls.Add(button);
            }
        }

        #endregion


        private List<ViewportWindowConfiguration> _configurations; 
        public LayoutSettings(IEnumerable<ViewportWindowConfiguration> configs)
        {
            InitializeComponent();
            LoadPreviews();

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
