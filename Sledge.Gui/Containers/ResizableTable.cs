using Sledge.Gui.Interfaces;
using Sledge.Gui.Interfaces.Containers;
using Sledge.Gui.Structures;

namespace Sledge.Gui.Containers
{
    public class ResizableTable : ContainerBase<IResizableTable>, IResizableTable
    {
        public int ControlPadding
        {
            get { return Control.ControlPadding; }
            set { Control.ControlPadding = value; }
        }

        public int MinimumViewSize
        {
            get { return Control.MinimumViewSize; }
            set { Control.MinimumViewSize = value; }
        }

        public ResizableTableConfiguration Configuration
        {
            get { return Control.Configuration; }
            set { Control.Configuration = value; }
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

        public void ResetViews()
        {
            Control.ResetViews();
        }

        public void FocusOn(IControl ctrl)
        {
            Control.FocusOn(ctrl);
        }

        public void FocusOn(int rowIndex, int columnIndex)
        {
            Control.FocusOn(rowIndex, columnIndex);
        }

        public void Unfocus()
        {
            Control.Unfocus();
        }

        public bool IsFocusing()
        {
            return Control.IsFocusing();
        }
    }
}