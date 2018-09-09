using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Sledge.BspEditor.Components;

namespace Sledge.BspEditor.Controls.Layout
{
    public class MapDocumentControlWindowConfiguration
    {
        public int WindowID { get; set; }

        public TableSplitConfiguration Configuration { get; set; }
        public List<float> RowSizes { get; set; }
        public List<float> ColumnSizes { get; set; }

        private Rectangle _size;
        public Rectangle Size
        {
            get => _size;
            set { _size = value; ValidateSize(); }
        }

        private void ValidateSize()
        {
            if (_size.IsEmpty || _size.Width < 400 || _size.Height < 400)
            {
                _size = Screen.FromPoint(Point.Empty).Bounds;
            }
        }

        public bool Maximised { get; set; }

        public MapDocumentControlWindowConfiguration()
        {
            Configuration = new TableSplitConfiguration();
            RowSizes = new List<float>();
            ColumnSizes = new List<float>();
        }

        public MapDocumentControlWindowConfiguration(ViewportWindow window, MapDocumentContainer container)
        {
            WindowID = container.WindowID;
            Configuration = container.Table.Configuration.Clone();
            RowSizes = container.Table.RowSizes.ToList();
            ColumnSizes = container.Table.ColumnSizes.ToList();

            if (window != null)
            {
                Maximised = window.WindowState == FormWindowState.Maximized;
                Size = new Rectangle(window.Location, window.Size);
            }
        }

        public MapDocumentControlWindowConfiguration Clone()
        {
            return new MapDocumentControlWindowConfiguration
            {
                Size = Size,
                WindowID = WindowID,
                RowSizes = RowSizes.ToList(),
                ColumnSizes = ColumnSizes.ToList(),
                Configuration = Configuration.Clone(),
                Maximised = Maximised
            };
        }

        public static MapDocumentControlWindowConfiguration Default(int windowId)
        {
            return new MapDocumentControlWindowConfiguration
            {
                WindowID = windowId,
                Configuration = TableSplitConfiguration.Default(),
                RowSizes = new List<float> {50, 50},
                ColumnSizes = new List<float> {50, 50},
                Maximised = false,
                Size = Screen.PrimaryScreen.Bounds
            };
        }
    }
}