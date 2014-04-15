using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Sledge.Editor.UI
{
    public class ViewportWindowConfiguration
    {
        private Rectangle _size;
        public int WindowID { get; set; }

        public Rectangle Size
        {
            get { return _size; }
            set { _size = value; ValidateSize(); }
        }

        private void ValidateSize()
        {
            if (_size.IsEmpty || _size.Width < 400 || _size.Height < 400)
            {
                _size = Screen.FromPoint(Point.Empty).Bounds;
            }
        }

        public TableSplitConfiguration Configuration { get; set; }
        public List<string> Viewports { get; set; }
        public bool Maximised { get; set; }

        public ViewportWindowConfiguration()
        {
            Viewports = new List<string>();
        }
    }
}