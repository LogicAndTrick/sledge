using Sledge.Gui.Attributes;

namespace Sledge.Gui.Interfaces
{
    [ControlInterface]
    public interface ITable : IContainer
    {
        int[] GetColumnWidths();
        int[] GetRowHeights();
        int ControlPadding { get; set; }
        void Insert(int row, int column, IControl child, int rowSpan = 1, int columnSpan = 1);
        void SetColumnWidth(int column, int width);
        void SetRowHeight(int row, int height);
    }
}