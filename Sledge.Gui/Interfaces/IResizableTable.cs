using Sledge.Gui.Attributes;

namespace Sledge.Gui.Interfaces
{
    [ControlInterface]
    public interface IResizableTable : ITable
    {
        int MinimumViewSize { get; set; }
        ResizableTableConfiguration Configuration { get; set; }
        void ResetViews();
        void FocusOn(IControl ctrl);
        void FocusOn(int rowIndex, int columnIndex);
        void Unfocus();
        bool IsFocusing();
    }
}