using Sledge.Gui.Interfaces;
using Sledge.Gui.Interfaces.Containers;

namespace Sledge.Gui.Containers
{
    public class Table : ContainerBase<ITable>, ITable
    {
        public int ControlPadding
        {
            get { return Control.ControlPadding; }
            set { Control.ControlPadding = value; }
        }

        public int[] GetColumnWidths()
        {
            return Control.GetColumnWidths();
        }

        public int[] GetRowHeights()
        {
            return Control.GetRowHeights();
        }

        public void Insert(int row, int column, IControl child, int rowSpan = 1, int columnSpan = 1, bool rowFill = false, bool columnFill = false)
        {
            Control.Insert(row, column, child, rowSpan, columnSpan, rowFill, columnFill);
        }

        public void SetColumnWidth(int column, int width)
        {
            Control.SetColumnWidth(column, width);
        }

        public void SetRowHeight(int row, int height)
        {
            Control.SetRowHeight(row, height);
        }
    }
}