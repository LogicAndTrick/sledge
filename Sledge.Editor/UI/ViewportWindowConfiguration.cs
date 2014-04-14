using System.Collections.Generic;
using System.Drawing;

namespace Sledge.Editor.UI
{
    public class ViewportWindowConfiguration
    {
        public int WindowID { get; set; }
        public Rectangle Size { get; set; }
        public TableSplitConfiguration Configuration { get; set; }
        public List<string> Viewports { get; set; }
        public bool Maximised { get; set; }

        public ViewportWindowConfiguration()
        {
            Viewports = new List<string>();
        }
    }
}